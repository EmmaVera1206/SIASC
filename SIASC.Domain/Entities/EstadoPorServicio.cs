using System;
using System.Collections.Generic;
using System.Text;


namespace SIASC.Domain.Entities;

public class EstadoPorServicio
{
    public int TipoServicioId { get; set; }

    public int EstadoId { get; set; }

    public TipoServicio TipoServicio { get; set; } = null!;

    public EstadoTramite Estado { get; set; } = null!;
}

