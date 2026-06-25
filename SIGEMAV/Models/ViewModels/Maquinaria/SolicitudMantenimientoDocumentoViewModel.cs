namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class SolicitudMantenimientoDocumentoViewModel
    {
        public int SolicitudMantenimientoDocumentoId { get; set; }

        public string TipoDocumento { get; set; }

        public string NombreArchivo { get; set; }

        public DateTime FechaCarga { get; set; }
    }
}
