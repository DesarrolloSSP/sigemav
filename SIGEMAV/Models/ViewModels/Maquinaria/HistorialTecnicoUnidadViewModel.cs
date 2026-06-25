namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class HistorialTecnicoUnidadViewModel
    {
        public DateTime Fecha { get; set; }
        public string Folio { get; set; }
        public string Concepto { get; set; }
        public decimal Kilometraje { get; set; }
        public decimal Costo { get; set; }
        public int DiasDesdeUltimo { get; set; }
        public bool EsRecurrencia { get; set; }
    }
}
