using System;
using System.Collections.Generic;
using System.Text;


namespace SIASC.Domain.Entities;

public class TransicionEstado
{
    public int TransicionId { get; set; }

    public int TipoServicioId { get; set; }

    public int EstadoOrigenId { get; set; }

    public int EstadoDestinoId { get; set; }

    public bool Activo { get; set; } = true;

    public TipoServicio TipoServicio { get; set; } = null!;

    public EstadoTramite EstadoOrigen { get; set; } = null!;

    public EstadoTramite EstadoDestino { get; set; } = null!;
}

