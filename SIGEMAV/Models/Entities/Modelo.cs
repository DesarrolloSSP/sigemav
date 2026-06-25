using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class Modelo
{
    public int ModeloId { get; set; }

    public string ModeloNombre { get; set; } = null!;

    public int MarcaId { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual ICollection<ConceptoServicioCosto> ConceptoServicioCostos { get; set; } = new List<ConceptoServicioCosto>();

    public virtual Marca Marca { get; set; } = null!;

    public virtual ICollection<UnidadVehicular> UnidadVehiculars { get; set; } = new List<UnidadVehicular>();
}
