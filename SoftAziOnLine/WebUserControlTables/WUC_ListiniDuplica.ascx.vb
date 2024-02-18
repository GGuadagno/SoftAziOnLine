Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.DataBaseUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.WebFormUtility
Partial Public Class WUC_ListiniDuplica
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDSCategArt.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
    End Sub

    Private Sub BtnTestInsArticoli_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BtnTestInsArticoli.Click
        ' ''WFPAnagrProvvInsert1.WucElement = Me
        ' ''Session(FINESTRA_ANAGR_PROVV_APERTA) = True
        ' ''WFPAnagrProvvInsert1.Show()
    End Sub
End Class