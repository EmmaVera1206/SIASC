using System;
using System.Collections.Generic;
using System.Text;


namespace SIASC.Domain.Entities;

public class Seguimiento
{
    public int SeguimientoId { get; set; }

    public int TramiteId { get; set; }

    public string Descripcion { get; set; } = string.Empty;

    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    public string UsuarioId { get; set; } = string.Empty;

    public Tramite Tramite { get; set; } = null!;
}
