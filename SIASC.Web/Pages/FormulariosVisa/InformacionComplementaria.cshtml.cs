
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SIASC.Domain.Entities;
using SIASC.Infrastructure.Data;

namespace SIASC.Web.Pages.FormulariosVisa;

[Authorize]
public class InformacionComplementariaModel : PageModel
{
    private readonly SIASCDbContext _context;

    public InformacionComplementariaModel(SIASCDbContext context)
    {
        _context = context;
    }

    public int TramiteId { get; set; }
    public string NombreCliente { get; set; } = string.Empty;
    public bool PuedeEditar { get; set; } = true;

    public List<RedSocialVisa> RedesSociales { get; set; } = new();
    public List<IdiomaVisa> Idiomas { get; set; } = new();
    public List<PaisVisitado> PaisesVisitados { get; set; } = new();

    [BindProperty]
    public string? Plataforma { get; set; }

    [BindProperty]
    public string? NombreUsuario { get; set; }

    [BindProperty]
    public string? NuevoIdioma { get; set; }

    [BindProperty]
    public string? NuevoPais { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        if (!await CargarTramiteAsync(id))
            return NotFound();

        await CargarRegistrosAsync(id);
        return Page();
    }

    public async Task<IActionResult> OnPostAgregarRedAsync(int id)
    {
        if (!await CargarTramiteAsync(id))
            return NotFound();

        var formulario = await ObtenerFormularioEditableAsync(id);

        if (formulario == null)
            return BadRequest("El formulario no permite modificaciones.");

        Plataforma = Plataforma?.Trim();
        NombreUsuario = NombreUsuario?.Trim();

        ModelState.Clear();

        if (string.IsNullOrWhiteSpace(Plataforma))
            ModelState.AddModelError(
                nameof(Plataforma),
                "Selecciona o escribe una plataforma.");
        else if (Plataforma.Length > 100)
            ModelState.AddModelError(
                nameof(Plataforma),
                "La plataforma no puede superar 100 caracteres.");

        if (string.IsNullOrWhiteSpace(NombreUsuario))
            ModelState.AddModelError(
                nameof(NombreUsuario),
                "Ingresa el nombre de usuario.");
        else if (NombreUsuario.Length > 200)
            ModelState.AddModelError(
                nameof(NombreUsuario),
                "El usuario no puede superar 200 caracteres.");

        if (!ModelState.IsValid)
        {
            await CargarRegistrosAsync(id);
            return Page();
        }

        var duplicada = await _context.RedesSocialesVisa
            .AnyAsync(r =>
                r.FormularioVisaId == formulario.FormularioVisaId &&
                r.Plataforma == Plataforma &&
                r.NombreUsuario == NombreUsuario);

        if (duplicada)
        {
            ModelState.AddModelError(
                string.Empty,
                "Esta red social ya está registrada.");

            await CargarRegistrosAsync(id);
            return Page();
        }

        _context.RedesSocialesVisa.Add(new RedSocialVisa
        {
            FormularioVisaId = formulario.FormularioVisaId,
            Plataforma = Plataforma!,
            NombreUsuario = NombreUsuario!
        });

        formulario.FechaActualizacion = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        TempData["MensajeExito"] =
            "La red social se registró correctamente.";

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostEliminarRedAsync(
        int id, int redId)
    {
        if (!await CargarTramiteAsync(id))
            return NotFound();

        var formulario = await ObtenerFormularioEditableAsync(
            id, false);

        if (formulario == null)
            return BadRequest("El formulario no permite modificaciones.");

        var red = await _context.RedesSocialesVisa
            .FirstOrDefaultAsync(r =>
                r.RedSocialVisaId == redId &&
                r.FormularioVisaId == formulario.FormularioVisaId);

        if (red == null)
            return NotFound();

        _context.RedesSocialesVisa.Remove(red);
        formulario.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        TempData["MensajeExito"] =
            "La red social se eliminó correctamente.";

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostAgregarIdiomaAsync(int id)
    {
        if (!await CargarTramiteAsync(id))
            return NotFound();

        var formulario = await ObtenerFormularioEditableAsync(id);

        if (formulario == null)
            return BadRequest("El formulario no permite modificaciones.");

        NuevoIdioma = NuevoIdioma?.Trim();
        ModelState.Clear();

        if (string.IsNullOrWhiteSpace(NuevoIdioma))
            ModelState.AddModelError(
                nameof(NuevoIdioma),
                "Ingresa un idioma.");
        else if (NuevoIdioma.Length > 100)
            ModelState.AddModelError(
                nameof(NuevoIdioma),
                "El idioma no puede superar 100 caracteres.");

        if (!ModelState.IsValid)
        {
            await CargarRegistrosAsync(id);
            return Page();
        }

        var duplicado = await _context.IdiomasVisa
            .AnyAsync(i =>
                i.FormularioVisaId == formulario.FormularioVisaId &&
                i.Idioma == NuevoIdioma);

        if (duplicado)
        {
            ModelState.AddModelError(
                nameof(NuevoIdioma),
                "Este idioma ya está registrado.");

            await CargarRegistrosAsync(id);
            return Page();
        }

        _context.IdiomasVisa.Add(new IdiomaVisa
        {
            FormularioVisaId = formulario.FormularioVisaId,
            Idioma = NuevoIdioma!
        });

        formulario.FechaActualizacion = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        TempData["MensajeExito"] =
            "El idioma se registró correctamente.";

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostEliminarIdiomaAsync(
        int id, int idiomaId)
    {
        if (!await CargarTramiteAsync(id))
            return NotFound();

        var formulario = await ObtenerFormularioEditableAsync(
            id, false);

        if (formulario == null)
            return BadRequest("El formulario no permite modificaciones.");

        var idioma = await _context.IdiomasVisa
            .FirstOrDefaultAsync(i =>
                i.IdiomaVisaId == idiomaId &&
                i.FormularioVisaId == formulario.FormularioVisaId);

        if (idioma == null)
            return NotFound();

        _context.IdiomasVisa.Remove(idioma);
        formulario.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        TempData["MensajeExito"] =
            "El idioma se eliminó correctamente.";

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostAgregarPaisAsync(int id)
    {
        if (!await CargarTramiteAsync(id))
            return NotFound();

        var formulario = await ObtenerFormularioEditableAsync(id);

        if (formulario == null)
            return BadRequest("El formulario no permite modificaciones.");

        NuevoPais = NuevoPais?.Trim();
        ModelState.Clear();

        if (string.IsNullOrWhiteSpace(NuevoPais))
            ModelState.AddModelError(
                nameof(NuevoPais),
                "Ingresa un país.");
        else if (NuevoPais.Length > 100)
            ModelState.AddModelError(
                nameof(NuevoPais),
                "El país no puede superar 100 caracteres.");

        if (!ModelState.IsValid)
        {
            await CargarRegistrosAsync(id);
            return Page();
        }

        var duplicado = await _context.PaisesVisitados
            .AnyAsync(p =>
                p.FormularioVisaId == formulario.FormularioVisaId &&
                p.Pais == NuevoPais);

        if (duplicado)
        {
            ModelState.AddModelError(
                nameof(NuevoPais),
                "Este país ya está registrado.");

            await CargarRegistrosAsync(id);
            return Page();
        }

        _context.PaisesVisitados.Add(new PaisVisitado
        {
            FormularioVisaId = formulario.FormularioVisaId,
            Pais = NuevoPais!
        });

        formulario.FechaActualizacion = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        TempData["MensajeExito"] =
            "El país visitado se registró correctamente.";

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostEliminarPaisAsync(
        int id, int paisId)
    {
        if (!await CargarTramiteAsync(id))
            return NotFound();

        var formulario = await ObtenerFormularioEditableAsync(
            id, false);

        if (formulario == null)
            return BadRequest("El formulario no permite modificaciones.");

        var pais = await _context.PaisesVisitados
            .FirstOrDefaultAsync(p =>
                p.PaisVisitadoId == paisId &&
                p.FormularioVisaId == formulario.FormularioVisaId);

        if (pais == null)
            return NotFound();

        _context.PaisesVisitados.Remove(pais);
        formulario.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        TempData["MensajeExito"] =
            "El país visitado se eliminó correctamente.";

        return RedirectToPage(new { id });
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
            return false;

        TramiteId = tramite.TramiteId;
        NombreCliente = tramite.NombreCliente;

        return true;
    }

    private async Task<FormularioVisa?> ObtenerFormularioEditableAsync(
        int id,
        bool crearSiNoExiste = true)
    {
        var formulario = await _context.FormulariosVisa
            .FirstOrDefaultAsync(f => f.TramiteId == id);

        if (formulario != null)
        {
            return formulario.EstadoCaptura == "Borrador"
                ? formulario
                : null;
        }

        if (!crearSiNoExiste)
            return null;

        formulario = new FormularioVisa
        {
            TramiteId = id,
            EstadoCaptura = "Borrador",
            FechaCreacion = DateTime.UtcNow,
            FechaActualizacion = DateTime.UtcNow
        };

        _context.FormulariosVisa.Add(formulario);
        await _context.SaveChangesAsync();

        return formulario;
    }

    private async Task CargarRegistrosAsync(int id)
    {
        var formulario = await _context.FormulariosVisa
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.TramiteId == id);

        if (formulario == null)
            return;

        PuedeEditar = formulario.EstadoCaptura == "Borrador";

        RedesSociales = await _context.RedesSocialesVisa
            .AsNoTracking()
            .Where(r => r.FormularioVisaId == formulario.FormularioVisaId)
            .OrderBy(r => r.Plataforma)
            .ThenBy(r => r.NombreUsuario)
            .ToListAsync();

        Idiomas = await _context.IdiomasVisa
            .AsNoTracking()
            .Where(i => i.FormularioVisaId == formulario.FormularioVisaId)
            .OrderBy(i => i.Idioma)
            .ToListAsync();

        PaisesVisitados = await _context.PaisesVisitados
            .AsNoTracking()
            .Where(p => p.FormularioVisaId == formulario.FormularioVisaId)
            .OrderBy(p => p.Pais)
            .ToListAsync();
    }
}
