namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class DocumentoSolicitudViewModel
    {
        public int SolicitudMantenimientoDocumentoId { get; set; }

        public string TipoDocumento { get; set; } = null!;

        public string NombreArchivo { get; set; } = null!;

        public string RutaArchivo { get; set; } = null!;

        public DateTime FechaCarga { get; set; }
    }
}
