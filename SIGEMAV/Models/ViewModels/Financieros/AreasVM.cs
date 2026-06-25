namespace SIGEMAV.Models.ViewModels.Financieros
{
    public class AreasVM
    {

        public int AreaId { get; set; }

        public string AreaNombre { get; set; } = null!;

        public bool Activo { get; set; }

        public DateTime FechaCreacion { get; set; }

    }
}
