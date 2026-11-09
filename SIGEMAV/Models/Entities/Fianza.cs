using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class Fianza
{
    public int FianzaId { get; set; }

    public string Fianza1 { get; set; } = null!;

    public bool Activo { get; set; }
}
