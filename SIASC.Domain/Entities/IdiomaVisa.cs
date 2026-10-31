using System;
using System.Collections.Generic;
using System.Text;


namespace SIASC.Domain.Entities;

public class IdiomaVisa
{
    public int IdiomaVisaId { get; set; }
    public int FormularioVisaId { get; set; }

    public string Idioma { get; set; } = string.Empty;

    public FormularioVisa FormularioVisa { get; set; } = null!;
}
