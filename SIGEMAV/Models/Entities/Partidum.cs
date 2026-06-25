using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class Partidum
{
    public int IdPartida { get; set; }

    public string ClavePartida { get; set; } = null!;

    public string Partida { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual ICollection<OrdenPagoDetalle> OrdenPagoDetalles { get; set; } = new List<OrdenPagoDetalle>();
}
