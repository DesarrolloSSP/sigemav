using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class DispPresupuestal
{
    public int IdDispPresupuestal { get; set; }

    public int IdObjetoGasto { get; set; }

    public int IdClaveAdmin { get; set; }

    public int IdProyecto { get; set; }

    public int IdArea { get; set; }

    public decimal Importe { get; set; }

    public int IdDsp { get; set; }

    public bool Activo { get; set; }

    public virtual Area IdAreaNavigation { get; set; } = null!;

    public virtual ClaveAdministrativa IdClaveAdminNavigation { get; set; } = null!;

    public virtual Dsp IdDspNavigation { get; set; } = null!;

    public virtual ObjetoGasto IdObjetoGastoNavigation { get; set; } = null!;

    public virtual Proyecto IdProyectoNavigation { get; set; } = null!;

    public virtual ICollection<MovimientoPresupuestal> MovimientoPresupuestals { get; set; } = new List<MovimientoPresupuestal>();
}
