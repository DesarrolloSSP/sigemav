using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class Recurso
{
    public int RecursoId { get; set; }

    public string Nombre { get; set; } = null!;

    public bool Activo { get; set; }

    public virtual ICollection<Taller> Tallers { get; set; } = new List<Taller>();
}
