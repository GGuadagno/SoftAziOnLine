Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Magazzino
Imports SoftAziOnLine.Formatta

Partial Public Class WUC_Magazzino
    Inherits System.Web.UI.UserControl

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

#Region "Metodi private - eventi"

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ModalPopup.WucElement = Me
        If (Not IsPostBack) Then
            Session(SWOP) = SWOPNESSUNA
            Session(SWOPDETTDOC) = SWOPNESSUNA
            Session(SWOPDETTDOCL) = SWOPNESSUNA
        End If
        Dim StrErrore As String = ""
        Dim TotArticoli As Integer = 0
        Dim TotMovimenti As Integer = 0
        Dim DataPrimoMovimento As Date
        Dim DataUltimoMovimento As Date

        Dim TotOrdCli As Integer = 0
        Dim DataPrimoOrdCli As Date
        Dim DataUltimoOrdCli As Date

        Dim TotOrdFor As Integer = 0
        Dim DataPrimoOrdFor As Date
        Dim DataUltimoOrdFor As Date
        Try

            If InfoPerRicalcoloGiacenza(TotArticoli, TotMovimenti, DataPrimoMovimento, DataUltimoMovimento, StrErrore, TotOrdCli, DataPrimoOrdCli, DataUltimoOrdCli, TotOrdFor, DataPrimoOrdFor, DataUltimoOrdFor) Then
                'SCRIVO LABEL
                lblTotArticoli.Text = "<B>Totale articoli: " & CStr(TotArticoli) & "</B>"
                lblTotMovimenti.Text = "<B>Totale movimenti magazzino: " & CStr(TotMovimenti) & "</B>"
                If Not (DataPrimoMovimento = CDate("01/01/1900")) Then
                    lblDataPrimoMovimento.Text = "<B>Data primo movimento magazzino: " & CStr(Format(DataPrimoMovimento, FormatoData)) & "</B>"
                Else
                    lblDataPrimoMovimento.Text = "<B>Data primo movimento magazzino: NON DEFINITO</B>"
                End If
                If Not (DataUltimoMovimento = CDate("01/01/1900")) Then
                    lblDataUltimoMovimento.Text = "<B>Data ultimo movimento magazzino: " & CStr(Format(DataUltimoMovimento, FormatoData)) & "</B>"
                Else
                    lblDataUltimoMovimento.Text = "<B>Data ultimo movimento magazzino: NON DEFINITO</B>"
                End If

                lblTotOrdCli.Text = "<B>Totale ordini clienti: " & CStr(TotOrdCli) & "</B>"
                If Not (DataPrimoOrdCli = CDate("01/01/1900")) Then
                    lblDataPrimoOrdCli.Text = "<B>Data primo ordine cliente: " & CStr(Format(DataPrimoOrdCli, FormatoData)) & "</B>"
                Else
                    lblDataPrimoOrdCli.Text = "<B>Data primo ordine cliente: NON DEFINITO</B>"
                End If
                If Not (DataUltimoOrdCli = CDate("01/01/1900")) Then
                    lblDataUltimoOrdCli.Text = "<B>Data ultimo ordine cliente: " & CStr(Format(DataUltimoOrdCli, FormatoData)) & "</B>"
                Else
                    lblDataUltimoOrdCli.Text = "<B>Data ultimo ordine cliente: NON DEFINITO</B>"
                End If

                lblTotOrdFor.Text = "<B>Totale ordini fornitori: " & CStr(TotOrdFor) & "</B>"
                If Not (DataPrimoOrdFor = CDate("01/01/1900")) Then
                    lblDataPrimoOrdFor.Text = "<B>Data primo ordine fornitore: " & CStr(Format(DataPrimoOrdFor, FormatoData)) & "</B>"
                Else
                    lblDataPrimoOrdFor.Text = "<B>Data primo ordine fornitore: NON DEFINITO</B>"
                End If
                If Not (DataUltimoOrdFor = CDate("01/01/1900")) Then
                    lblDataUltimoOrdFor.Text = "<B>Data ultimo ordine fornitore: " & CStr(Format(DataUltimoOrdFor, FormatoData)) & "</B>"
                Else
                    lblDataUltimoOrdFor.Text = "<B>Data ultimo ordine fornitore: NON DEFINITO</B>"
                End If


            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
            End If

        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
    End Sub

#End Region

    Protected Sub btnRicalcola_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnRicalcola.Click
        Dim strErrore As String = ""
        Dim SWNegativi As Boolean = False
        If Ricalcola_Giacenze("", strErrore, SWNegativi, True) Then 'giu190613 aggiunto SWNegativi
            If strErrore.Trim <> "" Then
                'If Session("SWConferma") = SWNO Then 'giu300423
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Ricalcolo giacenze", strErrore.Trim, WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
            '---------
            Session(MODALPOPUP_CALLBACK_METHOD) = "CallMenu"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            If SWNegativi Then
                ModalPopup.Show("Attenzione", "Sono presenti articoli in negativo.", WUC_ModalPopup.TYPE_CONFIRM_Y)
            Else
                ModalPopup.Show("Ricalcolo giacenze", "Avvenuto con successo.", WUC_ModalPopup.TYPE_CONFIRM_Y)
            End If
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = "CallMenu"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Ricalcolo giacenze", "Si è verificato un errore durante il ricalcolo delle giacenze: " & strErrore, WUC_ModalPopup.TYPE_CONFIRM_Y)
        End If
    End Sub
    Public Sub CallMenu()
        Session(SWOP) = SWOPNESSUNA
        Try
            Response.Redirect("WF_Menu.aspx?labelForm=Menu principale")
        Catch ex As Exception
            Response.Redirect("WF_Menu.aspx?labelForm=Menu principale")
        End Try
    End Sub
    Private Sub btnAnnulla_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAnnulla.Click
        CallMenu()
    End Sub
End Class