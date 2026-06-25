using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class EstatusMantenimiento
{
    public int EstatusMantenimientoId { get; set; }

    public string Nombre { get; set; } = null!;

    public bool Activo { get; set; }

    public virtual ICollection<MantenimientoSeguimiento> MantenimientoSeguimientos { get; set; } = new List<MantenimientoSeguimiento>();

    public virtual ICollection<Mantenimiento> Mantenimientos { get; set; } = new List<Mantenimiento>();
}
