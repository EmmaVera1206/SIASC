using System;
using System.Collections.Generic;
using System.Text;


namespace SIASC.Domain.Entities;

public class Pago
{
    public int PagoId { get; set; }

    public int TramiteId { get; set; }

    public long MontoCentavos { get; set; }

    public DateTime FechaPago { get; set; }

    public string MetodoPago { get; set; } = string.Empty;

    public string TipoPago { get; set; } = string.Empty;

    public string? Referencia { get; set; }

    public string? Observaciones { get; set; }

    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    public bool Anulado { get; set; }

    public DateTime? FechaAnulacion { get; set; }

    public string? MotivoAnulacion { get; set; }

    public Tramite Tramite { get; set; } = null!;
}
