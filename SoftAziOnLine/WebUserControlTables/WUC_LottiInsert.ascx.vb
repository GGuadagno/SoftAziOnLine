Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.App
Imports SoftAziOnLine.Formatta
Imports System.Data.Sql
Imports System.Data.SqlClient

Partial Public Class WUC_LottiInsert

    Inherits System.Web.UI.UserControl
    Dim dsLotti As New DSDocumenti
    Dim dr As DSDocumenti.LottiRow
    Dim dt As New DSDocumenti.LottiDataTable
    Dim NCollo As Integer
    Dim rowLotti() As DSDocumenti.DocumentiDLottiRow
    Dim rowLotti2() As DSDocumenti.LottiRow
    Dim dvCaricati As DataView
    Dim WFPLotti1 As New WFP_LottiInsert
    Dim totArt As Integer
    Dim NRiga As Integer
    Dim row As DSDocumenti.DocumentiDLottiInsertRow

    Dim SqlAdapDocDettL As New SqlDataAdapter
    Dim SqlDbInserCmdL As New SqlCommand

    Public Enum CellIdxArt
        Riga = 1
        CodArt = 2
        DesArt = 3
        QtaEv = 4
    End Enum
#Region "Variabili private"

    Private _WucElement As Object

#End Region

#Region "Property"

    Property WucElement() As Object
        Get
            Return _WucElement
        End Get
        Set(ByVal value As Object)
            _WucElement = value
        End Set
    End Property

#End Region

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
       
        Try
            If String.IsNullOrEmpty(Session(F_CARICALOTTI)) Then
                gridLotti.DataSource = Nothing
                gridLotti.DataBind()
                GridViewDocD.DataSource = Nothing
                GridViewDocD.DataBind()
                GridViewDocDA.DataSource = Nothing
                GridViewDocDA.DataBind()
            ElseIf Session(F_CARICALOTTI) = True Then
                ' ''Call PopolaDb()
                ' ''Call SetPostBackGrid()
                dt = Session("dt")
                gridLotti.DataSource = dt
                gridLotti.DataBind()

                dsLotti = Session("POPOLAGRID")
                GridViewDocD.DataSource = dsLotti.DocumentiDLotti
                'GridViewDocD.DataBind()
                'giu180323
                GridViewDocDA.DataSource = dsLotti.DocumentiD
                'GridViewDocDA.DataBind()
                'GridViewDocDA.SelectedIndex = -1
                txtNserie.Focus()
            Else
                gridLotti.DataSource = Nothing
                gridLotti.DataBind()
                GridViewDocD.DataSource = Nothing
                GridViewDocD.DataBind()
                GridViewDocDA.DataSource = Nothing
                GridViewDocDA.DataBind()
            End If
            Dim valore As String = ""
            Dim myQta As String = ""
            _WucElement.GetTipoScansioneSL(valore, myQta)
            txtNCollo.Enabled = True : txtNCollo.BackColor = SEGNALA_OK
            If valore = "NLotto" Then
            ElseIf valore = "NSerie" Then
                If totArt > 1 Then txtNCollo.Text = "1"
                txtNCollo.Enabled = False : txtNCollo.BackColor = SEGNALA_OKLBL
            ElseIf valore = "NSerieNLotto" Then
                txtNCollo.Text = "1"
                txtNCollo.Enabled = False : txtNCollo.BackColor = SEGNALA_OKLBL
            End If
        Catch ex As Exception

        End Try
    End Sub



    Protected Sub PopolaDb()

        If String.IsNullOrEmpty(Session("passaDaDB")) Then

            CaricaGridLottiDB()
        End If

    End Sub

    Public Sub CaricaGridLottiDB()
        ' ''_WucElement.SetLblMessUtente("")
        Try
            dsLotti.Clear()
            dt.Clear()
            NRiga = 0
            If IsNothing(Session("NRiga")) Then
                NRiga = 0
            End If
            If String.IsNullOrEmpty(Session("NRiga")) Then
                NRiga = 0
            Else
                NRiga = CInt(Session("NRiga"))
            End If
            '-
            Dim dbConn As New dbStringaConnesioneFacade(Session(ESERCIZIO))
            Dim objConnection As New SqlConnection
            objConnection.ConnectionString = dbConn.getConnectionString(TipoDB.dbSoftAzi)
            '-
            Dim cmdArt As New SqlCommand("get_DocDByIDDocumenti", objConnection)
            cmdArt.CommandType = CommandType.StoredProcedure
            cmdArt.Parameters.Add("@IDDocumenti", SqlDbType.Int).Value = Session(IDDOCUMENTI)
            Dim sqlDatpArt As New SqlDataAdapter(cmdArt)
            sqlDatpArt.SelectCommand.Connection = objConnection
            sqlDatpArt.SelectCommand.CommandType = CommandType.StoredProcedure
            dsLotti.DocumentiD.Clear()
            sqlDatpArt.Fill(dsLotti.DocumentiD)
            Dim RowDett As DSDocumenti.DocumentiDRow
            For Each RowDett In dsLotti.DocumentiD.Select("ISNULL(Cod_Articolo,'')='' or Qta_Evasa=0")
                RowDett.Delete()
            Next
            '--------
            Dim cmd As New SqlCommand("get_DocDLByIDDocRiga", objConnection)
            cmd.CommandType = CommandType.StoredProcedure
            cmd.Parameters.Add("@IDDocumenti", SqlDbType.Int).Value = Session(IDDOCUMENTI)
            cmd.Parameters.Add("@Riga", SqlDbType.Int).Value = NRiga
            Dim sqlDatp As New SqlDataAdapter(cmd)
            sqlDatp.SelectCommand.Connection = objConnection
            sqlDatp.SelectCommand.CommandType = CommandType.StoredProcedure
            sqlDatp.Fill(dsLotti.DocumentiDLotti)
            '-
            dsLotti.AcceptChanges()
            '----------------------------------------
            objConnection.Close()
            Session("POPOLAGRID") = dsLotti
            Session("passaDaDB") = "true"
            NCollo = 0
            If dsLotti.DocumentiDLotti.Count > 0 Then
                Dim dv As New DataView(dsLotti.DocumentiDLotti)
                If dv.Count > 0 Then dv.Sort = "NCollo ASC"
                NCollo = dsLotti.DocumentiDLotti.Rows(dv.Count - 1).Item("NCollo")
            End If
            Session("NCollo") = NCollo
            Session("dt") = dt

            'Per quanto apre con comando da pulsante
            Call SetPostBackGrid()
            txtNserie.Text = ""
            txtNserie.Focus()
        Catch ex As Exception
            _WucElement.SetLblMessUtenteRED("Errore caricamento lotti: " & ex.Message.Trim)
            _WucElement.setValoriRigaDett("", "", "", "", "", "", "", "", "", "", "", "", "", "", 0, "")
        End Try
    End Sub


    Protected Sub txtNserie_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtNserie.TextChanged
        Dim passo As String = "Inizio - txtNserie_TextChanged"
        Session("TipoCodiceLetto") = ""
        Try
            If String.IsNullOrEmpty(Session("NSerieChange")) Then
                If Not String.IsNullOrEmpty(txtNserie.Text) Then
                    Session("NSerieChange") = txtNserie.Text.Trim
                    txtNserie.Text = ""
                    '--
                    _WucElement.SetLblMessUtente("")
                    '-
                    Dim valore As String = "NSerie"
                    Dim myQta As String = ""
                    _WucElement.GetTipoScansioneSL(valore, myQta)
                    If IsNothing(myQta) Then
                        myQta = "0"
                    End If
                    If String.IsNullOrEmpty(myQta) Then
                        myQta = "0"
                    ElseIf Not IsNumeric(myQta) Then
                        myQta = "0"
                    End If
                    Session("QtaEvasa") = CInt(myQta)
                    'giu290323
                    Dim myQtaColli As String = txtNCollo.Text.Trim
                    If IsNothing(myQtaColli) Then
                        myQtaColli = "0"
                    End If
                    If String.IsNullOrEmpty(myQtaColli) Then
                        myQtaColli = "0"
                    ElseIf Not IsNumeric(myQtaColli) Then
                        myQtaColli = "0"
                    End If
                    '----------
                    If valore = "NLotto" Then
                        Session("TipoScansioneSL") = "NLotto"
                    ElseIf valore = "NSerie" Then
                        Session("TipoScansioneSL") = "NSerie"
                        If CInt(myQtaColli) > 1 Then myQtaColli = "1"
                    ElseIf valore = "NSerieNLotto" Then
                        Session("TipoScansioneSL") = "NSerieNLotto"
                        myQtaColli = "1"
                    Else
                        Session("TipoScansioneSL") = "NSerie"
                        If CInt(myQtaColli) > 1 Then myQtaColli = "1"
                    End If
                    '-
                    passo = "Inizio - controlliNSerie"
                    If controlliNSerie(Session("NSerieChange").ToString.Trim) Then 'non esiste OK Inserisco
                        passo = "Inizio - Qtà Evasa e Qtà Lotti"
                        dr = Nothing
                        NCollo = 0

                        dt = Session("dt")
                        dsLotti = Session("POPOLAGRID")

                        totArt = 0
                        totArt = Session("QtaEvasa")
                        Dim TotQtaColli As Integer = GetQtaColli()
                        If TotQtaColli + CInt(myQtaColli) > totArt Then
                            If CInt(myQtaColli) = 0 Then
                                Session("NSerieChange") = ""
                                _WucElement.SetLblMessUtenteRED("Sono già stati inseriti N°Serie/Lotti sufficienti!.<br>Totale N°Colli: RICHIESTI: " + (TotQtaColli - totArt).ToString.Trim + " - ASSEGNATI: " + TotQtaColli.ToString.Trim + ".")
                            ElseIf String.IsNullOrEmpty(Session("TipoScansioneSL")) Then 'default NSerie
                                Session("NSerieChange") = ""
                                _WucElement.SetLblMessUtenteRED("Qtà Colli SUPERIORE al N°Colli RICHIESTI!.<br>Totale N°Colli: RICHIESTI: " + (TotQtaColli - totArt).ToString.Trim + " - ASSEGNATI: " + TotQtaColli.ToString.Trim + ".")
                            ElseIf Session("TipoScansioneSL") <> "NSerieNLotto" Then
                                Session("NSerieChange") = ""
                                _WucElement.SetLblMessUtenteRED("Qtà Colli SUPERIORE al N°Colli RICHIESTI!.<br>Totale N°Colli: RICHIESTI: " + (TotQtaColli - totArt).ToString.Trim + " - ASSEGNATI: " + TotQtaColli.ToString.Trim + ".")
                            Else
                                NCollo = Session("NCollo") 'Ultimo letto
                                Dim RowDL As DSDocumenti.LottiRow
                                If dt.Select("ID=" + NCollo.ToString.Trim).Length = 0 Then
                                    Session("NSerieChange") = ""
                                    _WucElement.SetLblMessUtenteRED("Sono già stati inseriti N°Serie/Lotti sufficienti!.<br>Totale N°Colli: RICHIESTI: " + (TotQtaColli - totArt).ToString.Trim + " - ASSEGNATI: " + TotQtaColli.ToString.Trim + ".")
                                Else
                                    For Each RowDL In dt.Select("ID=" + NCollo.ToString.Trim)
                                        If RowDL.IsNLottoNull Then
                                            RowDL.BeginEdit()
                                            RowDL.NLotto = Session("NSerieChange").ToString.Trim
                                            Session("TipoCodiceLetto") = "NLotto"
                                            RowDL.EndEdit()
                                            '-
                                            txtNserie.Text = ""
                                            If dt.Count > 0 Then dt.DefaultView.Sort = "ID DESC"
                                            Session("dt") = dt
                                            gridLotti.DataSource = dt
                                            gridLotti.DataBind()
                                        ElseIf RowDL.NLotto.Trim = "" Then
                                            RowDL.BeginEdit()
                                            RowDL.NLotto = Session("NSerieChange").ToString.Trim
                                            Session("TipoCodiceLetto") = "NLotto"
                                            RowDL.EndEdit()
                                            '-
                                            If dt.Count > 0 Then dt.DefaultView.Sort = "ID DESC"
                                            Session("dt") = dt
                                            gridLotti.DataSource = dt
                                            gridLotti.DataBind()
                                        Else
                                            Session("NSerieChange") = ""
                                            _WucElement.SetLblMessUtenteRED("Sono già stati inseriti N°Serie/Lotti sufficienti!.<br>Totale N°Colli: RICHIESTI: " + (TotQtaColli - totArt).ToString.Trim + " - ASSEGNATI: " + TotQtaColli.ToString.Trim + ".")
                                        End If
                                        Exit For
                                    Next
                                End If
                                '--------
                            End If
                        ElseIf CInt(myQtaColli) = 0 Then
                            Session("NSerieChange") = ""
                            _WucElement.SetLblMessUtenteRED("Sono già stati inseriti N°Serie/Lotti sufficienti!.<br>Totale N°Colli: RICHIESTI: " + (TotQtaColli - totArt).ToString.Trim + " - ASSEGNATI: " + TotQtaColli.ToString.Trim + ".")
                        Else
                            passo = "Inizio - Aggiunta riga Serie/Lotti"
                            If String.IsNullOrEmpty(Session("TipoScansioneSL")) Then 'default NSerie
                                Session("TipoScansioneSL") = "NSerie"
                                Session("TipoCodiceLetto") = "NSerie"
                                NCollo = Session("NCollo")
                                NCollo = NCollo + 1

                                dr = dt.NewRow

                                dr.ID = NCollo
                                dr.NSerie = Session("NSerieChange").ToString.Trim
                                dr.QtaColli = CInt(myQtaColli)
                                dr.NLotto = ""
                                dt.Rows.Add(dr)
                            ElseIf Session("TipoScansioneSL") = "NSerie" Then
                                NCollo = Session("NCollo")
                                NCollo = NCollo + 1

                                dr = dt.NewRow

                                dr.ID = NCollo
                                dr.NSerie = Session("NSerieChange").ToString.Trim
                                Session("TipoCodiceLetto") = "NSerie"
                                dr.QtaColli = CInt(myQtaColli)
                                dr.NLotto = ""
                                dt.Rows.Add(dr)
                            ElseIf Session("TipoScansioneSL") = "NLotto" Then
                                NCollo = Session("NCollo")
                                NCollo = NCollo + 1

                                dr = dt.NewRow

                                dr.ID = NCollo
                                dr.NSerie = ""
                                dr.NLotto = Session("NSerieChange").ToString.Trim
                                Session("TipoCodiceLetto") = "NLotto"
                                dr.QtaColli = CInt(myQtaColli)
                                dt.Rows.Add(dr)
                            ElseIf dt.Count = 0 Then 'nessuna lettura prec ok aggiungo 
                                NCollo = Session("NCollo")
                                NCollo = NCollo + 1

                                dr = dt.NewRow

                                dr.ID = NCollo
                                dr.NSerie = Session("NSerieChange").ToString.Trim
                                Session("TipoCodiceLetto") = "NSerie"
                                dr.QtaColli = CInt(myQtaColli)
                                dr.NLotto = ""
                                dt.Rows.Add(dr)
                            Else 'entrambi allora se esiste un rk,l'ultimo allora è il lotto
                                NCollo = Session("NCollo") 'Ultimo letto
                                Dim RowDL As DSDocumenti.LottiRow
                                If dt.Select("ID=" + NCollo.ToString.Trim).Length = 0 Then
                                    NCollo = Session("NCollo")
                                    NCollo = NCollo + 1

                                    dr = dt.NewRow

                                    dr.ID = NCollo
                                    dr.NSerie = Session("NSerieChange").ToString.Trim
                                    Session("TipoCodiceLetto") = "NSerie"
                                    dr.QtaColli = CInt(myQtaColli)
                                    dr.NLotto = ""
                                    dt.Rows.Add(dr)
                                Else
                                    For Each RowDL In dt.Select("ID=" + NCollo.ToString.Trim)
                                        If RowDL.IsNLottoNull Then
                                            RowDL.BeginEdit()
                                            RowDL.NLotto = Session("NSerieChange").ToString.Trim
                                            Session("TipoCodiceLetto") = "NLotto"
                                            RowDL.EndEdit()
                                        ElseIf RowDL.NLotto.Trim = "" Then
                                            RowDL.BeginEdit()
                                            RowDL.NLotto = Session("NSerieChange").ToString.Trim
                                            Session("TipoCodiceLetto") = "NLotto"
                                            RowDL.EndEdit()
                                        Else
                                            NCollo = Session("NCollo")
                                            NCollo = NCollo + 1

                                            dr = dt.NewRow

                                            dr.ID = NCollo
                                            dr.NSerie = Session("NSerieChange").ToString.Trim
                                            Session("TipoCodiceLetto") = "NSerie"
                                            dr.QtaColli = CInt(myQtaColli)
                                            dr.NLotto = ""
                                            dt.Rows.Add(dr)
                                        End If
                                        Exit For
                                    Next
                                End If
                                '--------
                            End If
                            '-
                            Session("NSerieChange") = ""
                            Session("NCollo") = NCollo
                            If dt.Count > 0 Then dt.DefaultView.Sort = "ID DESC"
                            Session("dt") = dt
                            gridLotti.DataSource = dt
                            gridLotti.DataBind()
                        End If
                        'GIU310323 RIDETERMINO LA QTA
                        _WucElement.GetTipoScansioneSL(valore, myQta)
                        If IsNothing(myQta) Then
                            myQta = "0"
                        End If
                        If String.IsNullOrEmpty(myQta) Then
                            myQta = "0"
                        ElseIf Not IsNumeric(myQta) Then
                            myQta = "0"
                        End If
                        Session("QtaEvasa") = CInt(myQta)
                        TotQtaColli = GetQtaColli()
                        totArt = Session("QtaEvasa") - TotQtaColli
                        If totArt < 1 Then totArt = 0
                        txtNCollo.Text = totArt.ToString.Trim
                        If totArt = 0 Then
                            If Session("TipoScansioneSL") = "NSerieNLotto" Then
                                If String.IsNullOrEmpty(Session("TipoCodiceLetto")) Then
                                    _WucElement.SetLblMessUtente("Totale N°Colli: RICHIESTI: " + txtNCollo.Text.Trim + " - ASSEGNATI: " + TotQtaColli.ToString.Trim + ".")
                                ElseIf Session("TipoCodiceLetto") = "NLotto" Then
                                    _WucElement.SetLblMessUtenteRED("Sono già stati inseriti N°Serie/Lotti sufficienti!.<br>Totale N°Colli: RICHIESTI: " + txtNCollo.Text.Trim + " - ASSEGNATI: " + TotQtaColli.ToString.Trim + ".")
                                Else
                                    _WucElement.SetLblMessUtente("Totale N°Colli: RICHIESTI: " + txtNCollo.Text.Trim + " - ASSEGNATI: " + TotQtaColli.ToString.Trim + "." + "<br>In attesa della II Lettura.....")
                                End If
                            Else
                                _WucElement.SetLblMessUtenteRED("Sono già stati inseriti N°Serie/Lotti sufficienti!.<br>Totale N°Colli: RICHIESTI: " + txtNCollo.Text.Trim + " - ASSEGNATI: " + TotQtaColli.ToString.Trim + ".")
                            End If
                        Else
                            _WucElement.SetLblMessUtente("Totale N°Colli: RICHIESTI: " + txtNCollo.Text.Trim + " - ASSEGNATI: " + TotQtaColli.ToString.Trim + ".")
                        End If
                        If valore = "NLotto" Then
                        ElseIf valore = "NSerie" Then
                            If totArt > 1 Then txtNCollo.Text = "1"
                        ElseIf valore = "NSerieNLotto" Then
                            txtNCollo.Text = "1"
                        End If
                        '---------------
                    Else
                        Session("NSerieChange") = ""
                        _WucElement.SetLblMessUtenteRED("Numero di serie già presente nell'archivio!")
                    End If
                End If
                Session("NSerieChange") = ""
            End If
        Catch ex As Exception
            txtNserie.Text = ""
            txtNserie.Focus()
            Session("NSerieChange") = ""
            _WucElement.SetLblMessUtenteRED("Errore nella scansione. Passo: " + passo + " - " + ex.Message.Trim)
        End Try
    End Sub

    Private Sub SetPostBackGrid()

        dt = Session("dt")
        gridLotti.DataSource = dt
        gridLotti.DataBind()

        dsLotti = Session("POPOLAGRID")
        GridViewDocD.DataSource = dsLotti.DocumentiDLotti
        GridViewDocD.DataBind()
        'giu180323
        GridViewDocDA.DataSource = dsLotti.DocumentiD
        GridViewDocDA.DataBind()
        GridViewDocDA.SelectedIndex = -1
        NRiga = 0
        If IsNothing(Session("NRiga")) Then
            NRiga = 0
        End If
        If String.IsNullOrEmpty(Session("NRiga")) Then
            NRiga = 0
        Else
            NRiga = CInt(Session("NRiga"))
        End If
        '-
        If GridViewDocDA.Rows.Count > 0 And NRiga > 0 Then
            For Each rowCTR As GridViewRow In GridViewDocDA.Rows
                If NRiga.ToString.Trim = rowCTR.Cells(CellIdxArt.Riga).Text.Trim Then
                    GridViewDocDA.SelectedIndex = rowCTR.RowIndex
                    Exit For
                End If
            Next
            If GridViewDocDA.SelectedIndex = -1 Then
                GridViewDocDA.SelectedIndex = 0
                Call GridViewDocDA_SelectedIndexChanged(Nothing, Nothing)
                Exit Sub
            End If
        Else
            _WucElement.SetLblMessUtenteRED("Articolo non trovato, SELEZIONARE L'ARTICOLO prima di iniziare a scansionare.")
            _WucElement.setValoriRigaDett("", "", "", "", "", "", "", "", "", "", "", "", "", "", 0, "")
        End If
        'GIU220323
        Dim valore As String = ""
        Dim myQta As String = ""
        _WucElement.GetTipoScansioneSL(valore, myQta)
        If IsNothing(myQta) Then
            myQta = "0"
        End If
        If String.IsNullOrEmpty(myQta) Then
            myQta = "0"
        ElseIf Not IsNumeric(myQta) Then
            myQta = "0"
        End If
        Session("QtaEvasa") = CInt(myQta)
        totArt = 0
        Dim TotQtaColli As Integer = GetQtaColli()
        totArt = Session("QtaEvasa") - TotQtaColli
        If totArt < 1 Then totArt = 0
        txtNCollo.Text = totArt.ToString.Trim
        If totArt = 0 Then
            _WucElement.SetLblMessUtenteRED("Sono già stati inseriti N°Serie/Lotti sufficienti!.<br>Totale N°Colli: RICHIESTI: " + totArt.ToString.Trim + " - ASSEGNATI: " + TotQtaColli.ToString.Trim + ".")
        Else
            _WucElement.SetLblMessUtente("Totale N°Colli: RICHIESTI: " + totArt.ToString.Trim + " - ASSEGNATI: " + TotQtaColli.ToString.Trim + ".")
        End If
        If valore = "NLotto" Then
        ElseIf valore = "NSerie" Then
            If totArt > 1 Then txtNCollo.Text = "1"
        ElseIf valore = "NSerieNLotto" Then
            txtNCollo.Text = "1"
        End If
        
        txtNserie.Focus()
    End Sub
    'GIU260323
    Public Function GetQtaColli() As Integer
        GetQtaColli = 0
        Try
            dsLotti = Session("POPOLAGRID")
            rowLotti = Nothing
            rowLotti = dsLotti.DocumentiDLotti.Select("")
            For i = 0 To rowLotti.Count - 1 Step 1
                GetQtaColli += rowLotti(i).QtaColli 'Rows(i).Item("QtaColli")
            Next
            '-
            dt = Session("dt")
            rowLotti2 = Nothing
            rowLotti2 = dt.Select("")
            For i = 0 To rowLotti2.Count - 1 Step 1
                GetQtaColli += dt(i).QtaColli
            Next
        Catch ex As Exception
            GetQtaColli = 0
        End Try
    End Function

    Private Function controlliNSerie(ByVal NSerie As String) As Boolean

        'Try
        dsLotti = Session("POPOLAGRID")
        rowLotti = Nothing
        rowLotti = dsLotti.DocumentiDLotti.Select("NSerie = '" + Controlla_Apice(NSerie) + "'")

        If rowLotti.Length > 0 Then
            Return False
            Exit Function
        End If

        dt = Session("dt")
        rowLotti2 = Nothing
        rowLotti2 = dt.Select("NSerie = '" + Controlla_Apice(NSerie) + "'")
        If rowLotti2.Length > 0 Then
            Return False
            Exit Function
        Else
            Return True
            Exit Function
        End If

        'Catch
        '    Return False
        'End Try

    End Function

    Private Sub gridLotti_RowDeleting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewDeleteEventArgs) Handles gridLotti.RowDeleting

        dt = Session("dt")

        Dim RigaSel As Integer = 0
        RigaSel = e.RowIndex
        RigaSel = dt.Count - 1 - RigaSel

        dt.Rows(RigaSel).Delete()

        If dt.Count > 0 Then dt.DefaultView.Sort = "ID ASC"

        For i = RigaSel To dt.Count - 1 Step 1

            dt.Rows(i).Item("ID") = dt.Rows(i).Item("ID") - 1

        Next
        Try
            Session("NCollo") = dt.Rows(dt.Count - 1).Item("ID")
        Catch
            Session("NCollo") = 0
        End Try
        If dt.Count > 0 Then dt.DefaultView.Sort = "ID DESC"

        Session("dt") = dt

        gridLotti.DataSource = dt
        gridLotti.DataBind()
        'GIU310323 RIDETERMINO LA QTA
        Dim Valore As String = ""
        Dim myQta As String = ""
        Dim TotQtaColli As Integer = 0
        _WucElement.GetTipoScansioneSL(valore, myQta)
        If IsNothing(myQta) Then
            myQta = "0"
        End If
        If String.IsNullOrEmpty(myQta) Then
            myQta = "0"
        ElseIf Not IsNumeric(myQta) Then
            myQta = "0"
        End If
        Session("QtaEvasa") = CInt(myQta)
        TotQtaColli = GetQtaColli()
        totArt = Session("QtaEvasa") - TotQtaColli
        If totArt < 1 Then totArt = 0
        txtNCollo.Text = totArt.ToString.Trim
        If totArt = 0 Then
            '_WucElement.SetLblMessUtenteRED("Sono già stati inseriti N°Serie/Lotti sufficienti!.<br>Totale N°Colli: RICHIESTI: " + txtNCollo.Text.Trim + " - ASSEGNATI: " + TotQtaColli.ToString.Trim + ".")
        Else
            _WucElement.SetLblMessUtente("Totale N°Colli: RICHIESTI: " + txtNCollo.Text.Trim + " - ASSEGNATI: " + TotQtaColli.ToString.Trim + ".")
        End If
        If valore = "NLotto" Then
        ElseIf Valore = "NSerie" Then
            If totArt > 1 Then txtNCollo.Text = "1"
        ElseIf valore = "NSerieNLotto" Then
            txtNCollo.Text = "1"
        End If
        '---------------

    End Sub

    Private Function SetCdmDAdp() As Boolean

        Dim dbConn As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        Dim objConnection As New SqlConnection
        objConnection.ConnectionString = dbConn.getConnectionString(TipoDB.dbSoftAzi)
        '
        SqlDbInserCmdL.CommandText = "insert_DocDLByIDDocRiga"
        SqlDbInserCmdL.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbInserCmdL.Connection = objConnection
        SqlDbInserCmdL.Parameters.AddRange( _
            New System.Data.SqlClient.SqlParameter() {New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.[Variant], 0, System.Data.ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 0, "IDDocumenti"), _
            New System.Data.SqlClient.SqlParameter("@Riga", System.Data.SqlDbType.Int, 0, "Riga"), _
            New System.Data.SqlClient.SqlParameter("@NCollo", System.Data.SqlDbType.Int, 0, "NCollo"), _
            New System.Data.SqlClient.SqlParameter("@Cod_Articolo", System.Data.SqlDbType.NVarChar, 0, "Cod_Articolo"), _
            New System.Data.SqlClient.SqlParameter("@Lotto", System.Data.SqlDbType.NVarChar, 0, "Lotto"), _
            New System.Data.SqlClient.SqlParameter("@QtaColli", System.Data.SqlDbType.Int, 0, "QtaColli"), _
            New System.Data.SqlClient.SqlParameter("@Sfusi", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Sfusi", System.Data.DataRowVersion.Current, Nothing), _
            New System.Data.SqlClient.SqlParameter("@NSerie", System.Data.SqlDbType.NVarChar, 0, "NSerie")})

        SqlAdapDocDettL.InsertCommand = SqlDbInserCmdL
    End Function


    Public Sub btnOk_Click(ByVal Nriga As Integer, ByVal CodArt As String, ByVal dt As DSDocumenti.LottiDataTable, ByVal IDDoc As Integer)
        If dt.Count = 0 Then Exit Sub
        'Inserire per salvatagggio dati
        Try
            Call SetCdmDAdp()

            'dt = Nothing

            If dt.Count > 0 Then dt.DefaultView.Sort = "ID ASC"
            Dim dslotti As New DSDocumenti
            Try
                Dim myTipoEv As String = Session("TipoScansioneSL")
                If String.IsNullOrEmpty(myTipoEv) Then
                    'Proseguo
                ElseIf myTipoEv = "NSerieNLotto" Then
                    Dim myNSerie As String = "" : Dim myNLotto As String = ""
                    For i = 0 To dt.Count - 1
                        Try
                            myNLotto = IIf(dt(i).IsNLottoNull, "", dt(i).NLotto.Trim)
                        Catch ex As Exception
                            myNLotto = ""
                        End Try
                        Try
                            myNSerie = IIf(dt(i).IsNSerieNull, "", dt(i).NSerie.Trim)
                        Catch ex As Exception
                            myNSerie = ""
                        End Try
                        If myNLotto.Trim = "" Or myNSerie.Trim = "" Then
                            _WucElement.SetLblMessUtenteRED("ATTENZIONE, PREVISTO DOPPIA LETTURA, una delle righe scansionate manca della seconda lettura")
                            Exit For
                        End If
                    Next
                End If
            Catch ex As Exception

            End Try
            '-
            dslotti.DocumentiDLottiInsert.Clear()
            For i = 0 To dt.Count - 1
                row = dslotti.DocumentiDLottiInsert.NewRow

                row.Item("IDDocumenti") = IDDoc
                row.Item("Riga") = Nriga
                row.Item("NCollo") = dt(i).ID
                row.Item("Cod_Articolo") = CodArt
                row.Item("QtaColli") = dt(i).QtaColli
                Try
                    row.Item("Lotto") = IIf(dt(i).IsNLottoNull, "", dt(i).NLotto.Trim)
                Catch ex As Exception
                    row.Item("Lotto") = ""
                End Try
                row.Item("Sfusi") = 0
                Try
                    row.Item("NSerie") = IIf(dt(i).IsNSerieNull, "", dt(i).NSerie.Trim)
                Catch ex As Exception
                    row.Item("NSerie") = ""
                End Try
                If CodArt.Trim = "" Then
                    _WucElement.SetLblMessUtenteRED("ERRORE, Codice Articolo mancante nell'inserimento Serie/Lotti in archivio")
                ElseIf row.Item("Lotto").ToString.Trim = "" And row.Item("NSerie").ToString.Trim = "" Then
                    _WucElement.SetLblMessUtenteRED("ERRORE, Serie/Lotti mancante nell'inserimento Serie/Lotti in archivio")
                Else
                    dslotti.DocumentiDLottiInsert.Rows.Add(row)
                End If

            Next

            SqlAdapDocDettL.Update(dslotti.DocumentiDLottiInsert)
        Catch ex As Exception
            _WucElement.SetLblMessUtenteRED("ERRORE, Inserimento N°Serie/Lotti in archivio: " + ex.Message.Trim)
        End Try

    End Sub

    Private Sub GridViewDocDA_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewDocDA.RowDataBound
        Try
            e.Row.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(GridViewDocDA, "Select$" + e.Row.RowIndex.ToString()))
            If e.Row.DataItemIndex > -1 Then
                If IsNumeric(e.Row.Cells(CellIdxArt.QtaEv).Text) Then
                    If CDec(e.Row.Cells(CellIdxArt.QtaEv).Text) <> 0 Then
                        e.Row.Cells(CellIdxArt.QtaEv).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxArt.QtaEv).Text), -1).ToString
                    Else
                        e.Row.Cells(CellIdxArt.QtaEv).Text = ""
                    End If
                End If
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub GridViewDocDA_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewDocDA.SelectedIndexChanged
        _WucElement.SetLblMessUtente("Scansionare il N° Serie / N° Lotto da caricare, secondo il metodo di Lettura selezionato.")
        Dim txtCodArtIns As String = ""
        dt = Session("dt")
        NRiga = 0
        If IsNothing(Session("NRiga")) Then
            NRiga = 0
        End If
        If String.IsNullOrEmpty(Session("NRiga")) Then
            NRiga = 0
        Else
            NRiga = CInt(Session("NRiga"))
        End If
        '-
        If GridViewDocDA.Rows.Count > 0 And dt.Count > 0 Then
            For Each rowCTR As GridViewRow In GridViewDocDA.Rows
                If NRiga.ToString.Trim = rowCTR.Cells(CellIdxArt.Riga).Text.Trim Then
                    txtCodArtIns = rowCTR.Cells(CellIdxArt.CodArt).Text.Trim
                    Exit For
                End If
            Next
            '--
            If txtCodArtIns.Trim <> "" Then
                Call btnOk_Click(CInt(Session("NRiga")), txtCodArtIns, Session("dt"), Session(IDDOCUMENTI))
            Else
                _WucElement.SetLblMessUtenteRED("ERRORE, N°Serie/Lotti non inseriti (Non trovata riga Articolo). Si prega di verificare la riga precedente")
            End If
        End If
        '- 
        Dim Riga As Integer = -1
        Dim txtDesArtIns As String = "" : Dim txtIVAIns As String = "" : Dim txtPrezzoIns As String = ""
        Dim txtQtaEv As String = "" : Dim txtQtaIns As String = "" : Dim txtSconto1Ins As String = "" : Dim txtUMIns As String = ""
        Dim LblPrezzoNetto As String = "" : Dim LblImportoRiga As String = "" : Dim lblGiacenza As String = "" : Dim lblGiacImp As String = ""
        Dim lblOrdFor As String = "" : Dim lblDataArr As String = "" : Dim lblLabelQtaRe As String = ""
        Try
            Dim row As GridViewRow = GridViewDocDA.SelectedRow
            Riga = IIf(Not IsNumeric(row.Cells(CellIdxArt.Riga).Text.Trim), 0, row.Cells(CellIdxArt.Riga).Text.Trim)
        Catch ex As Exception
            _WucElement.setValoriRigaDett("", "", "", "", "", "", "", "", "", "", "", "", "", "", 0, "")
            _WucElement.SetLblMessUtenteRED("ERRORE, Riga/Articolo non trovato nell'archivio!")
            Exit Sub
        End Try

        dsLotti = Session("POPOLAGRID")
        Dim RowsD() As DataRow = dsLotti.DocumentiD.Select("RIGA=" + Riga.ToString.Trim)
        Dim RowD As DSDocumenti.DocumentiDRow
        If RowsD.Length = 0 Then
            _WucElement.setValoriRigaDett("", "", "", "", "", "", "", "", "", "", "", "", "", "", 0, "")
            _WucElement.SetLblMessUtenteRED("ERRORE, Riga/Articolo non trovato nell'archivio!")
            Exit Sub
        ElseIf RowsD.Length = 1 Then
            ' proseguo
        Else
            _WucElement.setValoriRigaDett("", "", "", "", "", "", "", "", "", "", "", "", "", "", 0, "")
            _WucElement.SetLblMessUtenteRED("ERRORE, Riga/Articolo presente piu volte nell'archivio!")
            Exit Sub
        End If
        '-ok popolo le variabili
        Try
            RowD = RowsD(0)
            txtCodArtIns = RowD.Cod_Articolo.Trim
            txtDesArtIns = RowD.Descrizione.Trim
            txtIVAIns = FormattaNumero(RowD.Cod_Iva)

            Dim SWPNettoModificato As Boolean = False
            Try
                SWPNettoModificato = RowD.SWPNettoModificato
            Catch ex As Exception
                SWPNettoModificato = False
            End Try
            '---------------------------------------------
            Try
                If SWPNettoModificato = False Then
                    txtPrezzoIns = FormattaNumero(RowD.Prezzo, GetParamGestAzi(Session(ESERCIZIO)).Decimali_Prezzi)
                Else
                    txtPrezzoIns = FormattaNumero(RowD.Prezzo_Netto_Inputato, GetParamGestAzi(Session(ESERCIZIO)).Decimali_Prezzi)
                End If
            Catch ex As Exception
                txtPrezzoIns = "0"
            End Try
            txtQtaEv = FormattaNumero(RowD.Qta_Evasa, -1).Trim
            txtQtaIns = FormattaNumero(RowD.Qta_Ordinata, -1).Trim
            lblDataArr = FormattaNumero(RowD.Qta_Residua, -1).Trim
            txtSconto1Ins = FormattaNumero(RowD.Sconto_1, GetParamGestAzi(Session(ESERCIZIO)).Decimali_Sconto)
            txtUMIns = IIf(RowD.IsUmNull, "", RowD.Um.Trim)
            LblPrezzoNetto = FormattaNumero(RowD.Prezzo_Netto, GetParamGestAzi(Session(ESERCIZIO)).Decimali_Prezzi)
            LblImportoRiga = Format(RowD.Importo, FormatoValEuro)
            '------------------------------------------
            GetDatiGiacenza(txtCodArtIns.Trim, lblGiacenza, lblGiacImp, lblOrdFor, lblDataArr)
            '-
            _WucElement.setValoriRigaDett(txtCodArtIns.Trim, txtDesArtIns.Trim, txtIVAIns.Trim, txtPrezzoIns.Trim, txtQtaEv.Trim, _
                                      txtQtaIns.Trim, txtSconto1Ins.Trim, txtUMIns.Trim, LblPrezzoNetto.Trim, LblImportoRiga.Trim, _
                                      lblGiacenza.Trim, lblGiacImp.Trim, lblOrdFor.Trim, lblDataArr.Trim, Riga, lblLabelQtaRe.Trim)
            '--------
            dsLotti.DocumentiDLotti.Clear()
            dt.Clear()

            NRiga = Riga
            Session("NRiga") = Riga

            Dim dbConn As New dbStringaConnesioneFacade(Session(ESERCIZIO))
            Dim objConnection As New SqlConnection
            objConnection.ConnectionString = dbConn.getConnectionString(TipoDB.dbSoftAzi)
            '-
            Dim cmd As New SqlCommand("get_DocDLByIDDocRiga", objConnection)
            cmd.CommandType = CommandType.StoredProcedure
            cmd.Parameters.Add("@IDDocumenti", SqlDbType.Int).Value = Session(IDDOCUMENTI)
            cmd.Parameters.Add("@Riga", SqlDbType.Int).Value = NRiga
            Dim sqlDatp As New SqlDataAdapter(cmd)
            sqlDatp.SelectCommand.Connection = objConnection
            sqlDatp.SelectCommand.CommandType = CommandType.StoredProcedure
            sqlDatp.Fill(dsLotti.DocumentiDLotti)
            '-
            dsLotti.AcceptChanges()
            '----------------------------------------
            objConnection.Close()

            NCollo = 0
            If dsLotti.DocumentiDLotti.Count > 0 Then
                Dim dv As New DataView(dsLotti.DocumentiDLotti)
                If dt.Count > 0 Then dv.Sort = "NCollo ASC"
                NCollo = dsLotti.DocumentiDLotti.Rows(dv.Count - 1).Item("NCollo")
            End If
            Session("NCollo") = NCollo
            Session("POPOLAGRID") = dsLotti
            Session("dt") = dt
            gridLotti.DataSource = dt
            gridLotti.DataBind()

            GridViewDocD.DataSource = dsLotti.DocumentiDLotti
            GridViewDocD.DataBind()
            '-
            'GIU220323
            Dim valore As String = ""
            Dim myQta As String = ""
            _WucElement.GetTipoScansioneSL(valore, myQta)
            If IsNothing(myQta) Then
                myQta = "0"
            End If
            If String.IsNullOrEmpty(myQta) Then
                myQta = "0"
            ElseIf Not IsNumeric(myQta) Then
                myQta = "0"
            End If
            Session("QtaEvasa") = CInt(myQta)
            Dim TotQtaColli As Integer = GetQtaColli()
            totArt = Session("QtaEvasa") - TotQtaColli
            If totArt < 1 Then totArt = 0
            txtNCollo.Text = totArt.ToString.Trim
            If totArt = 0 Then
                _WucElement.SetLblMessUtenteRED("Sono già stati inseriti N°Serie/Lotti sufficienti!.<br>Totale N°Colli: RICHIESTI: " + txtNCollo.Text.Trim + " - ASSEGNATI: " + TotQtaColli.ToString.Trim + ".")
            Else
                _WucElement.SetLblMessUtente("Totale N°Colli: RICHIESTI: " + txtNCollo.Text.Trim + " - ASSEGNATI: " + TotQtaColli.ToString.Trim + ".")
            End If
            If valore = "NLotto" Then
            ElseIf valore = "NSerie" Then
                If totArt > 1 Then txtNCollo.Text = "1"
            ElseIf valore = "NSerieNLotto" Then
                txtNCollo.Text = "1"
            End If

            txtNserie.Text = ""
            txtNserie.Focus()

        Catch ex As Exception
            _WucElement.setValoriRigaDett("", "", "", "", "", "", "", "", "", "", "", "", "", "", 0, "")
            _WucElement.SetLblMessUtenteRED("ERRORE, Visualizza Articolo: " + ex.Message.Trim)
        End Try
    End Sub
    Private Function GetDatiGiacenza(ByVal CodArt As String, ByRef lblGiacenza As String, ByRef lblGiacImp As String, ByRef lblOrdFor As String, ByRef lblDataArr As String) As Boolean
        'GIU230920
        Dim myCMag As String = Session(IDMAGAZZINO)
        If IsNothing(myCMag) Then
            myCMag = "0"
        End If
        If String.IsNullOrEmpty(myCMag) Then
            myCMag = "0"
        End If
        '---------
        lblGiacenza = ""
        lblGiacImp = ""
        lblOrdFor = ""
        lblDataArr = ""
        If CodArt.Trim = "" Then Exit Function

        Dim strSQL As String = "" : Dim Comodo = ""
        Dim ObjDB As New DataBaseUtility
        Dim dsArt As New DataSet
        Dim rowArt() As DataRow
        If myCMag = "0" Then
            strSQL = "Select * From AnaMag WHERE Cod_Articolo = '" & CodArt.Trim & "'"
        Else
            strSQL = "Select * From ArtDiMag WHERE Codice_Magazzino=" + myCMag.Trim + " AND Cod_Articolo = '" & CodArt.Trim & "'"
        End If
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, dsArt)
            If (dsArt.Tables.Count > 0) Then
                If (dsArt.Tables(0).Rows.Count > 0) Then
                    rowArt = dsArt.Tables(0).Select()
                    lblGiacenza = FormattaNumero(IIf(IsDBNull(rowArt(0).Item("Giacenza")), 0, rowArt(0).Item("Giacenza")), -1)
                    If lblGiacenza = "0" Then lblGiacenza = ""
                    lblGiacImp = FormattaNumero(IIf(IsDBNull(rowArt(0).Item("Giac_Impegnata")), 0, rowArt(0).Item("Giac_Impegnata")), -1)
                    If lblGiacImp.Trim = "0" Then lblGiacImp = ""
                    If myCMag = "0" Then
                        lblOrdFor = FormattaNumero(IIf(IsDBNull(rowArt(0).Item("Ord_Fornit")), 0, rowArt(0).Item("Ord_Fornit")), -1)
                    Else
                        lblOrdFor = FormattaNumero(IIf(IsDBNull(rowArt(0).Item("Ordinati")), 0, rowArt(0).Item("Ordinati")), -1)
                    End If
                    '---
                    If lblOrdFor.Trim = "0" Then lblOrdFor = ""
                    lblDataArr = ""
                    Comodo = IIf(IsDBNull(rowArt(0).Item("Data_Arrivo")), "", rowArt(0).Item("Data_Arrivo"))
                    If IsDate(Comodo.Trim) Then
                        If CDate(Comodo.Trim) = DATANULL Then
                            Comodo = ""
                        End If
                    Else
                        Comodo = ""
                    End If
                    If Comodo.Trim <> "" Then
                        lblDataArr = Format(CDate(Comodo), FormatoData) & " "
                        Comodo = FormattaNumero(IIf(IsDBNull(rowArt(0).Item("QtaArrivoFornit")), 0, rowArt(0).Item("QtaArrivoFornit")), -1)
                        If CDec(Comodo.Trim) > 0 Then
                            lblDataArr += "(" & Comodo.Trim & ")"
                        End If
                    End If
                Else
                    If myCMag = "0" Then 'giu230920
                        _WucElement.SetLblMessUtenteRED("Attenzione", "Non trovato Cod.Articolo nella tabella Anagrafica articoli: " & CodArt.Trim)
                    Else
                        _WucElement.SetLblMessUtenteRED("Attenzione", "Non trovato Cod.Articolo nel Magazzino: (" + myCMag.Trim + ") - CArt.:" & CodArt.Trim)
                    End If
                    Exit Function
                End If
            Else
                If myCMag = "0" Then 'giu230920
                    _WucElement.SetLblMessUtenteRED("Attenzione", "Non trovato Cod.Articolo nella tabella Anagrafica articoli: " & CodArt.Trim)
                Else
                    _WucElement.SetLblMessUtenteRED("Attenzione", "Non trovato Cod.Articolo nel Magazzino: (" + myCMag.Trim + ") - CArt.:" & CodArt.Trim)
                End If
                Exit Function
            End If
        Catch Ex As Exception
            _WucElement.SetLblMessUtenteRED("Errore in DocumentiDett.GetDatiGiacenza", "Lettura articoli: " & Ex.Message)
            Exit Function
        End Try
    End Function

    Public Sub settxtNCollo(ByVal _Qta As String)
        txtNCollo.Text = _Qta.Trim
    End Sub
End Class