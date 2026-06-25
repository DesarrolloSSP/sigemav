namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class UnidadVehicularFiltroViewModel
    {
        public string? NumeroSerie { get; set; }

        public string? NumeroEconomico { get; set; }

        public string? PlacaActual { get; set; }

        public int? MarcaId { get; set; }

        public int? ModeloId { get; set; }

        public int? AreaId { get; set; }

        public int? MunicipioId { get; set; }

        public bool? Activo { get; set; }

        // PAGINACIÓN

        public int Pagina { get; set; } = 1;

        public int RegistrosPorPagina { get; set; } = 15;
    }
}
