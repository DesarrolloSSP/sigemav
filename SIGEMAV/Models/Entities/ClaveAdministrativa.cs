using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class ClaveAdministrativa
{
    public int IdClaveAdmin { get; set; }

    public string ClaveAdmin { get; set; } = null!;

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual ICollection<DispPresupuestal> DispPresupuestals { get; set; } = new List<DispPresupuestal>();

    public virtual ICollection<Proyecto> Proyectos { get; set; } = new List<Proyecto>();
}
