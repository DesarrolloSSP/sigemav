using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class Taller
{
    public int TallerId { get; set; }

    public string RazonSocial { get; set; } = null!;

    public string Rfc { get; set; } = null!;

    public int BancoId { get; set; }

    public string Ubicacion { get; set; } = null!;

    public bool Activo { get; set; }

    public string? Clabe { get; set; }

    public int? ObjetivoMantenimientoId { get; set; }

    public string? Fianza { get; set; }

    public virtual Banco Banco { get; set; } = null!;

    public virtual ICollection<FacturaMantenimiento> FacturaMantenimientos { get; set; } = new List<FacturaMantenimiento>();

    public virtual ICollection<Mantenimiento> Mantenimientos { get; set; } = new List<Mantenimiento>();

    public virtual ObjetivoMantenimiento? ObjetivoMantenimiento { get; set; }

    public virtual ICollection<OrdenPago> OrdenPagos { get; set; } = new List<OrdenPago>();

    public virtual ICollection<SolicitudMantenimiento> SolicitudMantenimientos { get; set; } = new List<SolicitudMantenimiento>();

    public virtual ICollection<TallerConfiguracion> TallerConfiguracions { get; set; } = new List<TallerConfiguracion>();

    public virtual ICollection<TallerMovimientoPresupuesto> TallerMovimientoPresupuestos { get; set; } = new List<TallerMovimientoPresupuesto>();
}
