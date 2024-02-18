Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Magazzino
Imports SoftAziOnLine.Utility
Imports SoftAziOnLine.Formatta

Partial Public Class WUC_UtilityImpGiac
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
        Dim StrErrore As String = ""
        Dim TotArticoli As Integer = 0
        Dim TotOrdini As Integer = 0
        Dim DataPrimoOrdine As Date
        Dim DataUltimoOrdine As Date
        Try

            If InfoPerImpegnoGiacenzaOrdiniClienti(TotOrdini, DataPrimoOrdine, DataUltimoOrdine, StrErrore) Then
                'SCRIVO LABEL
                lblTotOrdini.Text = "<B>Totale ordini clienti: " & CStr(TotOrdini) & "</B>"
                If Not (DataPrimoOrdine = CDate("01/01/1900")) Then
                    lblDataPrimoOrdine.Text = "<B>Data primo ordine cliente: " & CStr(Format(DataPrimoOrdine, FormatoData)) & "</B>"
                Else
                    lblDataPrimoOrdine.Text = "<B>Data primo ordine cliente: NON DEFINITO</B>"
                End If

                If Not (DataUltimoOrdine = CDate("01/01/1900")) Then
                    lblDataUltimoOrdine.Text = "<B>Data ultimo ordine cliente: " & CStr(Format(DataUltimoOrdine, FormatoData)) & "</B>"
                Else
                    lblDataUltimoOrdine.Text = "<B>Data ultimo ordine cliente: NON DEFINITO</B>"
                End If
            Else

                ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
            End If

        Catch ex As Exception
            ModalPopup.Show("Errore", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
    End Sub

#End Region

    Protected Sub btnRicalcola_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnImpegna.Click
        'giu270814
        Dim SWNegativi As Boolean = False
        '----------
        Dim strErrore As String = ""
        Session(MODALPOPUP_CALLBACK_METHOD) = ""
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        If Ricalcola_Giacenze("", strErrore, SWNegativi, True) = False Then
            ModalPopup.Show("Impegno giacenze da ordini clienti", "Si è verificato un errore durante il ricalcolo delle giacenze: " & strErrore, WUC_ModalPopup.TYPE_ALERT)
        Else
            ModalPopup.Show("Impegno giacenze da ordini clienti", "Avvenuto con successo.", WUC_ModalPopup.TYPE_INFO)
        End If
    End Sub

    Private Sub btnAnnulla_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAnnulla.Click
        Try
            Response.Redirect("WF_Menu.aspx?labelForm=Menu principale ")
        Catch ex As Exception
            ModalPopup.Show("Errore", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
    End Sub
End Class