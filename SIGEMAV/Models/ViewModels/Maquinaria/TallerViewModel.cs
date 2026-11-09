namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class TallerViewModel
    {
        public int TallerId { get; set; }

        public string RazonSocial { get; set; } = string.Empty;

        public string Rfc { get; set; } = string.Empty;

        public int RecursoId { get; set; }

        public string RecursoNombre { get; set; } = string.Empty;

        public decimal MontoMinimo { get; set; }

        public string Contrato { get; set; } = string.Empty;

        public int BancoId { get; set; }

        public string BancoNombre { get; set; } = string.Empty;

        public int FianzaId { get; set; }

        public string FianzaNombre { get; set; } = string.Empty;

        public string Fianza { get; set; } = string.Empty;

        public string? Clabe { get; set; }

        public string Ubicacion { get; set; } = string.Empty;

        public bool Activo { get; set; }

        public decimal? MontoMaximo { get; set; }
    }




    public class TallerListadoViewModel
    {
        public string? Busqueda { get; set; }

        public bool? Activo { get; set; }

        public int Pagina { get; set; } = 1;

        public int RegistrosPorPagina { get; set; } = 10;
    }

}








