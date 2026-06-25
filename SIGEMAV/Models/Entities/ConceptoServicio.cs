using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class ConceptoServicio
{
    public int ConceptoServicioId { get; set; }

    public int TipoCatalogoServicioId { get; set; }

    public string ConceptoServicioNombre { get; set; } = null!;

    public string? ConceptoServicioDescripcion { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual ICollection<ConceptoServicioCosto> ConceptoServicioCostos { get; set; } = new List<ConceptoServicioCosto>();

    public virtual ICollection<ConfiguracionValidacionTecnica> ConfiguracionValidacionTecnicas { get; set; } = new List<ConfiguracionValidacionTecnica>();

    public virtual ICollection<MantenimientoDetalle> MantenimientoDetalles { get; set; } = new List<MantenimientoDetalle>();

    public virtual ICollection<SolicitudMantenimientoDetalle> SolicitudMantenimientoDetalles { get; set; } = new List<SolicitudMantenimientoDetalle>();

    public virtual TipoCatalogoServicio TipoCatalogoServicio { get; set; } = null!;
}
