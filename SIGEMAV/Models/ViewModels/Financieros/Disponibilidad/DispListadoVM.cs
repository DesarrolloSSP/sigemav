namespace SIGEMAV.Models.ViewModels.Financieros.Disponibilidad
{
    public class DispListadoVM
    {
        public int IdDatosDsp { get; set; }

        public string NoDsp { get; set; }

        public string ObjetoGasto { get; set; }

        public string ClaveAdmin { get; set; }

        public string Proyecto { get; set; }

        public string Area { get; set; }

        public decimal Importe { get; set; }

        public decimal Disponible { get; set; }

        public string RutaArchivo { get; set; }

        public bool Activo { get; set; }
    }
}
