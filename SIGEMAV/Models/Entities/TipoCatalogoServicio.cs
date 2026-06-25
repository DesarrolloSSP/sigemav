using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class TipoCatalogoServicio
{
    public int TipoCatalogoServicioId { get; set; }

    public string TipoCatalogoServicioNombre { get; set; } = null!;

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual ICollection<ConceptoServicio> ConceptoServicios { get; set; } = new List<ConceptoServicio>();

    public virtual ICollection<ConfiguracionValidacionTecnica> ConfiguracionValidacionTecnicas { get; set; } = new List<ConfiguracionValidacionTecnica>();
}
