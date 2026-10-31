using System;
using System.Collections.Generic;
using System.Text;


namespace SIASC.Domain.Entities;

public class Cita
{
    public int CitaId { get; set; }

    public int TramiteId { get; set; }

    public string TipoCita { get; set; } = string.Empty;

    public DateTime? FechaTentativa { get; set; }

    public DateTime? FechaConfirmada { get; set; }

    public string? Lugar { get; set; }

    public string EstadoCita { get; set; } = string.Empty;

    public string? Observaciones { get; set; }

    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;

    public Tramite Tramite { get; set; } = null!;
}

