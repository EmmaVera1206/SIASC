using System;
using System.Collections.Generic;
using System.Text;


namespace SIASC.Domain.Entities;

public class RedSocialVisa
{
    public int RedSocialVisaId { get; set; }
    public int FormularioVisaId { get; set; }

    public string Plataforma { get; set; } = string.Empty;
    public string NombreUsuario { get; set; } = string.Empty;

    public FormularioVisa FormularioVisa { get; set; } = null!;
}
