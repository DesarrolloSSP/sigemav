namespace SIGEMAV.Models.ViewModels.Maquinaria
{

    public class HistorialTecnicoUnidadViewModel
    {
        public DateTime Fecha { get; set; }

        public string Folio { get; set; } = string.Empty;

        public string Concepto { get; set; } = string.Empty;

        public int ConceptoServicioId { get; set; }

        public int Kilometraje { get; set; }

        public decimal Costo { get; set; }

        public int DiasDesdeUltimo { get; set; }

        // Ya existía
        public bool EsRecurrencia { get; set; }

        // NUEVOS
        public bool EsConceptoSolicitado { get; set; }

        public int NumeroEjecuciones { get; set; }
    }


}
