using System;
using System.Collections.Generic;
using System.Text;


namespace SIASC.Domain.Entities;

public class VisitaAnteriorEEUU
{
    public int VisitaAnteriorEEUUId { get; set; }
    public int FormularioVisaId { get; set; }

    public DateTime? FechaVisita { get; set; }
    public string? DuracionEstancia { get; set; }

    public FormularioVisa FormularioVisa { get; set; } = null!;
}

