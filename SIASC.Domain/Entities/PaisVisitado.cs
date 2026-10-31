using System;
using System.Collections.Generic;
using System.Text;


namespace SIASC.Domain.Entities;

public class PaisVisitado
{
    public int PaisVisitadoId { get; set; }
    public int FormularioVisaId { get; set; }

    public string Pais { get; set; } = string.Empty;

    public FormularioVisa FormularioVisa { get; set; } = null!;
}
