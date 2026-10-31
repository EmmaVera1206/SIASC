
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SIASC.Domain.Entities;
using SIASC.Infrastructure.Data;

namespace SIASC.Web.Pages.FormulariosVisa;

[Authorize]
public class InformacionFamiliarModel : PageModel
{
    private readonly SIASCDbContext _context;

    public InformacionFamiliarModel(SIASCDbContext context)
    {
        _context = context;
    }

    public int TramiteId { get; set; }

    public string NombreCliente { get; set; } = string.Empty;

    public bool PuedeEditar { get; set; } = true;

    public List<FamiliarRegistrado> Familiares { get; set; } = new();

    [BindProperty]
    public DatosEntrada Input { get; set; } = new();

    [BindProperty]
    public NuevoFamiliarEntrada NuevoFamiliar { get; set; } = new();

    public class DatosEntrada
    {
        [StringLength(200)]
        [Display(Name = "Nombre completo del padre")]
        public string? NombrePadre { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de nacimiento del padre")]
        public DateTime? FechaNacimientoPadre { get; set; }

        [Display(Name = "¿El padre está en Estados Unidos?")]
        public bool? PadreEnEstadosUnidos { get; set; }

        [StringLength(200)]
        [Display(Name = "Nombre completo de la madre")]
        public string? NombreMadre { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de nacimiento de la madre")]
        public DateTime? FechaNacimientoMadre { get; set; }

        [Display(Name = "¿La madre está en Estados Unidos?")]
        public bool? MadreEnEstadosUnidos { get; set; }

        [Display(Name = "¿Tiene familiares inmediatos en Estados Unidos?")]
        public bool? TieneFamiliaresInmediatosEEUU { get; set; }

        [Display(Name = "¿Tiene otros familiares en Estados Unidos?")]
        public bool? TieneOtrosFamiliaresEEUU { get; set; }

        [StringLength(200)]
        [Display(Name = "Nombre completo del cónyuge")]
        public string? NombreConyuge { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de nacimiento del cónyuge")]
        public DateTime? FechaNacimientoConyuge { get; set; }

        [StringLength(200)]
        [Display(Name = "Lugar de nacimiento del cónyuge")]
        public string? LugarNacimientoConyuge { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de matrimonio")]
        public DateTime? FechaMatrimonio { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de divorcio")]
        public DateTime? FechaDivorcio { get; set; }

        [StringLength(2000)]
        [Display(Name = "Información sobre el divorcio")]
        public string? MotivoDivorcio { get; set; }
    }

    public class NuevoFamiliarEntrada
    {
        [StringLength(200)]
        [Display(Name = "Nombre completo")]
        public string? NombreCompleto { get; set; }

        [StringLength(100)]
        [Display(Name = "Parentesco")]
        public string? Parentesco { get; set; }

        [StringLength(100)]
        [Display(Name = "Estatus legal en Estados Unidos")]
        public string? EstatusLegalEEUU { get; set; }
    }

    public class FamiliarRegistrado
    {
        public int FamiliarVisaId { get; set; }

        public string NombreCompleto { get; set; } = string.Empty;

        public string Parentesco { get; set; } = string.Empty;

        public string EstatusLegalEEUU { get; set; } = string.Empty;
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

        await CargarFamiliaresAsync(id);

        return Page();
    }

    public async Task<IActionResult> OnPostGuardarAsync(int id)
    {
        if (!await CargarTramiteAsync(id))
        {
            return NotFound();
        }

        ModelState.Remove("NuevoFamiliar.NombreCompleto");
        ModelState.Remove("NuevoFamiliar.Parentesco");
        ModelState.Remove("NuevoFamiliar.EstatusLegalEEUU");

        var formulario = await _context.FormulariosVisa
            .FirstOrDefaultAsync(f => f.TramiteId == id);

        if (formulario != null &&
            formulario.EstadoCaptura != "Borrador")
        {
            PuedeEditar = false;

            ModelState.AddModelError(
                string.Empty,
                "El formulario ya no está en borrador.");

            await CargarFamiliaresAsync(id);
            return Page();
        }

        if (!ModelState.IsValid)
        {
            await CargarFamiliaresAsync(id);
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

        formulario.NombrePadre = Limpiar(Input.NombrePadre);
        formulario.FechaNacimientoPadre = Input.FechaNacimientoPadre;
        formulario.PadreEnEstadosUnidos = Input.PadreEnEstadosUnidos;

        formulario.NombreMadre = Limpiar(Input.NombreMadre);
        formulario.FechaNacimientoMadre = Input.FechaNacimientoMadre;
        formulario.MadreEnEstadosUnidos = Input.MadreEnEstadosUnidos;

        formulario.TieneFamiliaresInmediatosEEUU =
            Input.TieneFamiliaresInmediatosEEUU;

        formulario.TieneOtrosFamiliaresEEUU =
            Input.TieneOtrosFamiliaresEEUU;

        formulario.NombreConyuge = Limpiar(Input.NombreConyuge);

        formulario.FechaNacimientoConyuge =
            Input.FechaNacimientoConyuge;

        formulario.LugarNacimientoConyuge =
            Limpiar(Input.LugarNacimientoConyuge);

        formulario.FechaMatrimonio = Input.FechaMatrimonio;
        formulario.FechaDivorcio = Input.FechaDivorcio;
        formulario.MotivoDivorcio = Limpiar(Input.MotivoDivorcio);

        formulario.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        TempData["MensajeExito"] =
            "La información familiar se guardó correctamente.";

        return RedirectToPage(
            "/FormulariosVisa/InformacionFamiliar",
            new { id });
    }

    public async Task<IActionResult> OnPostAgregarFamiliarAsync(int id)
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

        if (string.IsNullOrWhiteSpace(NuevoFamiliar.NombreCompleto))
        {
            ModelState.AddModelError(
                "NuevoFamiliar.NombreCompleto",
                "Ingresa el nombre del familiar.");
        }
        else if (NuevoFamiliar.NombreCompleto.Trim().Length > 200)
        {
            ModelState.AddModelError(
                "NuevoFamiliar.NombreCompleto",
                "El nombre no puede superar 200 caracteres.");
        }

        if (string.IsNullOrWhiteSpace(NuevoFamiliar.Parentesco))
        {
            ModelState.AddModelError(
                "NuevoFamiliar.Parentesco",
                "Indica el parentesco.");
        }
        else if (NuevoFamiliar.Parentesco.Trim().Length > 100)
        {
            ModelState.AddModelError(
                "NuevoFamiliar.Parentesco",
                "El parentesco no puede superar 100 caracteres.");
        }

        if (string.IsNullOrWhiteSpace(NuevoFamiliar.EstatusLegalEEUU))
        {
            ModelState.AddModelError(
                "NuevoFamiliar.EstatusLegalEEUU",
                "Indica el estatus legal.");
        }
        else if (NuevoFamiliar.EstatusLegalEEUU.Trim().Length > 100)
        {
            ModelState.AddModelError(
                "NuevoFamiliar.EstatusLegalEEUU",
                "El estatus no puede superar 100 caracteres.");
        }

        if (!ModelState.IsValid)
        {
            if (formulario != null)
            {
                CargarDatos(formulario);
            }

            await CargarFamiliaresAsync(id);
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

        var familiar = new FamiliarVisa
        {
            FormularioVisaId = formulario.FormularioVisaId,
            NombreCompleto = NuevoFamiliar.NombreCompleto!.Trim(),
            Parentesco = NuevoFamiliar.Parentesco!.Trim(),
            EstatusLegalEEUU = NuevoFamiliar.EstatusLegalEEUU!.Trim()
        };

        _context.FamiliaresVisa.Add(familiar);

        formulario.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        TempData["MensajeExito"] =
            "El familiar se registró correctamente.";

        return RedirectToPage(
            "/FormulariosVisa/InformacionFamiliar",
            new { id });
    }

    public async Task<IActionResult> OnPostEliminarFamiliarAsync(
        int id,
        int familiarId)
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

        var familiar = await _context.FamiliaresVisa
            .FirstOrDefaultAsync(f =>
                f.FamiliarVisaId == familiarId &&
                f.FormularioVisaId == formulario.FormularioVisaId);

        if (familiar == null)
        {
            return NotFound();
        }

        _context.FamiliaresVisa.Remove(familiar);

        formulario.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        TempData["MensajeExito"] =
            "El familiar se eliminó correctamente.";

        return RedirectToPage(
            "/FormulariosVisa/InformacionFamiliar",
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

    private async Task CargarFamiliaresAsync(int id)
    {
        Familiares = await _context.FamiliaresVisa
            .AsNoTracking()
            .Where(f => f.FormularioVisa.TramiteId == id)
            .OrderBy(f => f.NombreCompleto)
            .Select(f => new FamiliarRegistrado
            {
                FamiliarVisaId = f.FamiliarVisaId,
                NombreCompleto = f.NombreCompleto,
                Parentesco = f.Parentesco,
                EstatusLegalEEUU = f.EstatusLegalEEUU
            })
            .ToListAsync();
    }

    private void CargarDatos(FormularioVisa formulario)
    {
        Input = new DatosEntrada
        {
            NombrePadre = formulario.NombrePadre,
            FechaNacimientoPadre = formulario.FechaNacimientoPadre,
            PadreEnEstadosUnidos = formulario.PadreEnEstadosUnidos,

            NombreMadre = formulario.NombreMadre,
            FechaNacimientoMadre = formulario.FechaNacimientoMadre,
            MadreEnEstadosUnidos = formulario.MadreEnEstadosUnidos,

            TieneFamiliaresInmediatosEEUU =
                formulario.TieneFamiliaresInmediatosEEUU,

            TieneOtrosFamiliaresEEUU =
                formulario.TieneOtrosFamiliaresEEUU,

            NombreConyuge = formulario.NombreConyuge,
            FechaNacimientoConyuge =
                formulario.FechaNacimientoConyuge,

            LugarNacimientoConyuge =
                formulario.LugarNacimientoConyuge,

            FechaMatrimonio = formulario.FechaMatrimonio,
            FechaDivorcio = formulario.FechaDivorcio,
            MotivoDivorcio = formulario.MotivoDivorcio
        };
    }

    private static string? Limpiar(string? valor)
    {
        return string.IsNullOrWhiteSpace(valor)
            ? null
            : valor.Trim();
    }
}
