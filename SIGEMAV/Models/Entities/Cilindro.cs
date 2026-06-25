using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class Cilindro
{
    public int CilindroId { get; set; }

    public string CilindroNombre { get; set; } = null!;

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual ICollection<ConceptoServicioCosto> ConceptoServicioCostos { get; set; } = new List<ConceptoServicioCosto>();

    public virtual ICollection<UnidadVehicular> UnidadVehiculars { get; set; } = new List<UnidadVehicular>();
}
