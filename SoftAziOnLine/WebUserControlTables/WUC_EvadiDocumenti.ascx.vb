Imports System.Data.SqlClient
Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.DataBaseUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.WebFormUtility
Partial Public Class WUC_EvadiDocumenti
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDSDocumentiT.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDSDocumentiD.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        If Not IsPostBack Then
            btnAltroOrdine.Text = "Seleziona  altro ordine"
            btnEvadiParz.Text = "Evadi parzialmente"
            btnEvadiSing.Text = "Evadi singola riga ordine"
            btnEvadiAlles.Text = "Evadi righe ALLESTITE"
            btnEvadiDett.Text = "Evadi tutti i dettagli"
            BtnConfermaEvas.Text = "Conferma evasione"
            BtnCreaDDT.Text = "Crea DDT"
            BtnEliminaRiga.Text = "Elimina riga"
            BtnEliminaDett.Text = "Elimina tutti i dettagli"
            Call ImpostaGriglia_GridViewTestata()
            Call ImpostaGriglia_GridViewDettOrdine()
            Call ImpostaGriglia_GridViewDettDoc()
        End If
    End Sub

    Private Sub ImpostaGriglia_GridViewTestata()

        Dim nameColumn0 As New BoundField

        GridViewTestata.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridViewTestata.Attributes.Add("style", "table-layout:fixed")

        nameColumn0.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        nameColumn0.HeaderText = "Numero"
        nameColumn0.DataField = "Numero"
        'nameColumn0.DataFormatString = "{00:g}"
        nameColumn0.ItemStyle.HorizontalAlign = HorizontalAlign.Right
        nameColumn0.ItemStyle.Wrap = True
        GridViewTestata.Columns.Add(nameColumn0)

        Dim nameColumn1 As New BoundField
        nameColumn1.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        nameColumn1.HeaderText = "Data ord."
        nameColumn1.DataField = "Data_Doc"
        nameColumn1.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        nameColumn1.ItemStyle.Wrap = True
        GridViewTestata.Columns.Add(nameColumn1)


        Dim nameColumn2 As New BoundField
        nameColumn2.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        'nameColumn2.HeaderStyle.BackColor = Drawing.Color.FromName(ColoreSfondoIntes)
        nameColumn2.HeaderText = "Data cons."
        nameColumn2.DataField = "DataOraConsegna"
        nameColumn2.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        nameColumn2.ItemStyle.Wrap = True   'per andare a capo
        nameColumn2.ReadOnly = True ''esempio, è da rimuovere...
        GridViewTestata.Columns.Add(nameColumn2)

        Dim nameColumn3 As New BoundField
        nameColumn3.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        'nameColumn2.HeaderStyle.BackColor = Drawing.Color.FromName(ColoreSfondoIntes)
        nameColumn3.HeaderText = "Causale"
        nameColumn3.DataField = ""
        nameColumn3.ItemStyle.Width = 300 'non funziona!!! trasformare in template
        nameColumn3.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        nameColumn3.ItemStyle.Wrap = True
        GridViewTestata.Columns.Add(nameColumn3)

        Dim nameColumn4 As New BoundField
        nameColumn4.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        'nameColumn2.HeaderStyle.BackColor = Drawing.Color.FromName(ColoreSfondoIntes)
        nameColumn4.HeaderText = "Pagamento"
        nameColumn4.DataField = ""
        nameColumn4.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        nameColumn4.ItemStyle.Wrap = True
        GridViewTestata.Columns.Add(nameColumn4)

        Dim nameColumn5 As New BoundField
        nameColumn5.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        'nameColumn2.HeaderStyle.BackColor = Drawing.Color.FromName(ColoreSfondoIntes)
        nameColumn5.HeaderText = "Riferimento"
        nameColumn5.DataField = "Riferimento"
        nameColumn5.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        nameColumn5.ItemStyle.Wrap = True
        GridViewTestata.Columns.Add(nameColumn5)

        'Dim nameColumn6 As New BoundField
        'nameColumn6.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        ''nameColumn2.HeaderStyle.BackColor = Drawing.Color.FromName(ColoreSfondoIntes)
        'nameColumn6.HeaderText = "Quantità residua"
        'nameColumn6.DataField = "Qta_Residua"
        'nameColumn6.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        'nameColumn6.ItemStyle.Wrap = True
        'GridViewTestata.Columns.Add(nameColumn6)

        'Dim nameColumn7 As New BoundField
        'nameColumn7.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        ''nameColumn2.HeaderStyle.BackColor = Drawing.Color.FromName(ColoreSfondoIntes)
        'nameColumn7.HeaderText = "Aliq. IVA"
        'nameColumn7.DataField = "Cod_Iva"
        'nameColumn7.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        'nameColumn7.ItemStyle.Wrap = True
        'GridViewTestata.Columns.Add(nameColumn7)

        'Dim nameColumn8 As New BoundField
        'nameColumn8.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        ''nameColumn2.HeaderStyle.BackColor = Drawing.Color.FromName(ColoreSfondoIntes)
        'nameColumn8.HeaderText = "Prezzo"
        'nameColumn8.DataField = "Prezzo"
        'nameColumn8.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        'nameColumn8.ItemStyle.Wrap = True
        'GridViewTestata.Columns.Add(nameColumn8)

        'Dim nameColumn9 As New BoundField
        'nameColumn9.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        ''nameColumn2.HeaderStyle.BackColor = Drawing.Color.FromName(ColoreSfondoIntes)
        'nameColumn9.HeaderText = "Sconto 1"
        'nameColumn9.DataField = "Sconto_1"
        'nameColumn9.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        'nameColumn9.ItemStyle.Wrap = True
        'GridViewTestata.Columns.Add(nameColumn9)

        'Dim nameColumn10 As New BoundField
        'nameColumn10.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        ''nameColumn2.HeaderStyle.BackColor = Drawing.Color.FromName(ColoreSfondoIntes)
        'nameColumn10.HeaderText = "Sconto 2"
        'nameColumn10.DataField = "Sconto_2"
        'nameColumn10.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        'nameColumn10.ItemStyle.Wrap = True
        'GridViewTestata.Columns.Add(nameColumn10)

        'Dim nameColumn11 As New BoundField
        'nameColumn11.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        ''nameColumn2.HeaderStyle.BackColor = Drawing.Color.FromName(ColoreSfondoIntes)
        'nameColumn11.HeaderText = "Sconto Reale"
        'nameColumn11.DataField = "ScontoReale"
        'nameColumn11.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        'nameColumn11.ItemStyle.Wrap = True
        'GridViewTestata.Columns.Add(nameColumn11)

        'Dim nameColumn12 As New BoundField
        'nameColumn12.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        ''nameColumn2.HeaderStyle.BackColor = Drawing.Color.FromName(ColoreSfondoIntes)
        'nameColumn12.HeaderText = "Sconto valore"
        'nameColumn12.DataField = "ScontoValore"
        'nameColumn12.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        'nameColumn12.ItemStyle.Wrap = True
        'GridViewTestata.Columns.Add(nameColumn12)



    End Sub

    Private Sub ImpostaGriglia_GridViewDettOrdine()
        Dim nameColumn0 As New BoundField

        GridViewDettOrdine.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridViewDettOrdine.Attributes.Add("style", "table-layout:fixed")

        nameColumn0.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        nameColumn0.HeaderText = "Codice articolo"
        nameColumn0.DataField = "Cod_Articolo"
        nameColumn0.ItemStyle.HorizontalAlign = HorizontalAlign.Right
        nameColumn0.ItemStyle.Wrap = True
        GridViewDettOrdine.Columns.Add(nameColumn0)

        Dim nameColumn1 As New BoundField
        nameColumn1.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        nameColumn1.HeaderText = "Descrizione"
        nameColumn1.DataField = "Descrizione"
        nameColumn1.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        nameColumn1.ItemStyle.Width = 300 'non funziona!!! trasformare in template
        nameColumn1.ItemStyle.Wrap = True
        GridViewDettOrdine.Columns.Add(nameColumn1)

        Dim nameColumn2 As New BoundField
        nameColumn2.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        nameColumn2.HeaderText = "Qta. ordinata"
        nameColumn2.DataField = "Qta_Ordinata"
        nameColumn2.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        nameColumn2.ItemStyle.Wrap = True
        GridViewDettOrdine.Columns.Add(nameColumn2)


        Dim nameColumn3 As New BoundField
        nameColumn3.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        nameColumn3.HeaderText = "Qta. evasa"
        nameColumn3.DataField = "Qta_Evasa"
        nameColumn3.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        nameColumn3.ItemStyle.Wrap = True   'per andare a capo
        nameColumn3.ReadOnly = True ''esempio, è da rimuovere...
        GridViewDettOrdine.Columns.Add(nameColumn3)


        Dim nameColumn4 As New BoundField
        nameColumn4.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        nameColumn4.HeaderText = "Qta. da evadere"
        nameColumn4.DataField = ""
        nameColumn4.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        nameColumn4.ItemStyle.Wrap = True
        GridViewDettOrdine.Columns.Add(nameColumn4)


        Dim nameColumn5 As New BoundField
        nameColumn5.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        nameColumn5.HeaderText = "Qta. residua"
        nameColumn5.DataField = "Qta_Residua"
        nameColumn5.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        nameColumn5.ItemStyle.Wrap = True
        GridViewDettOrdine.Columns.Add(nameColumn5)


        Dim nameColumn6 As New BoundField
        nameColumn6.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        nameColumn6.HeaderText = "Q.ta allestita"
        nameColumn6.DataField = ""
        nameColumn6.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        nameColumn6.ItemStyle.Wrap = True
        GridViewDettOrdine.Columns.Add(nameColumn6)

    End Sub

    Private Sub ImpostaGriglia_GridViewDettDoc()
        Dim nameColumn0 As New BoundField

        GridViewDettDoc.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridViewDettDoc.Attributes.Add("style", "table-layout:fixed")

        nameColumn0.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        nameColumn0.HeaderText = "Num. ord."
        nameColumn0.DataField = ""
        nameColumn0.ItemStyle.HorizontalAlign = HorizontalAlign.Right
        nameColumn0.ItemStyle.Wrap = True
        GridViewDettDoc.Columns.Add(nameColumn0)

        Dim nameColumn1 As New BoundField
        nameColumn1.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        nameColumn1.HeaderText = "Riga"
        nameColumn1.DataField = "Riga"
        nameColumn1.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        nameColumn1.ItemStyle.Wrap = True
        GridViewDettDoc.Columns.Add(nameColumn1)

        Dim nameColumn2 As New BoundField
        nameColumn2.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        nameColumn2.HeaderText = "Cod. articolo"
        nameColumn2.DataField = "Cod_Articolo"
        nameColumn2.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        nameColumn2.ItemStyle.Wrap = True
        GridViewDettDoc.Columns.Add(nameColumn2)

        Dim nameColumn3 As New BoundField
        nameColumn3.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        nameColumn3.HeaderText = "Descrizione"
        nameColumn3.DataField = "Descrizione"
        nameColumn3.ItemStyle.Width = 300
        nameColumn3.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        nameColumn3.ItemStyle.Wrap = True   'per andare a capo
        nameColumn3.ReadOnly = True ''esempio, è da rimuovere...
        GridViewDettDoc.Columns.Add(nameColumn3)

        Dim nameColumn4 As New BoundField
        nameColumn4.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        nameColumn4.HeaderText = "UM"
        nameColumn4.DataField = "Um"
        nameColumn4.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        nameColumn4.ItemStyle.Wrap = True
        GridViewDettDoc.Columns.Add(nameColumn4)

        Dim nameColumn5 As New BoundField
        nameColumn5.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        nameColumn5.HeaderText = "Quantità"
        nameColumn5.DataField = "Qta_Ordinata"
        nameColumn5.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        nameColumn5.ItemStyle.Wrap = True
        GridViewDettDoc.Columns.Add(nameColumn5)


        Dim nameColumn6 As New BoundField
        nameColumn6.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        nameColumn6.HeaderText = "IVA"
        nameColumn6.DataField = "Cod_Iva"
        nameColumn6.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        nameColumn6.ItemStyle.Wrap = True
        GridViewDettDoc.Columns.Add(nameColumn6)

        Dim nameColumn7 As New BoundField
        nameColumn7.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        nameColumn7.HeaderText = "Prezzo"
        nameColumn7.DataField = "Prezzo"
        nameColumn7.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        nameColumn7.ItemStyle.Wrap = True
        GridViewDettDoc.Columns.Add(nameColumn7)

        Dim nameColumn8 As New BoundField
        nameColumn8.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        nameColumn8.HeaderText = "Sc. 1"
        nameColumn8.DataField = "Sconto_1"
        nameColumn8.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        nameColumn8.ItemStyle.Wrap = True
        GridViewDettDoc.Columns.Add(nameColumn8)

        Dim nameColumn9 As New BoundField
        nameColumn9.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        nameColumn9.HeaderText = "Sc. 2"
        nameColumn9.DataField = "Sconto_2"
        nameColumn9.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        nameColumn9.ItemStyle.Wrap = True
        GridViewDettDoc.Columns.Add(nameColumn9)
    End Sub

    Private Sub GridViewDettDoc_RowDeleting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewDeleteEventArgs) Handles GridViewDettDoc.RowDeleting

    End Sub

    Private Sub GridViewDettOrdine_RowEditing(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewEditEventArgs) Handles GridViewDettOrdine.RowEditing

    End Sub

    Private Sub GridViewDettOrdine_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewDettOrdine.SelectedIndexChanged

    End Sub
End Class