
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SIASC.Domain.Entities;
using SIASC.Infrastructure.Data;

namespace SIASC.Web.Pages.FormulariosVisa;

[Authorize]
public class ResumenModel : PageModel
{
    private readonly SIASCDbContext _context;

    public ResumenModel(SIASCDbContext context)
    {
        _context = context;
    }

    public int TramiteId { get; private set; }
    public string NombreCliente { get; private set; } = "";
    public string EstadoCaptura { get; private set; } = "";
    public DateTime FechaCreacion { get; private set; }
    public DateTime FechaActualizacion { get; private set; }
    public DateTime? FechaRevision { get; private set; }

    public List<SeccionResumen> Secciones { get; } = new();

    public class CampoResumen
    {
        public string Etiqueta { get; set; } = "";
        public string Valor { get; set; } = "";
    }

    public class TablaResumen
    {
        public string Titulo { get; set; } = "";
        public List<string> Columnas { get; set; } = new();
        public List<List<string>> Filas { get; set; } = new();
    }

    public class SeccionResumen
    {
        public string Titulo { get; set; } = "";
        public string PaginaEdicion { get; set; } = "";
        public List<CampoResumen> Campos { get; } = new();
        public List<TablaResumen> Tablas { get; } = new();

        public SeccionResumen Agregar(string etiqueta, object? valor)
        {
            Campos.Add(new CampoResumen
            {
                Etiqueta = etiqueta,
                Valor = Formatear(valor)
            });

            return this;
        }

        public SeccionResumen Tabla(
            string titulo,
            string[] columnas,
            IEnumerable<object?[]> filas)
        {
            Tablas.Add(new TablaResumen
            {
                Titulo = titulo,
                Columnas = columnas.ToList(),
                Filas = filas
                    .Select(f => f.Select(Formatear).ToList())
                    .ToList()
            });

            return this;
        }
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var tramite = await _context.Tramites
            .AsNoTracking()
            .Where(t =>
                t.TramiteId == id &&
                t.TipoServicio.Codigo == "VISA_AMERICANA")
            .Select(t => new
            {
                t.TramiteId,
                NombreCliente = t.Cliente.NombreCompleto
            })
            .FirstOrDefaultAsync();

        if (tramite == null)
            return NotFound();

        TramiteId = tramite.TramiteId;
        NombreCliente = tramite.NombreCliente;

        var formulario = await _context.FormulariosVisa
            .AsNoTracking()
            .Include(f => f.Acompanantes)
            .Include(f => f.Familiares)
            .Include(f => f.EmpleosAnteriores)
            .Include(f => f.RedesSociales)
            .Include(f => f.Idiomas)
            .Include(f => f.PaisesVisitados)
            .Include(f => f.VisitasAnteriores)
            .AsSplitQuery()
            .FirstOrDefaultAsync(f => f.TramiteId == id);

        if (formulario == null)
        {
            EstadoCaptura = "Sin iniciar";
            return Page();
        }

        EstadoCaptura = formulario.EstadoCaptura;
        FechaCreacion = formulario.FechaCreacion;
        FechaActualizacion = formulario.FechaActualizacion;
        FechaRevision = formulario.FechaRevision;

        ConstruirResumen(formulario);

        return Page();
    }

    private void ConstruirResumen(FormularioVisa f)
    {
        var personales = NuevaSeccion(
            "1. Datos personales y contacto",
            "/FormulariosVisa/DatosPersonales");

        personales
            .Agregar("Nombre según pasaporte", f.NombreSegunPasaporte)
            .Agregar("Sexo", f.Sexo)
            .Agregar("Estado civil", f.EstadoCivil)
            .Agregar("Nacionalidad", f.Nacionalidad)
            .Agregar("¿Tiene otra nacionalidad?", f.TieneOtraNacionalidad)
            .Agregar("Otra nacionalidad", f.OtraNacionalidad)
            .Agregar("¿Reside en otro país?", f.ResideEnOtroPais)
            .Agregar("País de residencia", f.PaisResidencia)
            .Agregar("Domicilio particular", f.DomicilioParticular)
            .Agregar("Teléfono particular", f.TelefonoParticular)
            .Agregar("Teléfono adicional", f.TelefonoAdicional)
            .Agregar("Correo personal", f.CorreoPersonal);

        var viaje = NuevaSeccion(
            "2. Planes y gastos del viaje",
            "/FormulariosVisa/PlanesViaje");

        viaje
            .Agregar("¿Tiene planes de viaje?", f.TienePlanesViaje)
            .Agregar("Fecha del viaje", f.FechaViaje)
            .Agregar("Motivo del viaje", f.MotivoViaje)
            .Agregar("Ciudad de destino", f.CiudadDestino)
            .Agregar("Hospedaje", f.Hospedaje)
            .Agregar("Empresa que visitará", f.EmpresaVisita)
            .Agregar("Dirección de la empresa", f.DireccionEmpresaVisita)
            .Agregar("Teléfono de la empresa", f.TelefonoEmpresaVisita)
            .Agregar("Correo de la empresa", f.CorreoEmpresaVisita)
            .Agregar("Responsable de los gastos", f.ResponsableGastos)
            .Agregar("Nombre del patrocinador", f.NombrePatrocinador)
            .Agregar("Parentesco del patrocinador", f.ParentescoPatrocinador)
            .Agregar("Teléfono del patrocinador", f.TelefonoPatrocinador);

        var historial = NuevaSeccion(
            "3. Historial de visas y viajes anteriores",
            "/FormulariosVisa/HistorialVisa");

        historial
            .Agregar("¿Ha visitado Estados Unidos?", f.HaVisitadoEstadosUnidos)
            .Agregar("¿Ha tenido visa?", f.HaTenidoVisa)
            .Agregar("¿Le han negado una visa?", f.VisaNegada)
            .Agregar("Fecha de negativa", f.FechaNegativa)
            .Agregar("Detalle de negativa", f.DetalleNegativa)
            .Agregar("¿Le han cancelado una visa?", f.VisaCancelada)
            .Agregar("Fecha de cancelación", f.FechaCancelacionVisa)
            .Agregar("Detalle de cancelación", f.DetalleCancelacionVisa)
            .Agregar("¿Visa extraviada o robada?", f.VisaExtraviadaORobada)
            .Agregar("Detalle de extravío o robo", f.DetalleExtravíoVisa);

        historial.Tabla(
            "Visitas anteriores a Estados Unidos",
            new[] { "Fecha de visita", "Duración de estancia" },
            f.VisitasAnteriores
                .OrderByDescending(v => v.FechaVisita)
                .Select(v => new object?[]
                {
                    v.FechaVisita,
                    v.DuracionEstancia
                }));

        var familia = NuevaSeccion(
            "4. Información familiar",
            "/FormulariosVisa/InformacionFamiliar");

        familia
            .Agregar("Nombre del padre", f.NombrePadre)
            .Agregar("Nacimiento del padre", f.FechaNacimientoPadre)
            .Agregar("¿Padre en Estados Unidos?", f.PadreEnEstadosUnidos)
            .Agregar("Nombre de la madre", f.NombreMadre)
            .Agregar("Nacimiento de la madre", f.FechaNacimientoMadre)
            .Agregar("¿Madre en Estados Unidos?", f.MadreEnEstadosUnidos)
            .Agregar("¿Familiares inmediatos en EE. UU.?",
                f.TieneFamiliaresInmediatosEEUU)
            .Agregar("¿Otros familiares en EE. UU.?",
                f.TieneOtrosFamiliaresEEUU)
            .Agregar("Nombre del cónyuge", f.NombreConyuge)
            .Agregar("Nacimiento del cónyuge", f.FechaNacimientoConyuge)
            .Agregar("Lugar de nacimiento del cónyuge",
                f.LugarNacimientoConyuge)
            .Agregar("Fecha de matrimonio", f.FechaMatrimonio)
            .Agregar("Fecha de divorcio", f.FechaDivorcio)
            .Agregar("Información del divorcio", f.MotivoDivorcio);

        familia.Tabla(
            "Familiares registrados en Estados Unidos",
            new[] { "Nombre", "Parentesco", "Estatus legal" },
            f.Familiares
                .OrderBy(x => x.NombreCompleto)
                .Select(x => new object?[]
                {
                    x.NombreCompleto,
                    x.Parentesco,
                    x.EstatusLegalEEUU
                }));

        var ocupacion = NuevaSeccion(
            "5. Ocupación, estudios y empleos",
            "/FormulariosVisa/OcupacionEstudios");

        ocupacion
            .Agregar("Ocupación actual", f.Ocupacion)
            .Agregar("Empresa actual", f.NombreEmpresaActual)
            .Agregar("Dirección de la empresa", f.DireccionEmpresaActual)
            .Agregar("Teléfono de la empresa", f.TelefonoEmpresaActual)
            .Agregar("Puesto actual", f.PuestoActual)
            .Agregar("Inicio del empleo", f.FechaInicioEmpleo)
            .Agregar("Institución educativa", f.NombreInstitucion)
            .Agregar("Dirección de la institución", f.DireccionInstitucion)
            .Agregar("Teléfono de la institución", f.TelefonoInstitucion)
            .Agregar("Carrera o curso", f.CarreraCurso)
            .Agregar("Inicio de estudios", f.FechaInicioEstudios)
            .Agregar("Fin de estudios", f.FechaFinEstudios);

        ocupacion.Tabla(
            "Empleos anteriores",
            new[]
            {
                "Empresa", "Dirección", "Teléfono", "Puesto",
                "Jefe inmediato", "Inicio", "Término"
            },
            f.EmpleosAnteriores
                .OrderByDescending(e => e.FechaInicio)
                .Select(e => new object?[]
                {
                    e.NombreEmpresa,
                    e.Direccion,
                    e.Telefono,
                    e.Puesto,
                    e.NombreJefeInmediato,
                    e.FechaInicio,
                    e.FechaTermino
                }));

        var complementaria = NuevaSeccion(
            "6. Información complementaria",
            "/FormulariosVisa/InformacionComplementaria");

        complementaria.Tabla(
            "Redes sociales",
            new[] { "Plataforma", "Nombre de usuario" },
            f.RedesSociales
                .OrderBy(r => r.Plataforma)
                .Select(r => new object?[]
                {
                    r.Plataforma,
                    r.NombreUsuario
                }));

        complementaria.Tabla(
            "Idiomas",
            new[] { "Idioma" },
            f.Idiomas
                .OrderBy(i => i.Idioma)
                .Select(i => new object?[] { i.Idioma }));

        complementaria.Tabla(
            "Países visitados",
            new[] { "País" },
            f.PaisesVisitados
                .OrderBy(p => p.Pais)
                .Select(p => new object?[] { p.Pais }));

        var acompanantes = NuevaSeccion(
            "7. Acompañantes de viaje",
            "/FormulariosVisa/AcompanantesViaje");

        acompanantes.Tabla(
            "Acompañantes registrados",
            new[] { "Nombre completo", "Parentesco o relación" },
            f.Acompanantes
                .OrderBy(a => a.NombreCompleto)
                .Select(a => new object?[]
                {
                    a.NombreCompleto,
                    a.Parentesco
                }));
    }

    private SeccionResumen NuevaSeccion(
        string titulo,
        string pagina)
    {
        var seccion = new SeccionResumen
        {
            Titulo = titulo,
            PaginaEdicion = pagina
        };

        Secciones.Add(seccion);
        return seccion;
    }

    public static string Formatear(object? valor)
    {
        if (valor == null)
            return "No registrado";

        if (valor is bool booleano)
            return booleano ? "Sí" : "No";

        if (valor is DateTime fecha)
            return fecha.ToString(
                "dd/MM/yyyy",
                CultureInfo.GetCultureInfo("es-MX"));

        var texto = valor.ToString();

        return string.IsNullOrWhiteSpace(texto)
            ? "No registrado"
            : texto;
    }
}
