namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class DSPPartidaViewModel
    {
        public int IdObjetoGasto { get; set; }

        public string ClaveObjetoGasto { get; set; }

        public string DescripcionObjetoGasto { get; set; }

        public decimal ImporteSolicitado { get; set; }

        public decimal DisponibleDSP { get; set; }

        public bool TieneSuficiencia { get; set; }
    }
}
