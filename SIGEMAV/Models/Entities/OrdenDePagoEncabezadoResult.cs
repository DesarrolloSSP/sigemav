namespace SIGEMAV.Models.Entities
{
    public class OrdenDePagoEncabezadoResult
    {
        public string NumeroOficio { get; set; } = string.Empty;
        public int SolicitudMantenimientoId { get; set; }
        public string? RazonSocial { get; set; }
        public string? Clabe { get; set; }
        public string? NoDsp { get; set; }
        public string? ClaveProyecto { get; set; }
        public string? AreaNombre { get; set; }
        public string? Contrato { get; set; }

    }


}
