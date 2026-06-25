using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class Dsp
{
    public int IdDsp { get; set; }

    public DateOnly Fecha { get; set; }

    public string NoDsp { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public decimal Importe { get; set; }

    public string RutaArchivo { get; set; } = null!;

    public bool Activo { get; set; }

    public virtual ICollection<DispPresupuestal> DispPresupuestals { get; set; } = new List<DispPresupuestal>();
}
