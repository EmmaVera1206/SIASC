using System;
using System.Collections.Generic;
using System.Text;


namespace SIASC.Domain.Entities;

public class EstadoTramite
{
    public int EstadoId { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public bool EsFinal { get; set; }

    public int OrdenVisual { get; set; }

    public bool Activo { get; set; } = true;
}

