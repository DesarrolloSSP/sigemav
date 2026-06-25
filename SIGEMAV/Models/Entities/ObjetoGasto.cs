using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class ObjetoGasto
{
    public int IdObjetoGasto { get; set; }

    public string ClaveObjGasto { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual ICollection<DispPresupuestal> DispPresupuestals { get; set; } = new List<DispPresupuestal>();

    public virtual ICollection<MantenimientoDetalle> MantenimientoDetalleIdObjetoGastoManoObraNavigations { get; set; } = new List<MantenimientoDetalle>();

    public virtual ICollection<MantenimientoDetalle> MantenimientoDetalleIdObjetoGastoRefaccionesNavigations { get; set; } = new List<MantenimientoDetalle>();

    public virtual ICollection<SolicitudMantenimientoDetalle> SolicitudMantenimientoDetalleIdObjetoGastoManoObraNavigations { get; set; } = new List<SolicitudMantenimientoDetalle>();

    public virtual ICollection<SolicitudMantenimientoDetalle> SolicitudMantenimientoDetalleIdObjetoGastoRefaccionesNavigations { get; set; } = new List<SolicitudMantenimientoDetalle>();
}
