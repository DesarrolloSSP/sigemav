using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class Mantenimiento
{
    public int MantenimientoId { get; set; }

    public int UnidadVehicularId { get; set; }

    public int TipoMantenimientoId { get; set; }

    public int TallerId { get; set; }

    public DateTime FechaIngreso { get; set; }

    public DateTime? FechaSalida { get; set; }

    public int KilometrajeEntrada { get; set; }

    public int? KilometrajeSalida { get; set; }

    public string MotivoMantenimiento { get; set; } = null!;

    public string? Diagnostico { get; set; }

    public string? TrabajoRealizado { get; set; }

    public string? Observaciones { get; set; }

    public int? DiasFueraServicio { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public int? UsuarioCreacionId { get; set; }

    public int? SolicitudMantenimientoId { get; set; }

    public int? EstatusMantenimientoId { get; set; }

    public DateTime? FechaInicioTrabajo { get; set; }

    public DateTime? FechaFinTrabajo { get; set; }

    public decimal? CostoRealTotal { get; set; }

    public int? UsuarioCierreId { get; set; }

    public DateTime? FechaCierre { get; set; }

    public DateTime? FechaEjecucion { get; set; }

    public int? KilometrajeEjecucion { get; set; }

    public virtual EstatusMantenimiento? EstatusMantenimiento { get; set; }

    public virtual ICollection<MantenimientoDetalle> MantenimientoDetalles { get; set; } = new List<MantenimientoDetalle>();

    public virtual ICollection<MantenimientoDocumento> MantenimientoDocumentos { get; set; } = new List<MantenimientoDocumento>();

    public virtual ICollection<MantenimientoSeguimiento> MantenimientoSeguimientos { get; set; } = new List<MantenimientoSeguimiento>();

    public virtual SolicitudMantenimiento? SolicitudMantenimiento { get; set; }

    public virtual Taller Taller { get; set; } = null!;

    public virtual TipoMantenimiento TipoMantenimiento { get; set; } = null!;

    public virtual UnidadVehicular UnidadVehicular { get; set; } = null!;
}
