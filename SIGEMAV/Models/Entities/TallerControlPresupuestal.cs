using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class TallerControlPresupuestal
{
    public int IdControlTaller { get; set; }

    public short Ejercicio { get; set; }

    public decimal PresupuestoAsignado { get; set; }

    public decimal PresupuestoEjercido { get; set; }

    public DateOnly FechaInicio { get; set; }

    public DateOnly FechaFin { get; set; }

    public bool Activo { get; set; }

    public decimal PresupuestoComprometido { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    public decimal? PresupuestoDisponible { get; set; }

    public int TallerConfiguracionPresupuestalId { get; set; }

    public virtual TallerConfiguracionPresupuestal TallerConfiguracionPresupuestal { get; set; } = null!;

    public virtual ICollection<TallerMovimientoPresupuesto> TallerMovimientoPresupuestos { get; set; } = new List<TallerMovimientoPresupuesto>();
}
