using System;
using System.Collections.Generic;
using System.Text;


namespace SIASC.Domain.Entities;

public class AcompananteViaje
{
    public int AcompananteViajeId { get; set; }
    public int FormularioVisaId { get; set; }

    public string NombreCompleto { get; set; } = string.Empty;
    public string Parentesco { get; set; } = string.Empty;

    public FormularioVisa FormularioVisa { get; set; } = null!;
}

