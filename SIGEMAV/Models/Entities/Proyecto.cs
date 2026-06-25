using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class Proyecto
{
    public int IdProyecto { get; set; }

    public string ClaveProyecto { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public int IdClaveAdmin { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual ICollection<DispPresupuestal> DispPresupuestals { get; set; } = new List<DispPresupuestal>();

    public virtual ClaveAdministrativa IdClaveAdminNavigation { get; set; } = null!;

    public virtual ICollection<ProyectoArea> ProyectoAreas { get; set; } = new List<ProyectoArea>();
}
