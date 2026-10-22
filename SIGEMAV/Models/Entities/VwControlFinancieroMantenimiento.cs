using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class VwControlFinancieroMantenimiento
{
    public string Folio { get; set; } = null!;

    public int MantenimientoId { get; set; }

    public string Uuid { get; set; } = null!;

    public decimal? Total { get; set; }

    public decimal? Comprometido { get; set; }

    public decimal? Devengado { get; set; }

    public decimal? Pagado { get; set; }
}
