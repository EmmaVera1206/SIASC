using System;
using System.Collections.Generic;
using System.Text;


namespace SIASC.Domain.Entities;
public class DatosPasaporte
{
    public int DatosPasaporteId { get; set; }

    public int TramiteId { get; set; }

    public string CURP { get; set; } = string.Empty;

    public bool CURPVerificada { get; set; }

    public DateTime? FechaVerificacion { get; set; }

    public string? Observaciones { get; set; }

    public Tramite Tramite { get; set; } = null!;
}
