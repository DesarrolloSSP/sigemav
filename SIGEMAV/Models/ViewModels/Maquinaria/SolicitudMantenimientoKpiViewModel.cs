namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class SolicitudMantenimientoKpiViewModel
    {
        /// <summary>
        /// Total de solicitudes registradas.
        /// </summary>
        public int TotalSolicitudes { get; set; }

        /// <summary>
        /// Solicitudes pendientes de DSP.
        /// </summary>
        public int PendientesDSP { get; set; }

        /// <summary>
        /// Solicitudes en revisión técnica.
        /// </summary>
        public int EnRevision { get; set; }

        /// <summary>
        /// Solicitudes autorizadas que ya ingresaron al taller.
        /// </summary>
        public int EnMantenimiento { get; set; }

        /// <summary>
        /// Solicitudes finalizadas.
        /// </summary>
        public int Finalizadas { get; set; }

        /// <summary>
        /// Total cotizado.
        /// </summary>
        public decimal ImporteCotizado { get; set; }
    }
}
