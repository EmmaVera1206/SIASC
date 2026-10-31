using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SIASC.Domain.Entities;
using SIASC.Infrastructure.Data;

namespace SIASC.Web.Pages.Pasaportes;

[Authorize]
public class DatosModel : PageModel
{
    private readonly SIASCDbContext _context;

    private static readonly Regex PatronCurp = new(
        @"^[A-Z][AEIOUX][A-Z]{2}\d{2}(0[1-9]|1[0-2])" +
        @"(0[1-9]|[12]\d|3[01])[HM]" +
        @"(AS|BC|BS|CC|CL|CM|CS|CH|DF|DG|GT|GR|HG|JC|" +
        @"MC|MN|MS|NT|NL|OC|PL|QT|QR|SP|SL|SR|TC|TS|" +
        @"TL|VZ|YN|ZS|NE)[B-DF-HJ-NP-TV-Z]{3}[A-Z0-9]\d$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public DatosModel(SIASCDbContext context)
    {
        _context = context;
    }

    public int TramiteId { get; private set; }
    public string NombreCliente { get; private set; } = string.Empty;
    public string NombreServicio { get; private set; } = string.Empty;
    public bool TieneRegistro { get; private set; }
    public bool CURPVerificada { get; private set; }
    public DateTime? FechaVerificacion { get; private set; }

    [BindProperty]
    public DatosEntrada Input { get; set; } = new();

    public class DatosEntrada
    {
        [Display(Name = "CURP")]
        [Required(ErrorMessage = "La CURP es obligatoria.")]
        [StringLength(18, MinimumLength = 18,
            ErrorMessage = "La CURP debe contener exactamente 18 caracteres.")]
        public string CURP { get; set; } = string.Empty;

        [Display(Name = "Observaciones")]
        [StringLength(2000,
            ErrorMessage = "Las observaciones no pueden superar 2000 caracteres.")]
        public string? Observaciones { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        if (!await CargarTramiteAsync(id))
            return NotFound();

        await CargarDatosAsync(id);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!await CargarTramiteAsync(id))
            return NotFound();

        Input.CURP = (Input.CURP ?? string.Empty).Trim().ToUpperInvariant();
        Input.Observaciones = string.IsNullOrWhiteSpace(Input.Observaciones)
            ? null
            : Input.Observaciones.Trim();

        ModelState.Clear();

        if (string.IsNullOrWhiteSpace(Input.CURP))
        {
            ModelState.AddModelError("Input.CURP", "La CURP es obligatoria.");
        }
        else if (Input.CURP.Length != 18)
        {
            ModelState.AddModelError("Input.CURP",
                "La CURP debe contener exactamente 18 caracteres.");
        }
        else if (!PatronCurp.IsMatch(Input.CURP))
        {
            ModelState.AddModelError("Input.CURP",
                "La CURP no cumple con el formato esperado.");
        }

        if (Input.Observaciones?.Length > 2000)
        {
            ModelState.AddModelError("Input.Observaciones",
                "Las observaciones no pueden superar 2000 caracteres.");
        }

        var datos = await _context.DatosPasaporte
            .FirstOrDefaultAsync(d => d.TramiteId == id);

        if (datos != null)
        {
            TieneRegistro = true;
            CURPVerificada = datos.CURPVerificada;
            FechaVerificacion = datos.FechaVerificacion;
        }

        if (!ModelState.IsValid)
            return Page();

        if (datos == null)
        {
            datos = new DatosPasaporte
            {
                TramiteId = id,
                CURP = Input.CURP,
                CURPVerificada = false,
                FechaVerificacion = null,
                Observaciones = Input.Observaciones
            };
            _context.DatosPasaporte.Add(datos);
        }
        else
        {
            if (!string.Equals(datos.CURP, Input.CURP, StringComparison.Ordinal))
            {
                datos.CURPVerificada = false;
                datos.FechaVerificacion = null;
            }

            datos.CURP = Input.CURP;
            datos.Observaciones = Input.Observaciones;
        }

        await _context.SaveChangesAsync();
        TempData["MensajeExito"] = "Los datos del pasaporte se guardaron correctamente.";
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostVerificarAsync(int id, string curpConfirmada)
    {
        if (!await CargarTramiteAsync(id))
            return NotFound();

        var datos = await _context.DatosPasaporte
            .FirstOrDefaultAsync(d => d.TramiteId == id);

        if (datos == null)
        {
            TempData["MensajeError"] = "Primero debes guardar una CURP.";
            return RedirectToPage(new { id });
        }

        if (!string.Equals(datos.CURP, curpConfirmada, StringComparison.Ordinal))
        {
            TempData["MensajeError"] =
                "La CURP cambió desde que abriste la página. Revisa los datos antes de verificar.";
            return RedirectToPage(new { id });
        }

        if (datos.CURPVerificada)
        {
            TempData["MensajeError"] = "Esta CURP ya está marcada como verificada.";
            return RedirectToPage(new { id });
        }

        datos.CURPVerificada = true;
        datos.FechaVerificacion = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        TempData["MensajeExito"] = "La CURP se marcó como verificada administrativamente.";
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostRetirarVerificacionAsync(int id, string curpConfirmada)
    {
        if (!await CargarTramiteAsync(id))
            return NotFound();

        var datos = await _context.DatosPasaporte
            .FirstOrDefaultAsync(d => d.TramiteId == id);

        if (datos == null)
        {
            TempData["MensajeError"] = "No existe un registro de pasaporte para este trámite.";
            return RedirectToPage(new { id });
        }

        if (!string.Equals(datos.CURP, curpConfirmada, StringComparison.Ordinal))
        {
            TempData["MensajeError"] =
                "La CURP cambió desde que abriste la página. Actualiza la pantalla e inténtalo nuevamente.";
            return RedirectToPage(new { id });
        }

        if (!datos.CURPVerificada)
        {
            TempData["MensajeError"] = "Esta CURP ya se encuentra sin verificar.";
            return RedirectToPage(new { id });
        }

        datos.CURPVerificada = false;
        datos.FechaVerificacion = null;
        await _context.SaveChangesAsync();

        TempData["MensajeExito"] = "Se retiró la verificación administrativa de la CURP.";
        return RedirectToPage(new { id });
    }

    private async Task CargarDatosAsync(int id)
    {
        var datos = await _context.DatosPasaporte.AsNoTracking()
            .FirstOrDefaultAsync(d => d.TramiteId == id);

        if (datos == null)
            return;

        TieneRegistro = true;
        CURPVerificada = datos.CURPVerificada;
        FechaVerificacion = datos.FechaVerificacion;
        Input = new DatosEntrada
        {
            CURP = datos.CURP,
            Observaciones = datos.Observaciones
        };
    }

    private async Task<bool> CargarTramiteAsync(int id)
    {
        var tramite = await _context.Tramites.AsNoTracking()
            .Where(t => t.TramiteId == id &&
                (t.TipoServicio.Codigo == "PASAPORTE_PRIMERA_VEZ" ||
                 t.TipoServicio.Codigo == "PASAPORTE_RENOVACION"))
            .Select(t => new
            {
                t.TramiteId,
                NombreCliente = t.Cliente.NombreCompleto,
                NombreServicio = t.TipoServicio.Nombre
            })
            .FirstOrDefaultAsync();

        if (tramite == null)
            return false;

        TramiteId = tramite.TramiteId;
        NombreCliente = tramite.NombreCliente;
        NombreServicio = tramite.NombreServicio;
        return true;
    }
}
