using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class SolicitudMantenimiento
{
    public int SolicitudMantenimientoId { get; set; }

    public string Folio { get; set; } = null!;

    public string NumeroOficio { get; set; } = null!;

    public DateTime FechaSolicitud { get; set; }

    public int AreaSolicitanteId { get; set; }

    public int UnidadVehicularId { get; set; }

    public int? TallerId { get; set; }

    public int TipoMantenimientoId { get; set; }

    public int KilometrajeActual { get; set; }

    public string? MotivoSolicitud { get; set; }

    public string? DiagnosticoInicial { get; set; }

    public string? Observaciones { get; set; }

    public decimal? ImporteCotizacion { get; set; }

    public int EstatusSolicitudMantenimientoId { get; set; }

    public DateTime? FechaAutorizacion { get; set; }

    public int? UsuarioSolicitaId { get; set; }

    public int? UsuarioAutorizaId { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public string? FacturaSerie { get; set; }

    public string? FacturaFolio { get; set; }

    public string? PlacaVehiculo { get; set; }

    public virtual Area AreaSolicitante { get; set; } = null!;

    public virtual EstatusSolicitudMantenimiento EstatusSolicitudMantenimiento { get; set; } = null!;

    public virtual ICollection<Mantenimiento> Mantenimientos { get; set; } = new List<Mantenimiento>();

    public virtual ICollection<MovimientoPresupuestal> MovimientoPresupuestals { get; set; } = new List<MovimientoPresupuestal>();

    public virtual ICollection<SolicitudMantenimientoDetalle> SolicitudMantenimientoDetalles { get; set; } = new List<SolicitudMantenimientoDetalle>();

    public virtual ICollection<SolicitudMantenimientoDocumento> SolicitudMantenimientoDocumentos { get; set; } = new List<SolicitudMantenimientoDocumento>();

    public virtual ICollection<SolicitudMantenimientoDsp> SolicitudMantenimientoDsps { get; set; } = new List<SolicitudMantenimientoDsp>();

    public virtual ICollection<SolicitudMantenimientoSeguimiento> SolicitudMantenimientoSeguimientos { get; set; } = new List<SolicitudMantenimientoSeguimiento>();

    public virtual Taller? Taller { get; set; }

    public virtual ICollection<TallerMovimientoPresupuesto> TallerMovimientoPresupuestos { get; set; } = new List<TallerMovimientoPresupuesto>();

    public virtual TipoMantenimiento TipoMantenimiento { get; set; } = null!;

    public virtual UnidadVehicular UnidadVehicular { get; set; } = null!;
}
