using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class TallerMovimientoPresupuesto
{
    public int IdMovimientoTaller { get; set; }

    public int TallerId { get; set; }

    public int SolicitudMantenimientoId { get; set; }

    public decimal Importe { get; set; }

    public string TipoMovimiento { get; set; } = null!;

    public DateTime FechaMovimiento { get; set; }

    public string? UsuarioCaptura { get; set; }

    public bool Activo { get; set; }

    public virtual SolicitudMantenimiento SolicitudMantenimiento { get; set; } = null!;

    public virtual Taller Taller { get; set; } = null!;
}
