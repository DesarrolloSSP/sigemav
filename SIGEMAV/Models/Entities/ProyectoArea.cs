using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class ProyectoArea
{
    public int IdProyectoArea { get; set; }

    public int IdProyecto { get; set; }

    public int IdArea { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual Area IdAreaNavigation { get; set; } = null!;

    public virtual Proyecto IdProyectoNavigation { get; set; } = null!;
}
