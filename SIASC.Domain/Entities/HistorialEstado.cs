using System;
using System.Collections.Generic;
using System.Text;


namespace SIASC.Domain.Entities;

public class HistorialEstado
{
    public int HistorialId { get; set; }

    public int TramiteId { get; set; }

    public int? EstadoAnteriorId { get; set; }

    public int EstadoNuevoId { get; set; }

    public DateTime FechaCambio { get; set; } = DateTime.UtcNow;

    public string UsuarioId { get; set; } = string.Empty;

    public string? MotivoCambio { get; set; }

    public Tramite Tramite { get; set; } = null!;

    public EstadoTramite? EstadoAnterior { get; set; }

    public EstadoTramite EstadoNuevo { get; set; } = null!;
}
