using System;
using System.Collections.Generic;
using System.Text;


namespace SIASC.Domain.Entities;

public class HistorialCosto
{
    public int HistorialCostoId { get; set; }

    public int TramiteId { get; set; }

    public long CostoAnteriorCentavos { get; set; }

    public long CostoNuevoCentavos { get; set; }

    public DateTime FechaCambio { get; set; } = DateTime.UtcNow;

    public string Motivo { get; set; } = string.Empty;

    public string UsuarioId { get; set; } = string.Empty;

    public Tramite Tramite { get; set; } = null!;
}

