using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class Color
{
    public int ColorId { get; set; }

    public string ColorNombre { get; set; } = null!;

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual ICollection<UnidadVehicular> UnidadVehiculars { get; set; } = new List<UnidadVehicular>();
}
