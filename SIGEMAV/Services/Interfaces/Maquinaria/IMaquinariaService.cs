using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SIGEMAV.Models.ViewModels.Maquinaria;
using static SIGEMAV.Areas.Maquinaria.DTOs.DTOs;

namespace SIGEMAV.Services.Interfaces.Maquinaria
{
    public interface IMaquinariaService
    {
        //Task<PaginacionViewModel<UnidadVehicularViewModel>> ObtenerVehiculos(int pagina = 1, int registrosPorPagina = 10);

        Task<PaginacionViewModel<UnidadVehicularViewModel>> ObtenerVehiculos(int pagina = 1, int registrosPorPagina = 10,
          string? busqueda = null,
          int? marcaId = null,
          int? modeloId = null,
          int? areaId = null,
          int? municipioId = null
      );


        //vehiculo
        Task<UnidadVehicularCreateViewModel> ObtenerVehiculoEditar(int id);
        Task<List<SelectListItem>> ObtenerModelosPorMarca(int marcaId);
        Task<ResultadoOperacionDto> CrearVehiculo(UnidadVehicularCreateDto dto);


        Task<ResultadoOperacionDto> EditarVehiculo(UnidadVehicularEditDto dto);
        Task<List<Select2Dto>> BuscarUnidadesVehiculares(string term);

        Task<PaginacionViewModel<MantenimientoViewModel>> ObtenerMantenimientos(int pagina = 1, int registrosPorPagina = 10);

        Task<List<SelectListItem>> ObtenerUnidadesVehicularesSelectList();

        //catalogos maquinaria
        Task<List<SelectListItem>> ObtenerTiposMantenimiento();
        Task<List<SelectListItem>> ObtenerTalleres();

        Task<List<SelectListItem>> ObtenerAreas();
        Task<List<SelectListItem>> ObtenerMarcas();

        Task<List<SelectListItem>> ObtenerAnios();

        Task<List<SelectListItem>> ObtenerTransmision();

        Task<List<SelectListItem>> ObtenerCilindro();
        Task<TipoCatalogoServicioIndexDto> ObtenerIndexTipoCatalogoServicioAsync(int pagina);

        Task<TipoCatalogoServicioEditViewModel> ObtenerTipoCatalogoServicioEditarAsync(int id);

        Task EditarTipoCatalogoServicioAsync(TipoCatalogoServicioEditDto dto);

        Task<TipoCatalogoServicioDeleteViewModel> ObtenerTipoCatalogoEliminarAsync(int id);

        Task EliminarTipoCatalogoAsync(int id);
        Task<ConceptoServicioIndexDto> ObtenerIndexConceptoServicioAsync(int pagina);

        Task<ConceptoServicioCostoIndexDto> ObtenerIndexConceptoServicioCostoAsync(int pagina);

        Task CrearTipoCatalogoServicioAsync(TipoCatalogoServicioCreateDto dto);






        // Solicitudes de Mantenimiento
        Task EditarSolicitudMantenimiento(SolicitudMantenimientoEditDto dto, IEnumerable<IFormFile>? archivos);

        Task<SolicitudMantenimientoInfoDto> ObtenerSolicitudMantenimientoPorId(int id);

        Task<UnidadVehicularDetailsViewModel> ObtenerUnidadVehicularDetails(int id);

        
        Task<PaginacionViewModel<SolicitudMantenimientoViewModel>> ObtenerSolicitudesMantenimiento(int pagina = 1, int registrosPorPagina = 10);
        Task CrearSolicitudMantenimiento(SolicitudMantenimientoCreateDto dto, IEnumerable<IFormFile> archivos);

        Task<List<SelectListItem>> ObtenerEstatusSolicitudMantenimiento();

        Task<SolicitudMantenimientoDetallePageViewModel> ObtenerDetalleSolicitud(int solicitudId);

        Task<int?> ObtenerAreaPorUnidad(int unidadVehicularId);

        Task AutorizarSolicitudMantenimiento(int solicitudId);

        Task<string> ObtenerTextoUnidadVehicular(int unidadVehicularId);







        // Conceptos de Servicio

        Task<ConceptoServicioCostoDto> ObtenerCostoConceptoCreate(int unidadVehicularId, int conceptoServicioId);
        Task<List<SelectListItem>> ObtenerConceptosServicio(int tipoCatalogoServicioId);

        Task<ConceptoServicioCostoDto> ObtenerCostoConcepto(int solicitudId, int conceptoServicioId);

        Task AgregarConceptoSolicitud(SolicitudMantenimientoDetalleCreateDto dto);

        Task<List<int>> ObtenerConceptosYaRegistrados(int solicitudId);


        // Revision de Solicitud
        Task<RevisionSolicitudViewModel> ObtenerRevisionSolicitud(int solicitudId);
        Task RevisarSolicitud(RevisionSolicitudDto dto);


        // Rechazo de Solicitud
        Task<List<SelectListItem>> ObtenerTiposRechazo();


        Task<UnidadVehicularEliminarViewModel?> ObtenerEliminarAsync(int id);

        Task<bool> EliminarAsync(UnidadVehicularEliminarDto dto);


        

        // OBJETO DEL GASTO
        Task<IEnumerable<SelectListItem>> ObtenerObjetosGasto();

       



        // Conceptos de servicio combo
        Task<List<SelectListItem>> ObtenerTiposCatalogoServicio();

        Task CrearConceptoServicioAsync(ConceptoServicioCreateDto dto);

        Task<List<SelectListItem>> ObtenerConceptosServicioFiltrados(int unidadVehicularId);


        Task<ConceptoServicioEditViewModel> ObtenerConceptoServicioEdit(int id);

        Task<ResultadoOperacionDto> EditarConceptoServicio(ConceptoServicioEditDto dto);

        Task<List<SelectListItem>> ObtenerTiposCatalogoServicioSelect();

        Task<ConceptoServicioDeleteViewModel> ObtenerConceptoServicioDelete(int id);

        Task<ResultadoOperacionDto> EliminarConceptoServicio(int id);


        Task<ResultadoOperacionDto> CrearConceptoServicioCosto(ConceptoServicioCostoCreateDto dto);

        Task<List<SelectListItem>> ObtenerConceptosServicioSelect();

        Task<ConceptoServicioCostoEditViewModel> ObtenerConceptoServicioCostoEdit(int id);
        Task EditarConceptoServicioCosto(ConceptoServicioCostoEditDto dto);

        Task<ConceptoServicioCostoDeleteViewModel> ObtenerConceptoServicioCostoDelete(int id);

        Task EliminarConceptoServicioCosto(int id);

        Task<SolicitudMantenimientoEditViewModel> ObtenerSolicitudMantenimientoEdit(int id);




        //transferencia de unidad vehicular

        Task<List<TransferenciaUnidadViewModel>> ObtenerTransferencias();

        Task<TransferenciaUnidadCreateViewModel> ObtenerTransferenciaCreate();

        Task<TransferenciaUnidadDetalleViewModel> ObtenerTransferenciaDetalle(int id);

        Task CrearTransferenciaUnidad(TransferenciaUnidadCreateDto dto, IFormFile? documento);




        // Acciones de transferencia
        Task AutorizarTransferencia(TransferenciaUnidadAutorizarDto dto, IFormFile? documento);

        Task RechazarTransferencia(TransferenciaUnidadRechazarDto dto);

        Task RecibirTransferencia(TransferenciaUnidadRecibirDto dto, IFormFile? documento);

        Task CancelarTransferencia(TransferenciaUnidadCancelarDto dto);

        Task EntregarTransferencia(TransferenciaUnidadEntregarDto dto, IFormFile documento);

        Task<TransferenciaUnidadEntregarViewModel> ObtenerTransferenciaParaEntrega(int transferenciaId);

        Task<TransferenciaUnidadRecepcionarViewModel> ObtenerRecepcionTransferencia(int transferenciaUnidadId);

        Task RecepcionarTransferencia(TransferenciaUnidadRecepcionarDto dto, IFormFile documento);





        // DSP
        Task<DSPViewModel> ObtenerDSP(int solicitudId);
        Task<IEnumerable<SelectListItem>> ObtenerDSPDisponibles(int areaId);

        Task ProcesarDSP(DSPDto dto);



        //mantenimiento
        Task IngresarSolicitudATaller(int solicitudId);
        Task<MantenimientoCreateViewModel?>ObtenerSolicitudParaIngresoTaller(int solicitudId);

        Task<MantenimientoEditViewModel> ObtenerMantenimientoEditar(int solicitudId);

        Task EditarMantenimiento(MantenimientoEditDto dto);

        Task FinalizarMantenimiento(int mantenimientoId);


        // Validacion de servicios historicos
        Task<List<ConfiguracionValidacionTecnicaDto>>ObtenerConfiguracionValidacion(List<int> conceptos);
        Task<ResultadoOperacionDto> GuardarConfiguracion(ConfiguracionValidacionTecnicaViewModel vm);

        Task<List<SelectListItem>> ObtenerConceptosPorTipo(int tipoCatalogoServicioId);

        Task<List<ConfiguracionValidacionTecnicaListDto>> ObtenerConfiguracionesValidacion();

        Task<ResultadoOperacionDto> CrearConfiguracion(ConfiguracionValidacionTecnicaCreateDto dto);

        Task<ResultadoValidacionHistoricaDto> ValidarServiciosHistoricos(int unidadVehicularId, List<int> conceptos, int tipoMantenimientoId);


    }
}
