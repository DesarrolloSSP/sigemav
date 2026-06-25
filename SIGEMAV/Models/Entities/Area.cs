using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class Area
{
    public int AreaId { get; set; }

    public string AreaNombre { get; set; } = null!;

    public bool Activo { get; set; }

    public bool ActivoFinancieros { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual ICollection<DispPresupuestal> DispPresupuestals { get; set; } = new List<DispPresupuestal>();

    public virtual ICollection<ProyectoArea> ProyectoAreas { get; set; } = new List<ProyectoArea>();

    public virtual ICollection<SolicitudMantenimiento> SolicitudMantenimientos { get; set; } = new List<SolicitudMantenimiento>();

    public virtual ICollection<TransferenciaUnidad> TransferenciaUnidadAreaDestinos { get; set; } = new List<TransferenciaUnidad>();

    public virtual ICollection<TransferenciaUnidad> TransferenciaUnidadAreaOrigens { get; set; } = new List<TransferenciaUnidad>();

    public virtual ICollection<UnidadVehicular> UnidadVehiculars { get; set; } = new List<UnidadVehicular>();
}
