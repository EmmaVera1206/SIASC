using System;
using System.Collections.Generic;
using System.Text;


namespace SIASC.Domain.Entities;

public class FormularioVisa
{
    public int FormularioVisaId { get; set; }
    public int TramiteId { get; set; }

    // Información personal
    public string? NombreSegunPasaporte { get; set; }
    public string? Sexo { get; set; }
    public string? EstadoCivil { get; set; }
    public string? Nacionalidad { get; set; }
    public bool? TieneOtraNacionalidad { get; set; }
    public string? OtraNacionalidad { get; set; }
    public bool? ResideEnOtroPais { get; set; }
    public string? PaisResidencia { get; set; }

    // Planes de viaje
    public bool? TienePlanesViaje { get; set; }
    public DateTime? FechaViaje { get; set; }
    public string? MotivoViaje { get; set; }
    public string? CiudadDestino { get; set; }
    public string? Hospedaje { get; set; }
    public string? EmpresaVisita { get; set; }
    public string? DireccionEmpresaVisita { get; set; }
    public string? TelefonoEmpresaVisita { get; set; }
    public string? CorreoEmpresaVisita { get; set; }

    // Gastos del viaje
    public string? ResponsableGastos { get; set; }
    public string? NombrePatrocinador { get; set; }
    public string? ParentescoPatrocinador { get; set; }
    public string? TelefonoPatrocinador { get; set; }

    // Historial de visa
    public bool? HaVisitadoEstadosUnidos { get; set; }
    public bool? HaTenidoVisa { get; set; }
    public bool? VisaNegada { get; set; }
    public DateTime? FechaNegativa { get; set; }
    public string? DetalleNegativa { get; set; }
    public bool? VisaCancelada { get; set; }
    public DateTime? FechaCancelacionVisa { get; set; }
    public string? DetalleCancelacionVisa { get; set; }
    public bool? VisaExtraviadaORobada { get; set; }
    public string? DetalleExtravíoVisa { get; set; }

    // Contacto
    public string? DomicilioParticular { get; set; }
    public string? TelefonoParticular { get; set; }
    public string? TelefonoAdicional { get; set; }
    public string? CorreoPersonal { get; set; }

    // Padres
    public string? NombrePadre { get; set; }
    public DateTime? FechaNacimientoPadre { get; set; }
    public bool? PadreEnEstadosUnidos { get; set; }
    public string? NombreMadre { get; set; }
    public DateTime? FechaNacimientoMadre { get; set; }
    public bool? MadreEnEstadosUnidos { get; set; }

    // Familia en Estados Unidos
    public bool? TieneFamiliaresInmediatosEEUU { get; set; }
    public bool? TieneOtrosFamiliaresEEUU { get; set; }

    // Cónyuge actual o anterior
    public string? NombreConyuge { get; set; }
    public DateTime? FechaNacimientoConyuge { get; set; }
    public string? LugarNacimientoConyuge { get; set; }
    public DateTime? FechaMatrimonio { get; set; }
    public DateTime? FechaDivorcio { get; set; }
    public string? MotivoDivorcio { get; set; }

    // Ocupación actual
    public string? Ocupacion { get; set; }
    public string? NombreEmpresaActual { get; set; }
    public string? DireccionEmpresaActual { get; set; }
    public string? TelefonoEmpresaActual { get; set; }
    public string? PuestoActual { get; set; }
    public DateTime? FechaInicioEmpleo { get; set; }

    // Últimos estudios
    public string? NombreInstitucion { get; set; }
    public string? DireccionInstitucion { get; set; }
    public string? TelefonoInstitucion { get; set; }
    public string? CarreraCurso { get; set; }
    public DateTime? FechaInicioEstudios { get; set; }
    public DateTime? FechaFinEstudios { get; set; }

    // Control de captura
    public string EstadoCaptura { get; set; } = "Borrador";
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;
    public DateTime? FechaRevision { get; set; }

    // Relaciones
    public Tramite Tramite { get; set; } = null!;

    public ICollection<AcompananteViaje> Acompanantes { get; set; }
        = new List<AcompananteViaje>();

    public ICollection<FamiliarVisa> Familiares { get; set; }
        = new List<FamiliarVisa>();

    public ICollection<EmpleoAnterior> EmpleosAnteriores { get; set; }
        = new List<EmpleoAnterior>();

    public ICollection<RedSocialVisa> RedesSociales { get; set; }
        = new List<RedSocialVisa>();

    public ICollection<IdiomaVisa> Idiomas { get; set; }
        = new List<IdiomaVisa>();

    public ICollection<PaisVisitado> PaisesVisitados { get; set; }
        = new List<PaisVisitado>();

    public ICollection<VisitaAnteriorEEUU> VisitasAnteriores { get; set; }
        = new List<VisitaAnteriorEEUU>();
}

