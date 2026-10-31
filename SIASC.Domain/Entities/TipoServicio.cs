using System;
using System.Collections.Generic;
using System.Text;


namespace SIASC.Domain.Entities;

public class TipoServicio
{
    public int TipoServicioId { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public bool Activo { get; set; } = true;

    public ICollection<Tramite> Tramites { get; set; } = new List<Tramite>();
}
