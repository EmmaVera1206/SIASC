using System;
using System.Collections.Generic;
using System.Text;


namespace SIASC.Domain.Entities;

public class Documento
{
    public int DocumentoId { get; set; }

    public int TramiteId { get; set; }

    public string TipoDocumento { get; set; } = string.Empty;

    public string NombreOriginal { get; set; } = string.Empty;

    public string NombreAlmacenado { get; set; } = string.Empty;

    public string TipoMime { get; set; } = string.Empty;

    public long TamanoBytes { get; set; }

    public string HashSHA256 { get; set; } = string.Empty;

    public DateTime FechaCarga { get; set; } = DateTime.UtcNow;

    public string? Observaciones { get; set; }

    public bool Activo { get; set; } = true;

    public Tramite Tramite { get; set; } = null!;
}
