Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.WebFormUtility

Partial Public Class WFP_RespVisite
    Inherits System.Web.UI.UserControl

    Private _WucElement As Object
    Property WucElement() As Object
        Get
            Return _WucElement
        End Get
        Set(ByVal value As Object)
            _WucElement = value
        End Set
    End Property
    
    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDSRegioni.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        SqlDSProvince.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        SqlDSRegPrElenco.ConnectionString = dbCon.getConnectionString(TipoDB.dbScadenzario)
        If (Not IsPostBack) Then
            lblMessUtente.Text = ""
            Session(IDRESPVISITE) = 0
        End If
        WUC_RespVisite.WucElement = Me
    End Sub

    Protected Sub btnAggiorna_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If (WUC_RespVisite.Aggiorna()) Then
            ProgrammaticModalPopup.Hide()
            _WucElement.CallBackWFPAnagrRespVisite()
            Session(F_ANAGRRESPVISITE_APERTA) = False
        End If
    End Sub
    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        ProgrammaticModalPopup.Hide()
        WUC_RespVisite.SvuotaCampi()
        _WucElement.CancBackWFPAnagrRespVisite()
        Session(F_ANAGRRESPVISITE_APERTA) = False
    End Sub
    Protected Sub btnNuovo_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        lblMessUtente.Text = "Inserimento nuovo elemento in tabella"
        WUC_RespVisite.SetNewCodice(GetNewCodice)
    End Sub
    Private Function GetNewCodice() As Long
        Dim strSQL As String = ""
        strSQL = "Select MAX(ISNULL(Codice,0)) AS Codice From RespVisite"
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    If Not IsDBNull(ds.Tables(0).Rows(0).Item("Codice")) Then
                        GetNewCodice = ds.Tables(0).Rows(0).Item("Codice") + 1
                    Else
                        GetNewCodice = 1
                    End If
                    Exit Function
                Else
                    GetNewCodice = 1
                    Exit Function
                End If
            Else
                GetNewCodice = 1
                Exit Function
            End If
        Catch Ex As Exception
            GetNewCodice = -1
            Exit Function
        End Try

    End Function

    Public Sub Show()
        ProgrammaticModalPopup.Show()
    End Sub

    Public Sub SvuotaCampi()
        WUC_RespVisite.SvuotaCampi()
        lblMessUtente.Text = ""
        WUC_RespVisite.SetNewCodice(GetNewCodice)
    End Sub
    Public Sub FiltraRespArea()
        WUC_RespVisite.FiltraRespArea()
    End Sub

    Public Sub SetlblMessaggi(ByVal strMessaggio As String)
        lblMessUtente.Text = strMessaggio
    End Sub

    Private Sub ddlRegioni_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlRegioni.SelectedIndexChanged
        Session("CodRegione") = ddlRegioni.SelectedValue
        ddlProvince.DataBind()
        ddlProvince.Items.Clear()
        ddlProvince.Items.Add("")
        ddlProvince.DataBind()
        '-- mi riposiziono 
        ddlProvince.AutoPostBack = False
        ddlProvince.SelectedIndex = -1
        ddlProvince.AutoPostBack = True
    End Sub
    Protected Sub btnAbbinaRegPr_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim myID As String = Session(IDRESPVISITE)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If Not IsNumeric(myID) Then
            lblMessUtente.Text = "Selezionare prima il Responsabile Visita."
            Exit Sub
        ElseIf CInt(myID) < 1 Then
            lblMessUtente.Text = "Selezionare prima il Responsabile Visita."
            Exit Sub
        End If
        If ddlRegioni.SelectedIndex > 0 Then
            'ok
        Else
            lblMessUtente.Text = "Selezionare la Regione."
            Exit Sub
        End If
        '-cONTROLLI
        'Dim strSQL As String = "Select * From RespVisiteRegPr WHERE CodRespVisite=" + myID.Trim + " AND CodRegione=" + ddlRegioni.SelectedValue.Trim
        Dim strSQL As String = "Select * From RespVisiteRegPr WHERE CodRegione=" + ddlRegioni.SelectedValue.Trim
        Dim strProvincia As String = "NULL"
        If ddlProvince.SelectedIndex > 0 Then
            strSQL += " AND Provincia='" + ddlProvince.SelectedValue.Trim + "'"
            strProvincia = "'" + ddlProvince.SelectedValue.Trim + "'"
        End If
        Dim strErrore As String = ""
        If ddlProvince.SelectedIndex > 0 Then
            Dim mystrSQL As String = "Select * From RespVisiteRegPr WHERE CodRegione=" + ddlRegioni.SelectedValue.Trim
            mystrSQL += " AND ISNULL(Provincia,'') =''"
            If ckNewRK(mystrSQL, strErrore) = True Then
                lblMessUtente.Text = strErrore.Trim '"Abbinamento già presente."
                Exit Sub
            ElseIf strErrore.Trim <> "" Then
                lblMessUtente.Text = strErrore.Trim
                Exit Sub
            End If
        End If
        If ckNewRK(strSQL, strErrore) = True Then
            lblMessUtente.Text = strErrore.Trim '"Abbinamento già presente."
            Exit Sub
        ElseIf strErrore.Trim <> "" Then
            lblMessUtente.Text = strErrore.Trim
            Exit Sub
        End If
        strSQL = "declare @NewCodice as int " + vbCr
        strSQL += "select @NewCodice=MAX(Codice) FROM RespVisiteRegPr " + vbCr
        strSQL += "IF @NewCodice IS NULL " + vbCr
        strSQL += "SET @NewCodice=0 " + vbCr
        strSQL += "SET @NewCodice = @NewCodice + 1 " + vbCr
        strSQL += "INSERT INTO RespVisiteRegPr (Codice,CodRespVisite,CodRegione,Provincia) " + vbCr
        strSQL += "VALUES(@NewCodice," + myID.Trim + "," + ddlRegioni.SelectedValue.Trim + "," + strProvincia + ")"
        If OKExecute(strSQL, strErrore) = True Then
            lblMessUtente.Text = "Abbinamento inserito"
        ElseIf strErrore.Trim <> "" Then
            lblMessUtente.Text = strErrore.Trim
            Exit Sub
        End If
        SqlDSRegPrElenco.DataBind()
        GridViewBody.DataBind()
    End Sub
    Private Function ckNewRK(ByVal strSQL As String, ByRef strErrore As String) As Boolean
        strErrore = ""
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    strErrore = "Abbinamento già presente. " + GetDesRespVisite(ds.Tables(0).Rows(0).Item("CodRespVisite").ToString.Trim)
                    ckNewRK = True
                    Exit Function
                Else
                    ckNewRK = False
                    Exit Function
                End If
            Else
                ckNewRK = False
                Exit Function
            End If
        Catch Ex As Exception
            strErrore = Ex.Message.Trim
            ckNewRK = False
            Exit Function
        End Try
    End Function
    Private Function GetDesRespVisite(ByVal CodRespVisite As String) As String
        GetDesRespVisite = ""
        Dim strSQL As String = "SELECT * FROM RespVisite WHERE Codice=" + IIf(CodRespVisite.Trim = "", "-1", CodRespVisite.Trim)
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    GetDesRespVisite = ds.Tables(0).Rows(0).Item("Descrizione").ToString.Trim
                    Exit Function
                Else
                    Exit Function
                End If
            Else
                Exit Function
            End If
        Catch Ex As Exception
            GetDesRespVisite = Ex.Message.Trim
            Exit Function
        End Try
    End Function

    Private Function OKExecute(ByVal strSQL As String, ByRef strErrore As String) As Boolean
        strErrore = ""
        Dim ObjDB As New DataBaseUtility
        Try
            ObjDB.ExecuteQueryUpdate(TipoDB.dbScadenzario, strSQL)
            ObjDB = Nothing
            OKExecute = True
        Catch Ex As Exception
            strErrore = Ex.Message.Trim
            OKExecute = False
            Exit Function
        End Try
    End Function
    Protected Sub btnEliminaRegPr_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Try
            Dim RigaDel As Integer = GridViewBody.SelectedDataKey.Value
            If RigaDel <> 0 Then
                Dim strSQL As String = "delete from RespVisiteRegPr where codice=" + RigaDel.ToString.Trim
                Dim strErrore As String = ""
                If OKExecute(strSQL, strErrore) = True Then
                    lblMessUtente.Text = "Abbinamento eliminato"
                ElseIf strErrore.Trim <> "" Then
                    lblMessUtente.Text = strErrore.Trim
                    Exit Sub
                End If
                SqlDSRegPrElenco.DataBind()
                GridViewBody.DataBind()
            End If
        Catch Ex As Exception
            lblMessUtente.Text = "Nessun elemento selezionato. (Elimina)"
            Exit Sub
        End Try
    End Sub
End Class