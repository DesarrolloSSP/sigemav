using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class Marca
{
    public int MarcaId { get; set; }

    public string MarcaNombre { get; set; } = null!;

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual ICollection<ConceptoServicioCosto> ConceptoServicioCostos { get; set; } = new List<ConceptoServicioCosto>();

    public virtual ICollection<Modelo> Modelos { get; set; } = new List<Modelo>();

    public virtual ICollection<UnidadVehicular> UnidadVehiculars { get; set; } = new List<UnidadVehicular>();
}
