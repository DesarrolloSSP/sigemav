using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class EstatusFacturaMantenimiento
{
    public int EstatusFacturaMantenimientoId { get; set; }

    public string Nombre { get; set; } = null!;

    public string Color { get; set; } = null!;

    public int Orden { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<FacturaMantenimiento> FacturaMantenimientos { get; set; } = new List<FacturaMantenimiento>();
}
