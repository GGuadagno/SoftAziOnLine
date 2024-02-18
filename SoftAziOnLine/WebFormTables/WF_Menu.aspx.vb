Imports SoftAziOnLine.Def
Imports SoftAziOnLine.WebFormUtility
Partial Public Class WF_Menu
    Inherits System.Web.UI.Page

    Private Sub WF_Menu_Error(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Error
        Session(CSTSALVADB) = False
        Session(F_ANAGRAGENTI_APERTA) = False
        Session(F_BANCHEIBAN_APERTA) = False
        Session(F_ANAGRCAPIGR_APERTA) = False
        Session(F_ANAGRCATEGORIEART_APERTA) = False
        Session(F_ANAGRCATEGORIE_APERTA) = False
        Session(F_ANAGRLINEEART_APERTA) = False
        Session(F_ANAGRTIPOCODART_APERTA) = False
        Session(F_ANAGRMISURE_APERTA) = False
        Session(F_ANAGRVETTORI_APERTA) = False
        Session(F_ANAGRZONE_APERTA) = False
        Session(F_PAGAMENTI_APERTA) = False
        Session(F_SEL_ARTICOLO_APERTA) = False
        Session(F_ANAGR_PROVV_APERTA) = False
        Session(F_ANAGRCLIFOR_APERTA) = False
        Session(F_FORNSEC_APERTA) = False
        Session(F_SCELTALISTINI_APERTA) = False
        Session(F_SCELTAMOVMAG_APERTA) = False
        Session(F_EVASIONEPARZ_APERTA) = False
        Session(F_ELENCO_CLIFORN_APERTA) = False
        Session(LISTINI_DA_AGG) = False
        Session(OSCLI_F_ELENCO_CLI1_APERTA) = False
        Session(OSCLI_F_ELENCO_CLI2_APERTA) = False
        Session(OSCLI_F_ELENCO_FORN1_APERTA) = False
        Session(OSCLI_F_ELENCO_FORN2_APERTA) = False
        Session(OSART_F_ELENCO_ART1_APERTA) = False
        Session(OSART_F_ELENCO_ART2_APERTA) = False
        Session(F_SCELTASPED_APERTA) = False
        Session(F_GESTIONETESTIEMAIL_APERTA) = False
    End Sub
    
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session(CSTVISUALMENU) = SWSI
        Session(CALLGESTIONE) = SWNO
    End Sub

End Class