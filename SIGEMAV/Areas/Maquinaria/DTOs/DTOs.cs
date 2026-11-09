using System.ComponentModel.DataAnnotations;

namespace SIGEMAV.Areas.Maquinaria.DTOs
{
    public class DTOs
    {

        public class UnidadVehicularCreateDto
        {
            public string NumeroSerie { get; set; } = null!;

            public string PlacaActual { get; set; } = null!;

            public string NumeroEconomico { get; set; } = null!;

            public int MarcaId { get; set; }

            public int ModeloId { get; set; }

            public int AnioId { get; set; }

            public int ColorId { get; set; }

            public int MunicipioId { get; set; }

            public int AreaId { get; set; }

            public int TransmisionId { get; set; }
            public int CilindroId { get; set; }
        }


        public class ResultadoOperacionDto
        {
            public bool Exitoso { get; set; }

            public string Mensaje { get; set; } = string.Empty;

            public string Tipo { get; set; }
        }



        public class MantenimientoListadoDto
        {
            public int MantenimientoId { get; set; }

            public string Folio { get; set; } = string.Empty;

            public string NumeroEconomico { get; set; } = string.Empty;

            public string PlacaActual { get; set; } = string.Empty;

            public string MarcaNombre { get; set; } = string.Empty;

            public string ModeloNombre { get; set; } = string.Empty;

            public string TipoMantenimiento { get; set; } = string.Empty;

            public string TallerNombre { get; set; } = string.Empty;

            public DateTime FechaIngreso { get; set; }

            public DateTime? FechaSalida { get; set; }

            public int KilometrajeEntrada { get; set; }

            public bool Activo { get; set; }
        }





        public class MantenimientoCreateDto
        {
            public int UnidadVehicularId { get; set; }

            public int TipoMantenimientoId { get; set; }

            public int TallerId { get; set; }

            public DateTime FechaIngreso { get; set; }

            public DateTime? FechaSalida { get; set; }

            public int KilometrajeEntrada { get; set; }

            public int? KilometrajeSalida { get; set; }

            public string MotivoMantenimiento { get; set; } = string.Empty;

            public string? Diagnostico { get; set; }

            public string? TrabajoRealizado { get; set; }

            public string? Observaciones { get; set; }
        }



        public class SolicitudMantenimientoListadoDto
        {
            public int SolicitudMantenimientoId { get; set; }

            public string Folio { get; set; } = string.Empty;

            public string NumeroOficio { get; set; } = string.Empty;

            public DateTime FechaSolicitud { get; set; }

            public string AreaSolicitante { get; set; } = string.Empty;

            public string NumeroEconomico { get; set; } = string.Empty;

            public string PlacaActual { get; set; } = string.Empty;

            public string MarcaNombre { get; set; } = string.Empty;

            public string ModeloNombre { get; set; } = string.Empty;

            public string TipoMantenimiento { get; set; } = string.Empty;

            public string TallerNombre { get; set; } = string.Empty;

            public decimal? ImporteCotizacion { get; set; }

            public string Estatus { get; set; } = string.Empty;

            public string ColorEstatus { get; set; } = string.Empty;

            public bool Activo { get; set; }
        }


        public class SolicitudMantenimientoCreateDto
        {

            public int SolicitudMantenimientoId { get; set; }
            public string Folio { get; set; }
                = string.Empty;

            public string NumeroOficio { get; set; }
                = string.Empty;

            public DateTime FechaSolicitud { get; set; }

            public int AreaSolicitanteId { get; set; }

            public int UnidadVehicularId { get; set; }

            public int? TallerId { get; set; }

            public int? TipoMantenimientoId { get; set; }

            public int? KilometrajeActual { get; set; }

            //public string MotivoSolicitud { get; set; }
            //    = string.Empty;

            //public string? DiagnosticoInicial { get; set; }

            public string? Observaciones { get; set; }

            public decimal? ImporteCotizacion { get; set; }

            public int EstatusSolicitudMantenimientoId { get; set; }

            public List<IFormFile>? Archivos { get; set; }

            // DETALLE DE SERVICIOS

            public List<SolicitudMantenimientoDetalleCreateDto> Servicios
            {
                get;
                set;
            } = new();

            // RESULTADO VALIDACION HISTÓRICA

            public ResultadoValidacionHistoricaDto? ValidacionHistorica {get;set; }


            public bool Correcto { get; set; }

            public int SolicitudId { get; set; }

            public int EstadoOdometroId { get; set; }


            public int TallerConfiguracionPresupuestalId { get; set; }


        }



        public class SolicitudMantenimientoDetalleCreateDto
        {
            public int ConceptoServicioId { get; set; }
            public int SolicitudMantenimientoId { get; set; }

            public decimal Cantidad { get; set; }

            public decimal CostoEstimadoManoObra { get; set; }

            public decimal CostoEstimadoRefacciones { get; set; }

            public int? IdObjetoGastoManoObra { get; set; }

            public int? IdObjetoGastoRefacciones { get; set; }

            public string? Observaciones { get; set; }

            public byte EstadoOdometroId { get; set; }

            public int TallerConfiguracionPresupuestalId { get; set; }
        }




        public class RevisionSolicitudDto
        {
            public int SolicitudMantenimientoId { get; set; }

            public bool Aprobada { get; set; }

            public string? Observaciones { get; set; }

            public int? TipoRechazoSolicitudId { get; set; }
        }





        public class DSPSolicitudDto
        {
            public int SolicitudMantenimientoId { get; set; }

            public bool Autorizado { get; set; }

            public string Observaciones { get; set; }
        }




        public class UnidadVehicularEditDto
        {
            public int IdUnidadVehicular { get; set; }

            public string NumeroSerie { get; set; }

            public string PlacaActual { get; set; }

            public string NumeroEconomico { get; set; }

            public int IdMarca { get; set; }

            public int IdModelo { get; set; }

            public int IdAnio { get; set; }

            public int IdColor { get; set; }

            public int IdMunicipio { get; set; }

            public int IdArea { get; set; }

            public int IdTransmision { get; set; }

            public int IdCilindro { get; set; }


        }


        public class UnidadVehicularEliminarDto
        {
            public int UnidadVehicularId { get; set; }
        }





        public class TipoCatalogoServicioIndexDto
        {
            public List<TipoCatalogoServicioRowDto> Registros { get; set; } = new();

            public int PaginaActual { get; set; }

            public int TotalPaginas { get; set; }

            public int TotalRegistros { get; set; }

            public bool TieneAnterior => PaginaActual > 1;

            public bool TieneSiguiente => PaginaActual < TotalPaginas;
        }

        public class TipoCatalogoServicioRowDto
        {
            public int TipoCatalogoServicioId { get; set; }

            public string TipoCatalogoServicioNombre { get; set; } = string.Empty;

            public bool Activo { get; set; }
        }



        public class ConceptoServicioRowDto
        {
            public int ConceptoServicioId { get; set; }

            public string TipoCatalogoServicioNombre { get; set; } = null!;

            public string ConceptoServicioNombre { get; set; } = null!;

            public string ConceptoServicioDescripcion { get; set; } = null!;

            public bool Activo { get; set; }

            public DateTime FechaCreacion { get; set; }
        }


        public class ConceptoServicioIndexDto
        {
            public List<ConceptoServicioRowDto> Registros { get; set; } = new();

            public int PaginaActual { get; set; }

            public int TotalPaginas { get; set; }

            public int TotalRegistros { get; set; }

            public bool TieneAnterior => PaginaActual > 1;

            public bool TieneSiguiente => PaginaActual < TotalPaginas;
        }

        //public class ConceptoServicioCostoRowDto
        //{
        //    public int ConceptoServicioCostoId { get; set; }

        //    public string ConceptoServicioNombre { get; set; } = null!;

        //    public decimal CostoManoObra { get; set; }

        //    public decimal CostoRefacciones { get; set; }

        //    public DateOnly FechaInicioVigencia { get; set; }

        //    public DateOnly FechaFinVigencia { get; set; }

        //    public string MarcaNombre { get; set; } = string.Empty;

        //    public string ModeloNombre { get; set; } = string.Empty;

        //    public string AnioNombre { get; set; } = string.Empty;

        //    public string TransmisionNombre { get; set; } = string.Empty;

        //    public string CilindroNombre { get; set; } = string.Empty;

        //    public string? Observaciones { get; set; }

        //    public bool Activo { get; set; }

        //    public DateTime FechaCreacion { get; set; }
        //}

        public class ConceptoServicioCostoIndexDto
        {
            public List<ConceptoServicioCostoRowDto> Registros { get; set; } = new();

            public int PaginaActual { get; set; }

            public int TotalPaginas { get; set; }

            public int TotalRegistros { get; set; }

            public bool TieneAnterior => PaginaActual > 1;

            public bool TieneSiguiente => PaginaActual < TotalPaginas;


        }


        public class TipoCatalogoServicioCreateDto
        {
            public string TipoCatalogoServicioNombre { get; set; } = string.Empty;
        }


        public class TipoCatalogoServicioEditDto
        {
            public int TipoCatalogoServicioId { get; set; }

            public string TipoCatalogoServicioNombre { get; set; } = string.Empty;
        }




        public class ConceptoServicioCreateDto
        {
            public int ConceptoServicioId { get; set; }

            public string Concepto { get; set; }

            public decimal CostoManoObra { get; set; }

            public decimal CostoRefacciones { get; set; }
            public int TipoCatalogoServicioId { get; internal set; }
            public string ConceptoServicioNombre { get; internal set; }
            public string? ConceptoServicioDescripcion { get; internal set; }
        }








        public class ConceptoServicioEditDto
        {
            public int ConceptoServicioId { get; set; }

            public int TipoCatalogoServicioId { get; set; }

            public string ConceptoServicioNombre { get; set; }

            public string? ConceptoServicioDescripcion { get; set; }
        }



        public class ConceptoServicioCostoCreateDto
        {
            public int ConceptoServicioId { get; set; }

            public int MarcaId { get; set; }

            public int ModeloId { get; set; }

            public int AnioId { get; set; }

            public int TransmisionId { get; set; }

            public int CilindroId { get; set; }

            public decimal CostoManoObra { get; set; }

            public decimal CostoRefacciones { get; set; }

            public string? Observaciones { get; set; }

            public DateOnly FechaInicioVigencia { get; set; }

            public DateOnly? FechaFinVigencia { get; set; }
        }





        public class ConceptoServicioCostoEditDto
        {
            public int ConceptoServicioCostoId { get; set; }

            public int ConceptoServicioId { get; set; }

            public int MarcaId { get; set; }

            public int ModeloId { get; set; }

            public int AnioId { get; set; }

            public int AnioInicio { get; set; }

            public int AnioFin { get; set; }

            public int TransmisionId { get; set; }

            public int CilindroId { get; set; }

            public decimal CostoManoObra { get; set; }

            public decimal CostoRefacciones { get; set; }

            public string? Observaciones { get; set; }

            public DateOnly FechaInicioVigencia { get; set; }

            public DateOnly? FechaFinVigencia { get; set; }
        }



        public class ConceptoServicioCostoRowDto
        {
            public int ConceptoServicioCostoId { get; set; }

            public string ConceptoServicioNombre { get; set; } = string.Empty;

            public decimal CostoManoObra { get; set; }

            public decimal CostoRefacciones { get; set; }

            public string MarcaNombre { get; set; } = string.Empty;

            public string ModeloNombre { get; set; } = string.Empty;

            public string AnioNombre { get; set; } = string.Empty;

            public string TransmisionNombre { get; set; } = string.Empty;

            public string CilindroNombre { get; set; } = string.Empty;

            public DateOnly FechaInicioVigencia { get; set; }

            public DateOnly? FechaFinVigencia { get; set; }

            public string? Observaciones { get; set; }

            public DateTime FechaCreacion { get; set; }

            public bool Activo { get; set; }
        }




        public class SolicitudMantenimientoEditDto
        {
            public int SolicitudMantenimientoId { get; set; }

            public string Folio { get; set; } = null!;

            public string NumeroOficio { get; set; } = null!;

            public DateTime FechaSolicitud { get; set; }

            public int AreaSolicitanteId { get; set; }

            public int UnidadVehicularId { get; set; }

            public int TallerId { get; set; }

            public int TipoMantenimientoId { get; set; }

            public decimal KilometrajeActual { get; set; }

            public byte EstadoOdometroId { get; set; }

            

            //public string MotivoSolicitud { get; set; } = null!;

            //public string? DiagnosticoInicial { get; set; }

            public string? Observaciones { get; set; }

            public decimal? ImporteCotizacion { get; set; }

            public int EstatusSolicitudMantenimientoId { get; set; }

            public List<SolicitudMantenimientoDetalleCreateDto> Servicios { get; set; } = new();
            public ResultadoValidacionHistoricaDto ValidacionHistorica { get; internal set; }

            public int TallerConfiguracionPresupuestalId { get; set; }
        }





        public class Select2Dto
        {
            public int id { get; set; }

            public string text { get; set; } = string.Empty;
        }




        public class SolicitudMantenimientoInfoDto
        {
            public int SolicitudMantenimientoId { get; set; }

            public string Folio { get; set; }

            public int AreaSolicitanteId { get; set; }

            public int UnidadVehicularId { get; set; }

            public int EstatusSolicitudMantenimientoId { get; set; }

            public DateTime FechaCreacion { get; set; }

            public string UnidadVehicularTexto { get; set; }
            public int TipoCatalogoServicioId { get; internal set; }
        }



        public class TransferenciaUnidadCreateDto
        {
            public int UnidadVehicularId { get; set; }

            public int AreaOrigenId { get; set; }

            public int AreaDestinoId { get; set; }

            public string MotivoTransferencia { get; set; } = null!;

            public string? Observaciones { get; set; }

            public DateTime FechaTransferencia { get; set; }
        }





        public class TransferenciaUnidadEditDto
        {
            public int TransferenciaUnidadId { get; set; }

            public int AreaDestinoId { get; set; }

            public string MotivoTransferencia { get; set; } = null!;

            public string? Observaciones { get; set; }
        }


        public class TransferenciaUnidadDetalleDto
        {
            public int TransferenciaUnidadId { get; set; }

            public string Folio { get; set; } = null!;

            public string UnidadVehicular { get; set; } = null!;

            public string AreaOrigen { get; set; } = null!;

            public string AreaDestino { get; set; } = null!;

            public string Estatus { get; set; } = null!;

            public DateTime FechaTransferencia { get; set; }

            public string MotivoTransferencia { get; set; } = null!;

            public string? Observaciones { get; set; }
        }





        public class TransferenciaUnidadAutorizarDto
        {
            public int TransferenciaUnidadId { get; set; }

            public string ComentariosAutorizacion { get; set; } = null!;

            public string UsuarioAutorizaId { get; set; }
        }



        public class TransferenciaUnidadRechazarDto
        {
            public int TransferenciaUnidadId { get; set; }

            public string Observaciones { get; set; } = null!;

            public string UsuarioId { get; set; }
        }





        public class TransferenciaUnidadCancelarDto
        {
            public int TransferenciaUnidadId { get; set; }

            public string MotivoCancelacion { get; set; } = null!;

            public string UsuarioId { get; set; }
        }



        public class TransferenciaUnidadEntregarDto
        {
            public int TransferenciaUnidadId { get; set; }

            public string ObservacionesEntrega { get; set; } = null!;

            public string UsuarioEntregaId { get; set; }
        }



        public class TransferenciaUnidadRecibirDto
        {
            public int TransferenciaUnidadId { get; set; }

            public string ObservacionesRecepcion { get; set; } = null!;

            public string UsuarioRecibeId { get; set; }
        }



        public class SolicitudMantenimientoServicioCreateDto
        {
            public int ConceptoServicioId { get; set; }

            public decimal Cantidad { get; set; }

            public decimal CostoUnitarioManoObra { get; set; }

            public decimal CostoUnitarioRefacciones { get; set; }

            public decimal CostoEstimadoManoObra { get; set; }

            public decimal CostoEstimadoRefacciones { get; set; }

            public string? Observaciones { get; set; }

            public List<SolicitudMantenimientoDetalleCreateDto> Servicios { get; set; } = new List<SolicitudMantenimientoDetalleCreateDto>();
        }



        public class DSPDto
        {
            public int SolicitudMantenimientoId { get; set; }

            public int IdDispPresupuestal { get; set; }

            public string? Observaciones { get; set; }
            public List<DSPPartidaDto> Partidas { get; set; } = new();
        }



        public class EntregaTransferenciaDto
        {
            public int TransferenciaUnidadId { get; set; }

            public string ObservacionesEntrega { get; set; }
        }


        public class TransferenciaUnidadRecepcionarDto
        {
            public int TransferenciaUnidadId { get; set; }

            public string UsuarioRecibeId { get; set; }

            public string? ObservacionesRecepcion { get; set; }
        }




        public class DSPPartidaDto
        {
            public int IdObjetoGasto { get; set; }

            public decimal ImporteSolicitado { get; set; }

            public string? ClaveObjetoGasto { get; set; }
        }





        public class ConceptoServicioCostoDto
        {
            public decimal CostoManoObra { get; set; }

            public decimal CostoRefacciones { get; set; }

            public int? IdObjetoGastoManoObra { get; set; }

            public int? IdObjetoGastoRefacciones { get; set; }


            public string? ClaveObjetoGastoManoObra { get; set; }

            public string? ClaveObjetoGastoRefacciones { get; set; }
        }





       




        public class MantenimientoEditDto
        {
            public int MantenimientoId { get; set; }

            public int SolicitudMantenimientoId { get; set; }


            public DateTime? FechaInicioTrabajo { get; set; }

            public DateTime? FechaFinTrabajo { get; set; }


            public int? KilometrajeSalida { get; set; }


            public string? Diagnostico { get; set; }

            public string? TrabajoRealizado { get; set; }

            public string? Observaciones { get; set; }


            public List<MantenimientoDetalleDto> Servicios { get; set; }
                = new();
        }





        public class MantenimientoDetalleEditDto
        {
            public int? MantenimientoDetalleId { get; set; }

            public int ConceptoServicioId { get; set; }

            public int Cantidad { get; set; }

            public decimal CostoUnitarioManoObra { get; set; }

            public decimal CostoUnitarioRefacciones { get; set; }

            public string? Observaciones { get; set; }

            public int? IdObjetoGastoManoObra { get; set; }

            public int? IdObjetoGastoRefacciones { get; set; }
        }


        public class ValidacionServicioHistoricoDto
        {
            public bool TieneDuplicadosActivos { get; set; }

            public bool TieneRecurrencia { get; set; }

            public List<string> Mensajes { get; set; }
                = new();

            public bool EsValido { get; set; }
        }








        public class ConfiguracionValidacionTecnicaDto
        {
            public int ConceptoServicioId { get; set; }

            public int TipoCatalogoServicioId { get; set; }

            public bool ValidarSolicitudesAbiertas { get; set; }

            public List<int> EstatusPermitidos { get; set; }

            public bool ValidarRecurrencia { get; set; }

            public int DiasRecurrencia { get; set; }

            public int ConfiguracionValidacionTecnicaId { get; set; }

        }




        public class RevisionHistoricoDto
        {
            public string Folio { get; set; }

            public DateTime Fecha { get; set; }

            public string Concepto { get; set; }

            public decimal Costo { get; set; }

            public int Kilometraje { get; set; }

            public int DiasDesdeUltimo { get; set; }

            public bool EsRecurrencia { get; set; }
        }

        public class RevisionResumenHistoricoDto
        {
            public int TotalServicios { get; set; }

            public DateTime? UltimaFecha { get; set; }

            public decimal CostoAcumulado { get; set; }

            public int ServiciosSolicitadosConHistorial { get; set; }

            public int TotalRecurrenciasSolicitadas { get; set; }

            public List<string> ConceptosConAntecedente { get; set; } = new();

            // NUEVOS

            public int DiasDesdeUltimoMantenimiento { get; set; }

            public int KilometrosDesdeUltimoMantenimiento { get; set; }

            public decimal CostoHistoricoServiciosSolicitados { get; set; }

            public decimal CostoPromedioServicio { get; set; }

            public int VecesServicioSolicitado { get; set; }

            public int ScoreRiesgo { get; set; }

            public string NivelRiesgo { get; set; } = string.Empty;

            public string RecomendacionTecnica { get; set; } = string.Empty;

            // Servicios solicitados realizados durante los últimos 30 días
            public int ServiciosRecientes { get; set; }

            // Costo histórico únicamente de los conceptos solicitados
            public decimal CostoHistoricoConceptosSolicitados { get; set; }



        }





        public class ConfiguracionValidacionTecnicaListDto
        {
            public int ConfiguracionValidacionTecnicaId { get; set; }

            public int TipoCatalogoServicioId { get; set; }
            public string TipoCatalogoServicioNombre { get; set; }

            public int ConceptoServicioId { get; set; }
            public string ConceptoServicioNombre { get; set; }

            public bool ValidarSolicitudesAbiertas { get; set; }

            public bool ValidarRecurrencia { get; set; }

            public int? DiasRecurrencia { get; set; }

            public bool Activo { get; set; }

            // Opcional pero útil para UI (si luego haces badges o reglas avanzadas)
            public string EstatusSolicitudMantenimientoNombre { get; set; }
        }





        public class ConfiguracionValidacionTecnicaCreateDto
        {
            public int TipoCatalogoServicioId { get; set; }

            public int ConceptoServicioId { get; set; }

            public bool ValidarSolicitudesAbiertas { get; set; }

            public List<int> EstatusPermitidos { get; set; } = new();

            public bool ValidarRecurrencia { get; set; }

            public int? DiasRecurrencia { get; set; }

            public bool Activo { get; set; }
        }



        public class ResultadoValidacionHistoricaDto
        {
            public bool EsValido { get; set; }

            public bool TieneDuplicadosActivos { get; set; }

            public bool TieneRecurrencia { get; set; }

            // NUEVOS

            public int ServiciosHistoricos { get; set; }

            public int VecesEjecutado { get; set; }

            public DateTime? UltimaEjecucion { get; set; }

            public int DiasDesdeUltimo { get; set; }

            public decimal CostoHistorico { get; set; }

            public decimal CostoUltimoServicio { get; set; }

            public int KilometrajeUltimo { get; set; }

            public int KilometrosRecorridos { get; set; }

            public string NivelRiesgo { get; set; }

            public List<string> ConceptosConAntecedente { get; set; } = new();

            public List<string> Mensajes { get; set; } = new();
         
            public List<int> ConceptosConConflicto { get; set; }= new();

            public List<ResultadoValidacionHistoricaDetalleDto> Detalles{get;set;} = new();


            public int TotalSolicitudesAbiertas { get; set; }

            public int TotalServiciosHistoricos { get; set; }

            public int TotalRecurrencias { get; set; }

            public int PuntajeRiesgo { get; set; }


            public List<string> FactoresRiesgo { get; set; }


        }






        public class ResultadoValidacionHistoricaDetalleDto
        {
            public int ConceptoServicioId { get; set; }

            public string ConceptoServicioNombre { get; set; }

            public bool TieneSolicitudAbierta { get; set; }

            public bool TieneRecurrencia { get; set; }

            public int? DiasRecurrencia { get; set; }

            public string Mensaje { get; set; }
        }


        public class FacturaMantenimientoCreateDto
        {
            public int SolicitudMantenimientoId { get; set; }
            public int MantenimientoId { get; set; }

            public int TallerId { get; set; }
            public int UnidadVehicularId { get; set; }

            public DateTime FechaFactura { get; set; }

            public decimal TotalFactura { get; set; }

            public IFormFile Xml { get; set; }
            public IFormFile Pdf { get; set; }
            public DateTime FechaRecepcion { get; internal set; }
            public string Observaciones { get; internal set; }
        }



        public class ResultadoValidacionFacturaDto
        {
            public bool EsValida { get; set; }

            public bool XmlDuplicado { get; set; }

            public bool DiferenciaImporte { get; set; }

            public decimal TotalMantenimiento { get; set; }

            public decimal TotalFactura { get; set; }

            public List<string> Mensajes { get; set; } = new();
        }





        public class FacturaMantenimientoDto
        {
            public int MantenimientoId { get; set; }
            public int TallerId { get; set; }

            public string UUID { get; set; }

            public string Serie { get; set; }
            public string Folio { get; set; }

            public DateTime FechaFactura { get; set; }

            public string RFCEmisor { get; set; }
            public string? RFCReceptor { get; set; }

            public string? RazonSocial { get; set; }

            public string? MetodoPago { get; set; }
            public string? FormaPago { get; set; }

            public string? UsoCFDI { get; set; }
            public string? Moneda { get; set; }
            public string? TipoComprobante { get; set; }

            public decimal Subtotal { get; set; }
            public decimal IVA { get; set; }
            public decimal Retencion { get; set; }
            public decimal Total { get; set; }

            public string? Observaciones { get; set; }
            public int FacturaMantenimientoId { get; internal set; }
            public DateTime FechaRecepcion { get; internal set; }
            public object FechaTimbrado { get; internal set; }
            public object Detalles { get; internal set; }
        }



        public class FacturaMantenimientoDetalleDto
        {
            public int FacturaMantenimientoId { get; set; }
            public int MantenimientoId { get; set; }
            public int TallerId { get; set; }

            public string Estatus { get; set; }

            public string UUID { get; set; }
            public string Serie { get; set; }
            public string Folio { get; set; }

            public DateOnly FechaFactura { get; set; }
            public DateTime FechaRecepcion { get; set; }

            public string RFCEmisor { get; set; }
            public string RFCReceptor { get; set; }
            public string RazonSocial { get; set; }

            public string MetodoPago { get; set; }
            public string FormaPago { get; set; }
            public string UsoCFDI { get; set; }

            public decimal Subtotal { get; set; }
            public decimal IVA { get; set; }
            public decimal Retencion { get; set; }
            public decimal Total { get; set; }

            public string Observaciones { get; set; }

            // RELACIONES UI
            public string TallerNombre { get; set; }
            public string FolioMantenimiento { get; set; }
            public string UnidadVehicular { get; set; }
        }






        public class FacturaMantenimientoIndexDto
        {
            public int FacturaMantenimientoId { get; set; }
            public string UUID { get; set; }

            public string Folio { get; set; }
            public string Serie { get; set; }

            public string TallerNombre { get; set; }
            public string UnidadVehicular { get; set; }

            public DateOnly FechaFactura { get; set; }
            public DateTime FechaRecepcion { get; set; }

            public decimal Total { get; set; }

            public string Estatus { get; set; }
            public string ColorEstatus { get; set; }
        }




        public class FacturaParseadaDto
        {
            public string UUID { get; set; }

            public string Serie { get; set; }

            public string Folio { get; set; }

            public DateTime FechaFactura { get; set; }

            public DateTime FechaTimbrado { get; set; }

            public string RFCEmisor { get; set; }

            public string RFCReceptor { get; set; }

            public string RazonSocial { get; set; }

            public string MetodoPago { get; set; }

            public string FormaPago { get; set; }

            public string UsoCFDI { get; set; }

            public string Moneda { get; set; }

            public string TipoComprobante { get; set; }

            public string LugarExpedicion { get; set; }

            public string NoCertificado { get; set; }

            public string NoCertificadoSAT { get; set; }

            public decimal Subtotal { get; set; }

            public decimal IVA { get; set; }

            public decimal Total { get; set; }

            public List<FacturaConceptoDto>
            Conceptos
            =
            new();
        }




        public class FacturaConceptoDto
        {
            public string ClaveProdServ { get; set; }

            public string ClaveUnidad { get; set; }

            public string Descripcion { get; set; }

            public decimal Cantidad { get; set; }

            public decimal ValorUnitario { get; set; }

            public decimal Importe { get; set; }

            public decimal IVA { get; set; }

            public decimal Total =>
            Importe + IVA;
        }



        public class FacturaDetalleDto
        {
            public int? ConceptoServicioId { get; set; }
            public int? Cantidad { get; set; }
            public decimal? Subtotal { get; set; }
            public decimal? IVA { get; set; }
            public decimal? Total { get; set; }
        }


        public class FacturaXmlDto
        {
            public string UUID { get; set; }

            public string Serie { get; set; }

            public string Folio { get; set; }

            public DateTime FechaFactura { get; set; }

            public DateTime? FechaTimbrado { get; set; }

            public string RFCEmisor { get; set; }

            public string RFCReceptor { get; set; }

            public string RazonSocial { get; set; }

            public string MetodoPago { get; set; }

            public string FormaPago { get; set; }

            public string UsoCFDI { get; set; }

            public string Moneda { get; set; }

            public string TipoComprobante { get; set; }

            public decimal Subtotal { get; set; }

            public decimal IVA { get; set; }

            public decimal Retencion { get; set; }

            public decimal Total { get; set; }

            public string LugarExpedicion { get; set; }

            public string NoCertificado { get; set; }

            public string NoCertificadoSAT { get; set; }

            public List<FacturaConceptoXmlDto> Conceptos { get; set; } = [];
        }

        public class FacturaConceptoXmlDto
        {
            public string Descripcion { get; set; }

            public decimal Cantidad { get; set; }

            public decimal ValorUnitario { get; set; }

            public decimal Importe { get; set; }

            public decimal IVA { get; set; }
        }



        public class ResultadoValidacionPresupuestalDto
        {
            public bool TieneWarning { get; set; }

            public bool RequiereIncremento { get; set; }

            public decimal DisponiblePosterior { get; set; }

            public List<string> Mensajes { get; set; }
                = new();
        }


        public class ProcesarDSPResultadoDto
        {
            public bool Exitoso { get; set; }
            public List<string> Warnings { get; set; } = new();

        }



        public class ValidacionSatDto
        {
            public bool ConsultaExitosa { get; set; }

            public bool EsValida { get; set; }

            public string Estado { get; set; } = "";

            public string CodigoEstatus { get; set; } = "";

            public string Cancelable { get; set; } = "";

            public string EstatusCancelacion { get; set; } = "";

            public DateTime FechaConsulta { get; set; }

            public string Mensaje { get; set; } = "";
        }



        public class MantenimientoDetalleDto
        {
            public int? MantenimientoDetalleId { get; set; }

            public int ConceptoServicioId { get; set; }

            public int Cantidad { get; set; }


            public decimal CostoUnitarioManoObra { get; set; }

            public decimal CostoUnitarioRefacciones { get; set; }


            public int? IdObjetoGastoManoObra { get; set; }

            public int? IdObjetoGastoRefacciones { get; set; }


            public string? Observaciones { get; set; }
        }




        public class EvaluacionConceptoDto
        {
            public int ConceptoServicioId { get; set; }

            public string Concepto { get; set; } = "";

            // Configuración

            public bool ValidarSolicitudesAbiertas { get; set; }

            public bool ValidarRecurrencia { get; set; }

            public int DiasRecurrencia { get; set; }

            // Solicitudes

            public int SolicitudesAbiertas { get; set; }

            public List<string> FoliosAbiertos { get; set; } = new();

            // Historial

            public int TotalHistorico { get; set; }

            public DateTime? UltimaFecha { get; set; }

            public int DiasDesdeUltimo { get; set; }

            public int KilometrajeUltimo { get; set; }

            public int KilometrosRecorridos { get; set; }

            public decimal CostoHistorico { get; set; }

            public decimal CostoPromedio { get; set; }

            public bool EsRecurrenciaDias { get; set; }

            public bool EsRecurrenciaFrecuencia { get; set; }

            // Resultado

            public int PuntajeRiesgo { get; set; }

            public string NivelRiesgo { get; set; } = "";

            public List<string> Observaciones { get; set; } = new();
        }



        public class ValidacionSolicitudActivaDto
        {
            public int SolicitudMantenimientoId { get; set; }

            public string Folio { get; set; }

            public int EstatusSolicitudMantenimientoId { get; set; }

            public int ConceptoServicioId { get; set; }

            public int TipoCatalogoServicioId { get; set; }

            public string ConceptoServicioNombre { get; set; }
        }

        public class ValidacionHistorialServicioDto
        {
            public int ConceptoServicioId { get; set; }

            public int TipoCatalogoServicioId { get; set; }

            public string ConceptoServicioNombre { get; set; }

            public DateTime Fecha { get; set; }
        }



        public class ResultadoCreacionSolicitudDto
        {
            public int SolicitudMantenimientoId { get; set; }
            public bool Correcto { get; set; }

            public int SolicitudId { get; set; }

            public string Folio { get; set; }

            public ResultadoValidacionHistoricaDto ValidacionHistorica { get; set; }
        }




        public class TallerDto
        {
            public int TallerId { get; set; }

            public string RazonSocial { get; set; } = string.Empty;

            public string Rfc { get; set; } = string.Empty;

            public int RecursoId { get; set; }

            public string RecursoNombre { get; set; } = string.Empty;

            public decimal MontoMinimo { get; set; }

            public string Contrato { get; set; } = string.Empty;

            public int BancoId { get; set; }

            public string BancoNombre { get; set; } = string.Empty;

            public int FianzaId { get; set; }

            public string FianzaNombre { get; set; } = string.Empty;

            public string? Clabe { get; set; }

            public string Ubicacion { get; set; } = string.Empty;

            public bool Activo { get; set; }

            public decimal? MontoMaximo { get; set; }

            public string Fianza { get; set; } = string.Empty;
        }
        }




        public class PaginacionDto<T>
        {
            public List<T> Registros { get; set; } = new();

            public int PaginaActual { get; set; }

            public int RegistrosPorPagina { get; set; }

            public int TotalRegistros { get; set; }

            public int TotalPaginas { get; set; }
        }





        public class RecursoDto
        {
            public int RecursoId { get; set; }

            public string Nombre { get; set; } = string.Empty;
        }




        public class BancoDto
        {
            public int BancoId { get; set; }

            public string Nombre { get; set; } = string.Empty;
        }


        public class FianzaDto
        {
            public int FianzaId { get; set; }

            public string Nombre { get; set; } = string.Empty;
        }



        public class ObjetivoMantenimientoDto
        {
            public int ObjetivoMantenimientoId { get; set; }

            public string Nombre { get; set; } = string.Empty;
        }





    public class TallerCreateDto
    {
        public string RazonSocial { get; set; } = string.Empty;

        public string Rfc { get; set; } = string.Empty;

        public int RecursoId { get; set; }

        public string Contrato { get; set; } = string.Empty;

        public int BancoId { get; set; }

        public string Fianza { get; set; } = string.Empty;

        public string Clabe { get; set; } = string.Empty;

        public int ObjetivoMantenimientoId { get; set; }

        public string Ubicacion { get; set; } = string.Empty;

        public decimal MontoMinimo { get; set; }

        public decimal MontoMaximo { get; set; }


        // =====================================================
        // ALERTA PREVENTIVA
        // =====================================================

        public decimal PorcentajeAlerta { get; set; }

        public decimal MontoAlerta { get; set; }


        // =====================================================
        // ALERTA CRÍTICA
        // =====================================================

        public decimal PorcentajeCritico { get; set; }

        public decimal MontoCritico { get; set; }


        public bool Activo { get; set; }
    }


    public class ResultadoOperacion
    {
        public bool Exito { get; set; }

        public string Mensaje { get; set; } = string.Empty;
    }






    public class TallerEditDto
    {
        // =====================================================
        // IDENTIFICADOR
        // =====================================================

        public int TallerId { get; set; }


        // =====================================================
        // DATOS DEL TALLER
        // =====================================================

        public string RazonSocial { get; set; }
            = string.Empty;

        public string Rfc { get; set; }
            = string.Empty;

        public int BancoId { get; set; }

        public string Fianza { get; set; }
            = string.Empty;

        public string Clabe { get; set; }
            = string.Empty;

        public int ObjetivoMantenimientoId { get; set; }

        public string Ubicacion { get; set; }
            = string.Empty;

        public bool Activo { get; set; }
    }




    public class TallerEditarDto
    {
        public int TallerId { get; set; }

        public string RazonSocial { get; set; } = string.Empty;

        public string Rfc { get; set; } = string.Empty;

        public int RecursoId { get; set; }

        public string RecursoNombre { get; set; }

        public string Contrato { get; set; } = string.Empty;

        public int BancoId { get; set; }

        public string? Fianza { get; set; }

        public string? Clabe { get; set; }

        public int? ObjetivoMantenimientoId { get; set; }

        public string Ubicacion { get; set; } = string.Empty;

        public decimal MontoMinimo { get; set; }

        public decimal? MontoMaximo { get; set; }

        public decimal? PorcentajeAlerta { get; set; }

        public decimal? MontoAlerta { get; set; }

        public decimal? PorcentajeCritico { get; set; }

        public decimal? MontoCritico { get; set; }

        public bool Activo { get; set; }
    }





    public class TallerConfiguracionPresupuestalCreateDto
    {
        public string Contrato { get; set; } = string.Empty;

        public int RecursoId { get; set; }

        public decimal MontoMinimo { get; set; }

        public decimal MontoMaximo { get; set; }

        public decimal? PorcentajeAlerta { get; set; }

        public decimal? PorcentajeCritico { get; set; }
    }




    public class TallerConfiguracionPresupuestalEditDto
    {
        public int TallerConfiguracionPresupuestalId { get; set; }

        public decimal MontoMinimo { get; set; }

        public decimal MontoMaximo { get; set; }

        public decimal? PorcentajeAlerta { get; set; }

        public decimal? PorcentajeCritico { get; set; }

        public bool Activo { get; set; }
    }




    public class TallerConfiguracionPresupuestalDto
    {
        public int TallerConfiguracionPresupuestalId { get; set; }

        public string Contrato { get; set; } = string.Empty;

        public int RecursoId { get; set; }

        public string RecursoNombre { get; set; } = string.Empty;

        public decimal MontoMinimo { get; set; }

        public decimal MontoMaximo { get; set; }

        public decimal? PorcentajeAlerta { get; set; }

        public decimal? MontoAlerta { get; set; }

        public decimal? PorcentajeCritico { get; set; }

        public decimal? MontoCritico { get; set; }

        public bool Activo { get; set; }

        public int CantidadTalleres { get; set; }

        public int CantidadTalleresActivos { get; set; }
    }






    public class TallerControlPresupuestalDto
    {
        public int IdControlTaller { get; set; }

        public int TallerConfiguracionPresupuestalId { get; set; }

        public string Contrato { get; set; } = string.Empty;

        public int RecursoId { get; set; }

        public string RecursoNombre { get; set; } = string.Empty;

        public short Ejercicio { get; set; }

        public decimal PresupuestoAsignado { get; set; }

        public decimal PresupuestoEjercido { get; set; }

        public decimal PresupuestoComprometido { get; set; }

        public decimal PresupuestoDisponible { get; set; }

        public DateOnly FechaInicio { get; set; }

        public DateOnly FechaFin { get; set; }

        public bool Activo { get; set; }
    }




    public class TallerControlPresupuestalCreateDto
    {
        public int TallerConfiguracionPresupuestalId { get; set; }

        public short Ejercicio { get; set; }

        public decimal PresupuestoAsignado { get; set; }

        public DateOnly FechaInicio { get; set; }

        public DateOnly FechaFin { get; set; }
    }




    public class TallerControlPresupuestalEditDto
    {
        public int IdControlTaller { get; set; }

        public decimal PresupuestoAsignado { get; set; }

        public DateOnly FechaInicio { get; set; }

        public DateOnly FechaFin { get; set; }

        public bool Activo { get; set; }
    }



    public class TallerConfiguracionPresupuestalSelectDto
    {
        public int TallerConfiguracionPresupuestalId { get; set; }

        public string Contrato { get; set; } = string.Empty;

        public string RecursoNombre { get; set; } = string.Empty;

        public decimal MontoMaximo { get; set; }

        public string Descripcion
        {
            get
            {
                return $"{Contrato} - {RecursoNombre}";
            }
        }
    }





    public class TallerConfiguracionCreateDto
    {
        public int TallerId { get; set; }

        public int TallerConfiguracionPresupuestalId { get; set; }

        public bool Activo { get; set; }
    }




    public class TallerConfiguracionEditDto
    {
        public int TallerId { get; set; }

        public int TallerConfiguracionPresupuestalId { get; set; }

        public bool Activo { get; set; }


        public int TallerConfiguracionPresupuestalIdActual { get; set; }

        public int TallerConfiguracionPresupuestalIdNueva { get; set; }

        public int ConfiguracionPresupuestalOriginalId { get; set; }

    }





    public class TallerConfiguracionDto
    {
        public int TallerId { get; set; }

        public int TallerConfiguracionPresupuestalId { get; set; }

        public string TallerNombre { get; set; } = string.Empty;

        public string RFC { get; set; } = string.Empty;

        public string Ubicacion { get; set; } = string.Empty;

        public string Contrato { get; set; } = string.Empty;

        public string RecursoNombre { get; set; } = string.Empty;

        public decimal MontoMaximo { get; set; }

        public bool Activo { get; set; }

        public DateTime FechaCreacion { get; set; }
    }


    public class FirmanteDto
    {
        public int FirmanteId { get; set; }

        [Required(ErrorMessage = "La clave del puesto es obligatoria.")]
        public string ClavePuesto { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre completo es obligatorio.")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "El cargo es obligatorio.")]
        public string Cargo { get; set; } = string.Empty;

        // Opcional: NULL para firmantes globales
        public int? AreaId { get; set; }

        public string? AreaNombre { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; }
    }










}

