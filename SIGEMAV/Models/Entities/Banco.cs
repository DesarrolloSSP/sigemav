using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class Banco
{
    public int BancoId { get; set; }

    public string Nombre { get; set; } = null!;

    public bool Activo { get; set; }

    public virtual ICollection<Taller> Tallers { get; set; } = new List<Taller>();
}
