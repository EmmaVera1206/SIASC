using System;
using System.Collections.Generic;
using System.Text;


namespace SIASC.Domain.Entities;

public class EmpleoAnterior
{
    public int EmpleoAnteriorId { get; set; }
    public int FormularioVisaId { get; set; }

    public string NombreEmpresa { get; set; } = string.Empty;
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
    public string? Puesto { get; set; }
    public string? NombreJefeInmediato { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaTermino { get; set; }

    public FormularioVisa FormularioVisa { get; set; } = null!;
}
