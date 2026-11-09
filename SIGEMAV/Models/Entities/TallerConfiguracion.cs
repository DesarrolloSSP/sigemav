using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class TallerConfiguracion
{
    public int TallerId { get; set; }

    public int TallerConfiguracionPresupuestalId { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual Taller Taller { get; set; } = null!;

    public virtual TallerConfiguracionPresupuestal TallerConfiguracionPresupuestal { get; set; } = null!;
}
