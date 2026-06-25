namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class ConfiguracionValidacionTecnicaIndexViewModel
    {
        public List<Item> Items { get; set; } = new();

        public class Item
        {
            public int ConfiguracionValidacionTecnicaId { get; set; }

            public string TipoCatalogoServicioNombre { get; set; }
            public string ConceptoServicioNombre { get; set; }

            public bool ValidarSolicitudesAbiertas { get; set; }
            public bool ValidarRecurrencia { get; set; }

            public int? DiasRecurrencia { get; set; }

            public string EstatusSolicitudesAbiertas { get; set; }

            public bool Activo { get; set; }

            public string EstatusSolicitudMantenimientoNombre { get; set; }
        }
    }



    public class ConfiguracionValidacionTecnicaListItemVm
    {
        public int Id { get; set; }

        public string TipoCatalogoServicio { get; set; }
        public string ConceptoServicio { get; set; }

        public bool ValidarSolicitudesAbiertas { get; set; }
        public bool ValidarRecurrencia { get; set; }
        public int? DiasRecurrencia { get; set; }

        public bool Activo { get; set; }
    }
}
