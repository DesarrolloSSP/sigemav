using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class FacturaMantenimiento
{
    public int FacturaMantenimientoId { get; set; }

    public int MantenimientoId { get; set; }

    public int TallerId { get; set; }

    public int EstatusFacturaMantenimientoId { get; set; }

    public string Uuid { get; set; } = null!;

    public string? Serie { get; set; }

    public string? Folio { get; set; }

    public DateOnly FechaFactura { get; set; }

    public DateTime FechaRecepcion { get; set; }

    public string Rfcemisor { get; set; } = null!;

    public string? Rfcreceptor { get; set; }

    public string? RazonSocial { get; set; }

    public string? MetodoPago { get; set; }

    public string? FormaPago { get; set; }

    public string? UsoCfdi { get; set; }

    public string? Moneda { get; set; }

    public string? TipoComprobante { get; set; }

    public decimal? Subtotal { get; set; }

    public decimal? Iva { get; set; }

    public decimal? Retencion { get; set; }

    public decimal? Total { get; set; }

    public string? Observaciones { get; set; }

    public DateTime FechaRegistro { get; set; }

    public int UsuarioRegistroId { get; set; }

    public bool Activo { get; set; }

    public DateTime? FechaTimbrado { get; set; }

    public string? CodigoPostalEmision { get; set; }

    public string? NoCertificado { get; set; }

    public string? NoCertificadoSat { get; set; }

    public string? EstadoSat { get; set; }

    public DateTime? FechaValidacionSat { get; set; }

    public string? CodigoEstatusSat { get; set; }

    public virtual EstatusFacturaMantenimiento EstatusFacturaMantenimiento { get; set; } = null!;

    public virtual ICollection<FacturaMantenimientoDetalle> FacturaMantenimientoDetalles { get; set; } = new List<FacturaMantenimientoDetalle>();

    public virtual ICollection<FacturaMantenimientoDocumento> FacturaMantenimientoDocumentos { get; set; } = new List<FacturaMantenimientoDocumento>();

    public virtual Mantenimiento Mantenimiento { get; set; } = null!;

    public virtual ICollection<MovimientoPresupuestal> MovimientoPresupuestals { get; set; } = new List<MovimientoPresupuestal>();

    public virtual Taller Taller { get; set; } = null!;
}
