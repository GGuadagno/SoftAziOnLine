Option Strict On
Option Explicit On 

Imports It.SoftAzi.Integration.Dao.SQLDao
Imports It.SoftAzi.SystemFramework
Namespace It.SoftAzi.Integration.Dao
    Public MustInherit Class FactoryDAO
        Public Shared Function getFactoryDAO() As FactoryDAO
            Dim typeOfFactory As String = ApplicationConfiguration.TypeFactory
            If typeOfFactory = "SQL" Then
                Return New SQLFactoryDAO
            Else
                Throw New Exception("Factory non gestita")
            End If
        End Function
        Public MustOverride Function getDataSource() As DataSource
        Public MustOverride Function getOperatoriByName() As OperatoriDAO
        Public MustOverride Function getDitteByCodice() As DitteDAO
        Public MustOverride Function getDitte() As DitteDAO
        Public MustOverride Function getAnaMagByCodice() As AnaMagDAO
        Public MustOverride Function getAnaMag() As AnaMagDAO
        Public MustOverride Function delAnaMagByCodice() As Boolean
        Public MustOverride Function InsertUpdateAnaMag() As Boolean
        Public MustOverride Function CIAnaMagByCodice() As Boolean
        Public MustOverride Function getFornitoreByCodice() As FornitoreDAO
        Public MustOverride Function getFornitori() As FornitoreDAO
        Public MustOverride Function getAnaMagDesByCodiceArticolo() As AnaMagDesDAO
        Public MustOverride Function getAnaMagCTVByCodiceArticolo() As AnaMagCTVDAO
        Public MustOverride Function getDistBaseByCodiceArticolo() As DistBaseDAO
        Public MustOverride Function InsertAnaMagDes() As Boolean
        Public MustOverride Function InsertAnaMagCTV() As Boolean
        Public MustOverride Function InsertDistBase() As Boolean
        Public MustOverride Function delAnaMagDesByCodiceArticolo() As Boolean
        Public MustOverride Function delAnaMagCTVByCodiceArticolo() As Boolean
        Public MustOverride Function delDistBaseByCodiceArticolo() As Boolean
        Public MustOverride Function getFornSecondariByCodiceArticolo() As FornSecondariDAO
        Public MustOverride Function delFornSecByCodiceArticolo() As Boolean
        Public MustOverride Function InsertFornitoriSec() As Boolean
        Public MustOverride Function getListVenTByCodice() As ListVenTDAO
        Public MustOverride Function getListVenT() As ListVenTDAO
        Public MustOverride Function getListVenDByCodLisCodArt() As ListVenDDAO
        Public MustOverride Function InsertUpdateListVenD() As Boolean
        Public MustOverride Function getUltimiPrezziAcquistoByCodiceArticolo() As UltimiPrezziAcquistoDAO
        Public MustOverride Function InsertUltimiPrezziAcquisto() As Boolean
        Public MustOverride Function getParametriGeneraliAzi() As ParametriGeneraliAziDAO
        Public MustOverride Function UpdOperatoriDataOraUltAccesso() As Boolean
        Public MustOverride Function DelOperatoreConnesso() As Boolean
        Public MustOverride Function InsertiArticoliOu() As Boolean
        Public MustOverride Function getClienti() As ClientiDAO
        Public MustOverride Function getClientiByCodice() As ClientiDAO
        Public MustOverride Function InsertUpdateCliente() As Boolean
        Public MustOverride Function delClientiByCodice() As Boolean
        Public MustOverride Function CIClienteByCodice() As Boolean
        Public MustOverride Function getNazioni() As NazioniDAO
        Public MustOverride Function getAliquoteIva() As AliquoteIvaDAO
        Public MustOverride Function getPagamenti() As PagamentiDAO
        Public MustOverride Function getProvince() As ProvinceDAO
        Public MustOverride Function getZone() As ZoneDAO
        Public MustOverride Function getVettori() As VettoriDAO
        Public MustOverride Function getCategorie() As CategorieDAO
        Public MustOverride Function getPianoDeiConti() As PianoDeiContiDAO
        Public MustOverride Function getAgenti() As AgentiDAO
        Public MustOverride Function getProgressivi() As ProgressiviDAO
        Public MustOverride Function getElencoBanche() As BancheDAO
        Public MustOverride Function getElencoFilialiByABI() As FilialiDAO
        Public MustOverride Function getTipoFattByCodice() As TipoFattDAO 'ALBERTO 19/12/2012
    End Class
End Namespace

