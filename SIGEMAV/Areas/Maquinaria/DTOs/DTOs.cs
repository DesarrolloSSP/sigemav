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
            public string Folio { get; set; }
                = string.Empty;

            public string NumeroOficio { get; set; }
                = string.Empty;

            public DateTime FechaSolicitud { get; set; }

            public int AreaSolicitanteId { get; set; }

            public int UnidadVehicularId { get; set; }

            public int? TallerId { get; set; }

            public int TipoMantenimientoId { get; set; }

            public int KilometrajeActual { get; set; }

            public string MotivoSolicitud { get; set; }
                = string.Empty;

            public string? DiagnosticoInicial { get; set; }

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

            public ResultadoValidacionHistoricaDto?
                ValidacionHistorica
            {
                get;
                set;
            }
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
        }




        public class RevisionSolicitudDto
        {
            public int SolicitudMantenimientoId { get; set; }

            public bool Aprobada { get; set; }

            public string Observaciones { get; set; }

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
            public int TipoCatalogoServicioId { get; set; }

            public string ConceptoServicioNombre { get; set; } = string.Empty;

            public string? ConceptoServicioDescripcion { get; set; }
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

            public string MotivoSolicitud { get; set; } = null!;

            public string? DiagnosticoInicial { get; set; }

            public string? Observaciones { get; set; }

            public decimal? ImporteCotizacion { get; set; }

            public int EstatusSolicitudMantenimientoId { get; set; }

            public List<SolicitudMantenimientoDetalleCreateDto> Servicios{ get; set; } = new();
            public ResultadoValidacionHistoricaDto ValidacionHistorica { get; internal set; }
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

            public int UsuarioAutorizaId { get; set; }
        }



        public class TransferenciaUnidadRechazarDto
        {
            public int TransferenciaUnidadId { get; set; }

            public string Observaciones { get; set; } = null!;

            public int UsuarioId { get; set; }
        }





        public class TransferenciaUnidadCancelarDto
        {
            public int TransferenciaUnidadId { get; set; }

            public string MotivoCancelacion { get; set; } = null!;

            public int UsuarioId { get; set; }
        }



        public class TransferenciaUnidadEntregarDto
        {
            public int TransferenciaUnidadId { get; set; }

            public string ObservacionesEntrega { get; set; } = null!;

            public int UsuarioEntregaId { get; set; }
        }



        public class TransferenciaUnidadRecibirDto
        {
            public int TransferenciaUnidadId { get; set; }

            public string ObservacionesRecepcion { get; set; } = null!;

            public int UsuarioRecibeId { get; set; }
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

            public int UsuarioRecibeId { get; set; }

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

            public string? Diagnostico { get; set; }

            public string? TrabajoRealizado { get; set; }

            public string? Observaciones { get; set; }

            public decimal? CostoRealTotal { get; set; }

            public int? KilometrajeSalida { get; set; }

            public List<MantenimientoDetalleEditDto> Servicios { get; set; }
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
            /// <summary>
            /// Indica si la solicitud puede continuar.
            /// FALSE = bloquear creación.
            /// </summary>
            public bool EsValido { get; set; } = true;

            /// <summary>
            /// Existen solicitudes abiertas
            /// para el mismo concepto.
            /// </summary>
            public bool TieneDuplicadosActivos { get; set; }

            /// <summary>
            /// Existe recurrencia dentro
            /// del rango configurado.
            /// </summary>
            public bool TieneRecurrencia { get; set; }

            /// <summary>
            /// Mensajes que se mostrarán
            /// en UI o excepción.
            /// </summary>
            public List<string> Mensajes { get; set; }
                = new();

            /// <summary>
            /// Opcional:
            /// conceptos que generaron conflicto.
            /// </summary>
            public List<int> ConceptosConConflicto { get; set; }
                = new();

            /// <summary>
            /// Opcional:
            /// detalle técnico para UI futura.
            /// </summary>
            public List<ResultadoValidacionHistoricaDetalleDto> Detalles
            {
                get;
                set;
            } = new();
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





    }
}

