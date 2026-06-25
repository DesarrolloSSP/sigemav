using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class SolicitudMantenimientoDsp
{
    public int SolicitudMantenimientoDspid { get; set; }

    public int SolicitudMantenimientoId { get; set; }

    public string? NumeroDsp { get; set; }

    public DateTime FechaDsp { get; set; }

    public decimal PresupuestoDisponible { get; set; }

    public decimal ImporteSolicitado { get; set; }

    public decimal ImporteAutorizado { get; set; }

    public bool TieneSuficiencia { get; set; }

    public string? Observaciones { get; set; }

    public int? UsuarioValidaId { get; set; }

    public DateTime FechaCreacion { get; set; }

    public bool Activo { get; set; }

    public virtual SolicitudMantenimiento SolicitudMantenimiento { get; set; } = null!;
}
