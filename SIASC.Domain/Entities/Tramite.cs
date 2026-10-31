using System;
using System.Collections.Generic;
using System.Text;


namespace SIASC.Domain.Entities;

public class Tramite
{
    public int TramiteId { get; set; }

    public int ClienteId { get; set; }

    public int TipoServicioId { get; set; }

    public int EstadoActualId { get; set; }

    public DateTime FechaInicio { get; set; } = DateTime.UtcNow;

    public DateTime FechaUltimaActualizacion { get; set; } = DateTime.UtcNow;

    public DateTime? FechaCierre { get; set; }

    public long CostoTotalCentavos { get; set; }

    public string? ObservacionesGenerales { get; set; }

    public string? MotivoCancelacion { get; set; }

    public Cliente Cliente { get; set; } = null!;

    public TipoServicio TipoServicio { get; set; } = null!;

    public EstadoTramite EstadoActual { get; set; } = null!;
}

