using System;
using System.Collections.Generic;
using System.Text;


namespace SIASC.Domain.Entities;

public class Pendiente
{
    public int PendienteId { get; set; }

    public int? TramiteId { get; set; }

    public string Titulo { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public DateTime? FechaVencimiento { get; set; }

    public string Prioridad { get; set; } = "Media";

    public string Estado { get; set; } = "Pendiente";

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public DateTime? FechaFinalizacion { get; set; }

    public Tramite? Tramite { get; set; }
}
