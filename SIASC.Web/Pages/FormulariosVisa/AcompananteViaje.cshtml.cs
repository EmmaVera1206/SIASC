
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SIASC.Domain.Entities;
using SIASC.Infrastructure.Data;

namespace SIASC.Web.Pages.FormulariosVisa;

[Authorize]
public class AcompanantesViajeModel : PageModel
{
    private readonly SIASCDbContext _context;

    public AcompanantesViajeModel(SIASCDbContext context)
    {
        _context = context;
    }

    public int TramiteId { get; set; }

    public string NombreCliente { get; set; } = string.Empty;

    public bool PuedeEditar { get; set; } = true;

    public List<AcompananteRegistrado> Acompanantes { get; set; } = new();

    [BindProperty]
    public string? NombreCompleto { get; set; }

    [BindProperty]
    public string? Parentesco { get; set; }

    public class AcompananteRegistrado
    {
        public int AcompananteViajeId { get; set; }

        public string NombreCompleto { get; set; } = string.Empty;

        public string Parentesco { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        if (!await CargarTramiteAsync(id))
        {
            return NotFound();
        }

        await CargarAcompanantesAsync(id);

        return Page();
    }

    public async Task<IActionResult> OnPostAgregarAsync(int id)
    {
        if (!await CargarTramiteAsync(id))
        {
            return NotFound();
        }

        var formulario = await ObtenerFormularioEditableAsync(id);

        if (formulario == null)
        {
            return BadRequest(
                "El formulario ya no permite modificaciones.");
        }

        NombreCompleto = NombreCompleto?.Trim();
        Parentesco = Parentesco?.Trim();

        ModelState.Clear();

        if (string.IsNullOrWhiteSpace(NombreCompleto))
        {
            ModelState.AddModelError(
                nameof(NombreCompleto),
                "Ingresa el nombre del acompañante.");
        }
        else if (NombreCompleto.Length > 200)
        {
            ModelState.AddModelError(
                nameof(NombreCompleto),
                "El nombre no puede superar 200 caracteres.");
        }

        if (string.IsNullOrWhiteSpace(Parentesco))
        {
            ModelState.AddModelError(
                nameof(Parentesco),
                "Indica el parentesco o relación.");
        }
        else if (Parentesco.Length > 100)
        {
            ModelState.AddModelError(
                nameof(Parentesco),
                "El parentesco no puede superar 100 caracteres.");
        }

        if (!ModelState.IsValid)
        {
            await CargarAcompanantesAsync(id);
            return Page();
        }

        var duplicado = await _context.AcompanantesViaje
            .AnyAsync(a =>
                a.FormularioVisaId == formulario.FormularioVisaId &&
                a.NombreCompleto == NombreCompleto &&
                a.Parentesco == Parentesco);

        if (duplicado)
        {
            ModelState.AddModelError(
                string.Empty,
                "Este acompañante ya está registrado.");

            await CargarAcompanantesAsync(id);
            return Page();
        }

        var acompanante = new AcompananteViaje
        {
            FormularioVisaId = formulario.FormularioVisaId,
            NombreCompleto = NombreCompleto!,
            Parentesco = Parentesco!
        };

        _context.AcompanantesViaje.Add(acompanante);

        formulario.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        TempData["MensajeExito"] =
            "El acompañante se registró correctamente.";

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostEliminarAsync(
        int id,
        int acompananteId)
    {
        if (!await CargarTramiteAsync(id))
        {
            return NotFound();
        }

        var formulario = await ObtenerFormularioEditableAsync(
            id, false);

        if (formulario == null)
        {
            return BadRequest(
                "El formulario ya no permite modificaciones.");
        }

        var acompanante = await _context.AcompanantesViaje
            .FirstOrDefaultAsync(a =>
                a.AcompananteViajeId == acompananteId &&
                a.FormularioVisaId == formulario.FormularioVisaId);

        if (acompanante == null)
        {
            return NotFound();
        }

        _context.AcompanantesViaje.Remove(acompanante);

        formulario.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        TempData["MensajeExito"] =
            "El acompañante se eliminó correctamente.";

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
        {
            return false;
        }

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
        {
            return null;
        }

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

    private async Task CargarAcompanantesAsync(int id)
    {
        var formulario = await _context.FormulariosVisa
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.TramiteId == id);

        if (formulario == null)
        {
            return;
        }

        PuedeEditar = formulario.EstadoCaptura == "Borrador";

        Acompanantes = await _context.AcompanantesViaje
            .AsNoTracking()
            .Where(a =>
                a.FormularioVisaId == formulario.FormularioVisaId)
            .OrderBy(a => a.NombreCompleto)
            .Select(a => new AcompananteRegistrado
            {
                AcompananteViajeId = a.AcompananteViajeId,
                NombreCompleto = a.NombreCompleto,
                Parentesco = a.Parentesco
            })
            .ToListAsync();
    }
}
