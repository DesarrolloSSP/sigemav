using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class ConfiguracionValidacionTecnica
{
    public int ConfiguracionValidacionTecnicaId { get; set; }

    public int? TipoCatalogoServicioId { get; set; }

    public int? ConceptoServicioId { get; set; }

    public bool ValidarSolicitudesAbiertas { get; set; }

    public string? EstatusSolicitudesAbiertas { get; set; }

    public bool ValidarRecurrencia { get; set; }

    public int? DiasRecurrencia { get; set; }

    public decimal? AdvertenciaPorcentaje { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public virtual ConceptoServicio? ConceptoServicio { get; set; }

    public virtual ICollection<ConfiguracionValidacionEstatus> ConfiguracionValidacionEstatuses { get; set; } = new List<ConfiguracionValidacionEstatus>();

    public virtual TipoCatalogoServicio? TipoCatalogoServicio { get; set; }
}
