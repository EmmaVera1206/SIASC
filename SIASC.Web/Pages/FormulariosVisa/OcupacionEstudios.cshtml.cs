
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SIASC.Domain.Entities;
using SIASC.Infrastructure.Data;

namespace SIASC.Web.Pages.FormulariosVisa;

[Authorize]
public class OcupacionEstudiosModel : PageModel
{
    private readonly SIASCDbContext _context;

    public OcupacionEstudiosModel(SIASCDbContext context)
    {
        _context = context;
    }

    public int TramiteId { get; set; }

    public string NombreCliente { get; set; } = string.Empty;

    public bool PuedeEditar { get; set; } = true;

    public List<EmpleoRegistrado> Empleos { get; set; } = new();

    [BindProperty]
    public DatosEntrada Input { get; set; } = new();

    [BindProperty]
    public NuevoEmpleoEntrada NuevoEmpleo { get; set; } = new();

    public class DatosEntrada
    {
        [StringLength(150)]
        [Display(Name = "Ocupación actual")]
        public string? Ocupacion { get; set; }

        [StringLength(200)]
        [Display(Name = "Nombre de la empresa actual")]
        public string? NombreEmpresaActual { get; set; }

        [StringLength(500)]
        [Display(Name = "Dirección de la empresa")]
        public string? DireccionEmpresaActual { get; set; }

        [StringLength(30)]
        [Display(Name = "Teléfono de la empresa")]
        public string? TelefonoEmpresaActual { get; set; }

        [StringLength(150)]
        [Display(Name = "Puesto actual")]
        public string? PuestoActual { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de inicio del empleo")]
        public DateTime? FechaInicioEmpleo { get; set; }

        [StringLength(200)]
        [Display(Name = "Nombre de la institución")]
        public string? NombreInstitucion { get; set; }

        [StringLength(500)]
        [Display(Name = "Dirección de la institución")]
        public string? DireccionInstitucion { get; set; }

        [StringLength(30)]
        [Display(Name = "Teléfono de la institución")]
        public string? TelefonoInstitucion { get; set; }

        [StringLength(200)]
        [Display(Name = "Carrera o curso")]
        public string? CarreraCurso { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de inicio de estudios")]
        public DateTime? FechaInicioEstudios { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de finalización de estudios")]
        public DateTime? FechaFinEstudios { get; set; }
    }

    public class NuevoEmpleoEntrada
    {
        [StringLength(200)]
        [Display(Name = "Nombre de la empresa")]
        public string? NombreEmpresa { get; set; }

        [StringLength(500)]
        [Display(Name = "Dirección")]
        public string? Direccion { get; set; }

        [StringLength(30)]
        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }

        [StringLength(150)]
        [Display(Name = "Puesto")]
        public string? Puesto { get; set; }

        [StringLength(200)]
        [Display(Name = "Nombre del jefe inmediato")]
        public string? NombreJefeInmediato { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de inicio")]
        public DateTime? FechaInicio { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de término")]
        public DateTime? FechaTermino { get; set; }
    }

    public class EmpleoRegistrado
    {
        public int EmpleoAnteriorId { get; set; }

        public string NombreEmpresa { get; set; } = string.Empty;

        public string? Puesto { get; set; }

        public string? NombreJefeInmediato { get; set; }

        public DateTime? FechaInicio { get; set; }

        public DateTime? FechaTermino { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        if (!await CargarTramiteAsync(id))
        {
            return NotFound();
        }

        var formulario = await _context.FormulariosVisa
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.TramiteId == id);

        if (formulario != null)
        {
            PuedeEditar = formulario.EstadoCaptura == "Borrador";
            CargarDatos(formulario);
        }

        await CargarEmpleosAsync(id);

        return Page();
    }

    public async Task<IActionResult> OnPostGuardarAsync(int id)
    {
        if (!await CargarTramiteAsync(id))
        {
            return NotFound();
        }

        ModelState.Remove("NuevoEmpleo.NombreEmpresa");
        ModelState.Remove("NuevoEmpleo.Direccion");
        ModelState.Remove("NuevoEmpleo.Telefono");
        ModelState.Remove("NuevoEmpleo.Puesto");
        ModelState.Remove("NuevoEmpleo.NombreJefeInmediato");
        ModelState.Remove("NuevoEmpleo.FechaInicio");
        ModelState.Remove("NuevoEmpleo.FechaTermino");

        var formulario = await _context.FormulariosVisa
            .FirstOrDefaultAsync(f => f.TramiteId == id);

        if (formulario != null &&
            formulario.EstadoCaptura != "Borrador")
        {
            PuedeEditar = false;

            ModelState.AddModelError(
                string.Empty,
                "El formulario ya no está en borrador.");

            await CargarEmpleosAsync(id);
            return Page();
        }

        if (Input.FechaInicioEstudios.HasValue &&
            Input.FechaFinEstudios.HasValue &&
            Input.FechaFinEstudios < Input.FechaInicioEstudios)
        {
            ModelState.AddModelError(
                "Input.FechaFinEstudios",
                "La fecha de finalización no puede ser anterior al inicio.");
        }

        if (!ModelState.IsValid)
        {
            await CargarEmpleosAsync(id);
            return Page();
        }

        if (formulario == null)
        {
            formulario = new FormularioVisa
            {
                TramiteId = id,
                EstadoCaptura = "Borrador",
                FechaCreacion = DateTime.UtcNow
            };

            _context.FormulariosVisa.Add(formulario);
        }

        formulario.Ocupacion = Limpiar(Input.Ocupacion);

        formulario.NombreEmpresaActual =
            Limpiar(Input.NombreEmpresaActual);

        formulario.DireccionEmpresaActual =
            Limpiar(Input.DireccionEmpresaActual);

        formulario.TelefonoEmpresaActual =
            Limpiar(Input.TelefonoEmpresaActual);

        formulario.PuestoActual = Limpiar(Input.PuestoActual);

        formulario.FechaInicioEmpleo =
            Input.FechaInicioEmpleo;

        formulario.NombreInstitucion =
            Limpiar(Input.NombreInstitucion);

        formulario.DireccionInstitucion =
            Limpiar(Input.DireccionInstitucion);

        formulario.TelefonoInstitucion =
            Limpiar(Input.TelefonoInstitucion);

        formulario.CarreraCurso = Limpiar(Input.CarreraCurso);

        formulario.FechaInicioEstudios =
            Input.FechaInicioEstudios;

        formulario.FechaFinEstudios =
            Input.FechaFinEstudios;

        formulario.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        TempData["MensajeExito"] =
            "La información laboral y académica se guardó correctamente.";

        return RedirectToPage(
            "/FormulariosVisa/OcupacionEstudios",
            new { id });
    }

    public async Task<IActionResult> OnPostAgregarEmpleoAsync(int id)
    {
        if (!await CargarTramiteAsync(id))
        {
            return NotFound();
        }

        var formulario = await _context.FormulariosVisa
            .FirstOrDefaultAsync(f => f.TramiteId == id);

        if (formulario != null &&
            formulario.EstadoCaptura != "Borrador")
        {
            return BadRequest(
                "El formulario no permite modificaciones.");
        }

        ModelState.Clear();

        if (string.IsNullOrWhiteSpace(NuevoEmpleo.NombreEmpresa))
        {
            ModelState.AddModelError(
                "NuevoEmpleo.NombreEmpresa",
                "Ingresa el nombre de la empresa.");
        }
        else if (NuevoEmpleo.NombreEmpresa.Trim().Length > 200)
        {
            ModelState.AddModelError(
                "NuevoEmpleo.NombreEmpresa",
                "El nombre no puede superar 200 caracteres.");
        }

        if (NuevoEmpleo.FechaInicio.HasValue &&
            NuevoEmpleo.FechaTermino.HasValue &&
            NuevoEmpleo.FechaTermino < NuevoEmpleo.FechaInicio)
        {
            ModelState.AddModelError(
                "NuevoEmpleo.FechaTermino",
                "La fecha de término no puede ser anterior al inicio.");
        }

        ValidarLongitud(
            "NuevoEmpleo.Direccion",
            NuevoEmpleo.Direccion,
            500);

        ValidarLongitud(
            "NuevoEmpleo.Telefono",
            NuevoEmpleo.Telefono,
            30);

        ValidarLongitud(
            "NuevoEmpleo.Puesto",
            NuevoEmpleo.Puesto,
            150);

        ValidarLongitud(
            "NuevoEmpleo.NombreJefeInmediato",
            NuevoEmpleo.NombreJefeInmediato,
            200);

        if (!ModelState.IsValid)
        {
            if (formulario != null)
            {
                CargarDatos(formulario);
            }

            await CargarEmpleosAsync(id);
            return Page();
        }

        if (formulario == null)
        {
            formulario = new FormularioVisa
            {
                TramiteId = id,
                EstadoCaptura = "Borrador",
                FechaCreacion = DateTime.UtcNow
            };

            _context.FormulariosVisa.Add(formulario);

            await _context.SaveChangesAsync();
        }

        var empleo = new EmpleoAnterior
        {
            FormularioVisaId = formulario.FormularioVisaId,
            NombreEmpresa = NuevoEmpleo.NombreEmpresa!.Trim(),
            Direccion = Limpiar(NuevoEmpleo.Direccion),
            Telefono = Limpiar(NuevoEmpleo.Telefono),
            Puesto = Limpiar(NuevoEmpleo.Puesto),

            NombreJefeInmediato =
                Limpiar(NuevoEmpleo.NombreJefeInmediato),

            FechaInicio = NuevoEmpleo.FechaInicio,
            FechaTermino = NuevoEmpleo.FechaTermino
        };

        _context.EmpleosAnteriores.Add(empleo);

        formulario.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        TempData["MensajeExito"] =
            "El empleo anterior se registró correctamente.";

        return RedirectToPage(
            "/FormulariosVisa/OcupacionEstudios",
            new { id });
    }

    public async Task<IActionResult> OnPostEliminarEmpleoAsync(
        int id,
        int empleoId)
    {
        if (!await CargarTramiteAsync(id))
        {
            return NotFound();
        }

        var formulario = await _context.FormulariosVisa
            .FirstOrDefaultAsync(f => f.TramiteId == id);

        if (formulario == null ||
            formulario.EstadoCaptura != "Borrador")
        {
            return BadRequest(
                "No es posible modificar este formulario.");
        }

        var empleo = await _context.EmpleosAnteriores
            .FirstOrDefaultAsync(e =>
                e.EmpleoAnteriorId == empleoId &&
                e.FormularioVisaId == formulario.FormularioVisaId);

        if (empleo == null)
        {
            return NotFound();
        }

        _context.EmpleosAnteriores.Remove(empleo);

        formulario.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        TempData["MensajeExito"] =
            "El empleo anterior se eliminó correctamente.";

        return RedirectToPage(
            "/FormulariosVisa/OcupacionEstudios",
            new { id });
    }

    private async Task<bool> CargarTramiteAsync(int id)
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
        {
            return false;
        }

        TramiteId = tramite.TramiteId;
        NombreCliente = tramite.NombreCliente;

        return true;
    }

    private async Task CargarEmpleosAsync(int id)
    {
        Empleos = await _context.EmpleosAnteriores
            .AsNoTracking()
            .Where(e => e.FormularioVisa.TramiteId == id)
            .OrderByDescending(e => e.FechaInicio)
            .Select(e => new EmpleoRegistrado
            {
                EmpleoAnteriorId = e.EmpleoAnteriorId,
                NombreEmpresa = e.NombreEmpresa,
                Puesto = e.Puesto,
                NombreJefeInmediato = e.NombreJefeInmediato,
                FechaInicio = e.FechaInicio,
                FechaTermino = e.FechaTermino
            })
            .ToListAsync();
    }

    private void CargarDatos(FormularioVisa formulario)
    {
        Input = new DatosEntrada
        {
            Ocupacion = formulario.Ocupacion,
            NombreEmpresaActual = formulario.NombreEmpresaActual,
            DireccionEmpresaActual = formulario.DireccionEmpresaActual,
            TelefonoEmpresaActual = formulario.TelefonoEmpresaActual,
            PuestoActual = formulario.PuestoActual,
            FechaInicioEmpleo = formulario.FechaInicioEmpleo,

            NombreInstitucion = formulario.NombreInstitucion,
            DireccionInstitucion = formulario.DireccionInstitucion,
            TelefonoInstitucion = formulario.TelefonoInstitucion,
            CarreraCurso = formulario.CarreraCurso,
            FechaInicioEstudios = formulario.FechaInicioEstudios,
            FechaFinEstudios = formulario.FechaFinEstudios
        };
    }

    private void ValidarLongitud(
        string campo,
        string? valor,
        int longitudMaxima)
    {
        if (valor != null &&
            valor.Trim().Length > longitudMaxima)
        {
            ModelState.AddModelError(
                campo,
                $"El campo no puede superar {longitudMaxima} caracteres.");
        }
    }

    private static string? Limpiar(string? valor)
    {
        return string.IsNullOrWhiteSpace(valor)
            ? null
            : valor.Trim();
    }
}

