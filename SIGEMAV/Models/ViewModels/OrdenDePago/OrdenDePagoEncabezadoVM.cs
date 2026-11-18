namespace SIGEMAV.Models.ViewModels.OrdenDePago
{
    public class OrdenDePagoEncabezadoVM
    {
        public string NumeroOficio { get; set; } = string.Empty;
        public string? NombreSolicitante { get; set; }
        public string? RazonSocial { get; set; }
        public string? ClabeInterbancaria { get; set; }
        public string? AreaSolicitante { get; set; }
        public string? ClaveProyecto { get; set; }
        public string? NoDsp { get; set; }
        public string? Contrato { get; set; }
        public decimal TotalGeneral { get; set; }


    }


}
