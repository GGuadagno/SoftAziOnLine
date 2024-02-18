Option Strict On
Option Explicit On

Imports It.SoftAzi.Integration.Dao
Namespace It.SoftAzi.Integration.Dao.SQLDao
    Public Class SQLFactoryDAO
        Inherits FactoryDAO

        Public Overrides Function getDataSource() As DataSource
            Return DataSource.getDataSource
        End Function

        Public Overrides Function getOperatoriByName() As OperatoriDAO
            Return New SQLOperatoriDAO
        End Function

        Public Overrides Function getDitteByCodice() As DitteDAO
            Return New SQLDitteDAO
        End Function

        Public Overrides Function getDitte() As DitteDAO
            Return New SQLDitteDAO
        End Function

        Public Overrides Function getAnaMagByCodice() As AnaMagDAO
            Return New SQLAnaMagDAO
        End Function

        Public Overrides Function getAnaMag() As AnaMagDAO
            Return New SQLAnaMagDAO
        End Function

        Public Overrides Function delAnaMagByCodice() As Boolean
            Return True
        End Function

        Public Overrides Function InsertUpdateAnaMag() As Boolean
            Return True
        End Function

        Public Overrides Function CIAnaMagByCodice() As Boolean
            Return True
        End Function

        Public Overrides Function getFornitoreByCodice() As FornitoreDAO
            Return New SQLFornitoreDAO
        End Function

        Public Overrides Function getFornitori() As FornitoreDAO
            Return New SQLFornitoreDAO
        End Function

        Public Overrides Function getAnaMagDesByCodiceArticolo() As AnaMagDesDAO
            Return New SQLAnaMagDesDAO
        End Function

        Public Overrides Function getAnaMagCTVByCodiceArticolo() As AnaMagCTVDAO
            Return New SQLAnaMagCTVDAO
        End Function

        Public Overrides Function InsertAnaMagDes() As Boolean
            Return True
        End Function

        Public Overrides Function InsertAnaMagCTV() As Boolean
            Return True
        End Function

        Public Overrides Function delAnaMagDesByCodiceArticolo() As Boolean
            Return True
        End Function

        Public Overrides Function delAnaMagCTVByCodiceArticolo() As Boolean
            Return True
        End Function

        Public Overrides Function getFornSecondariByCodiceArticolo() As FornSecondariDAO
            Return New SQLFornSecondariDAO
        End Function

        Public Overrides Function delFornSecByCodiceArticolo() As Boolean
            Return True
        End Function

        Public Overrides Function InsertFornitoriSec() As Boolean
            Return True
        End Function

        Public Overrides Function getListVenTByCodice() As ListVenTDAO
            Return New SQLListVenTDAO
        End Function

        Public Overrides Function getListVenT() As ListVenTDAO
            Return New SQLListVenTDAO
        End Function

        Public Overrides Function getListVenDByCodLisCodArt() As ListVenDDAO
            Return New SQLListVenDDAO
        End Function

        Public Overrides Function InsertUpdateListVenD() As Boolean
            Return True
        End Function

        Public Overrides Function getUltimiPrezziAcquistoByCodiceArticolo() As UltimiPrezziAcquistoDAO
            Return New SQLUltimiPrezziAcquistoDAO
        End Function

        Public Overrides Function InsertUltimiPrezziAcquisto() As Boolean
            Return True
        End Function

        Public Overrides Function getParametriGeneraliAzi() As ParametriGeneraliAziDAO
            Return New SQLParametriGeneraliAziDAO
        End Function

        Public Overrides Function UpdOperatoriDataOraUltAccesso() As Boolean
            Return True
        End Function

        Public Overrides Function DelOperatoreConnesso() As Boolean
            Return True
        End Function

        Public Overrides Function InsertiArticoliOu() As Boolean
            Return True
        End Function

        Public Overrides Function getClienti() As ClientiDAO
            Return New SQLClientiDAO
        End Function

        Public Overrides Function getClientiByCodice() As ClientiDAO
            Return New SQLClientiDAO
        End Function

        Public Overrides Function InsertUpdateCliente() As Boolean
            Return True
        End Function

        Public Overrides Function delClientiByCodice() As Boolean
            Return True
        End Function

        Public Overrides Function CIClienteByCodice() As Boolean
            Return True
        End Function

        Public Overrides Function getNazioni() As NazioniDAO
            Return New SQLNazioniDAO
        End Function

        Public Overrides Function getAliquoteIva() As AliquoteIvaDAO
            Return New SQLAliquoteIvaDAO
        End Function

        Public Overrides Function getProvince() As ProvinceDAO
            Return New SQLProvinceDAO
        End Function

        Public Overrides Function getPagamenti() As PagamentiDAO
            Return New SQLPagamentiDAO
        End Function

        Public Overrides Function getZone() As ZoneDAO
            Return New SQLZoneDAO
        End Function

        Public Overrides Function getVettori() As VettoriDAO
            Return New SQLVettoriDAO
        End Function

        Public Overrides Function getCategorie() As CategorieDAO
            Return New SQLCategorieDAO
        End Function

        Public Overrides Function getPianoDeiConti() As PianoDeiContiDAO
            Return New SQLPianoDeiContiDAO
        End Function

        Public Overrides Function getAgenti() As AgentiDAO
            Return New SQLAgentiDAO
        End Function

        Public Overrides Function getProgressivi() As ProgressiviDAO
            Return New SQLProgressiviDAO
        End Function

        Public Overrides Function getElencoBanche() As BancheDAO
            Return New SQLBancheDAO
        End Function

        Public Overrides Function getElencoFilialiByABI() As FilialiDAO
            Return New SQLFilialiDAO
        End Function

        Public Overrides Function getTipoFattByCodice() As TipoFattDAO
            Return New SQLTipoFattDAO
        End Function

        Public Overrides Function getDistBaseByCodiceArticolo() As DistBaseDAO
            Return New SQLDistBaseDAO
        End Function

        Public Overrides Function InsertDistBase() As Boolean
            Return True
        End Function

        Public Overrides Function delDistBaseByCodiceArticolo() As Boolean
            Return True
        End Function
    End Class

End Namespace
