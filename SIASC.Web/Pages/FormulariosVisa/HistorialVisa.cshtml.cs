
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SIASC.Domain.Entities;
using SIASC.Infrastructure.Data;

namespace SIASC.Web.Pages.FormulariosVisa;

[Authorize]
public class HistorialVisaModel : PageModel
{
    private readonly SIASCDbContext _context;

    public HistorialVisaModel(SIASCDbContext context)
    {
        _context = context;
    }

    public int TramiteId { get; set; }

    public string NombreCliente { get; set; } = string.Empty;

    public bool FormularioExistente { get; set; }

    public bool PuedeEditar { get; set; } = true;

    public List<VisitaRegistrada> Visitas { get; set; } = new();

    [BindProperty]
    public DatosEntrada Input { get; set; } = new();

    [BindProperty]
    public NuevaVisitaEntrada NuevaVisita { get; set; } = new();

    public class DatosEntrada
    {
        [Display(Name = "¿Ha visitado Estados Unidos?")]
        public bool? HaVisitadoEstadosUnidos { get; set; }

        [Display(Name = "¿Ha tenido visa estadounidense?")]
        public bool? HaTenidoVisa { get; set; }

        [Display(Name = "¿Le han negado una visa?")]
        public bool? VisaNegada { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de la negativa")]
        public DateTime? FechaNegativa { get; set; }

        [StringLength(2000)]
        [Display(Name = "Detalles de la negativa")]
        public string? DetalleNegativa { get; set; }

        [Display(Name = "¿Le han cancelado una visa?")]
        public bool? VisaCancelada { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de cancelación")]
        public DateTime? FechaCancelacionVisa { get; set; }

        [StringLength(2000)]
        [Display(Name = "Detalles de la cancelación")]
        public string? DetalleCancelacionVisa { get; set; }

        [Display(Name = "¿Ha extraviado o le han robado una visa?")]
        public bool? VisaExtraviadaORobada { get; set; }

        [StringLength(2000)]
        [Display(Name = "Detalles del extravío o robo")]
        public string? DetalleExtravíoVisa { get; set; }
    }

    public class NuevaVisitaEntrada
    {
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de visita")]
        public DateTime? FechaVisita { get; set; }

        [StringLength(100)]
        [Display(Name = "Duración de estancia")]
        public string? DuracionEstancia { get; set; }
    }

    public class VisitaRegistrada
    {
        public int VisitaAnteriorEEUUId { get; set; }

        public DateTime? FechaVisita { get; set; }

        public string? DuracionEstancia { get; set; }
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
            FormularioExistente = true;

            PuedeEditar = formulario.EstadoCaptura == "Borrador";

            Input = new DatosEntrada
            {
                HaVisitadoEstadosUnidos =
                    formulario.HaVisitadoEstadosUnidos,

                HaTenidoVisa = formulario.HaTenidoVisa,
                VisaNegada = formulario.VisaNegada,
                FechaNegativa = formulario.FechaNegativa,
                DetalleNegativa = formulario.DetalleNegativa,

                VisaCancelada = formulario.VisaCancelada,

                FechaCancelacionVisa =
                    formulario.FechaCancelacionVisa,

                DetalleCancelacionVisa =
                    formulario.DetalleCancelacionVisa,

                VisaExtraviadaORobada =
                    formulario.VisaExtraviadaORobada,

                DetalleExtravíoVisa =
                    formulario.DetalleExtravíoVisa
            };
        }

        await CargarVisitasAsync(id);

        return Page();
    }

    public async Task<IActionResult> OnPostGuardarAsync(int id)
    {
        if (!await CargarTramiteAsync(id))
        {
            return NotFound();
        }

        ModelState.Remove("NuevaVisita.FechaVisita");
        ModelState.Remove("NuevaVisita.DuracionEstancia");

        if (!ModelState.IsValid)
        {
            await CargarVisitasAsync(id);
            return Page();
        }

        var formulario = await _context.FormulariosVisa
            .FirstOrDefaultAsync(f => f.TramiteId == id);

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
        else if (formulario.EstadoCaptura != "Borrador")
        {
            ModelState.AddModelError(
                string.Empty,
                "El formulario ya no está en borrador.");

            PuedeEditar = false;
            await CargarVisitasAsync(id);

            return Page();
        }

        formulario.HaVisitadoEstadosUnidos =
            Input.HaVisitadoEstadosUnidos;

        formulario.HaTenidoVisa = Input.HaTenidoVisa;

        formulario.VisaNegada = Input.VisaNegada;

        formulario.FechaNegativa =
            Input.VisaNegada == true
                ? Input.FechaNegativa
                : null;

        formulario.DetalleNegativa =
            Input.VisaNegada == true
                ? Limpiar(Input.DetalleNegativa)
                : null;

        formulario.VisaCancelada = Input.VisaCancelada;

        formulario.FechaCancelacionVisa =
            Input.VisaCancelada == true
                ? Input.FechaCancelacionVisa
                : null;

        formulario.DetalleCancelacionVisa =
            Input.VisaCancelada == true
                ? Limpiar(Input.DetalleCancelacionVisa)
                : null;

        formulario.VisaExtraviadaORobada =
            Input.VisaExtraviadaORobada;

        formulario.DetalleExtravíoVisa =
            Input.VisaExtraviadaORobada == true
                ? Limpiar(Input.DetalleExtravíoVisa)
                : null;

        formulario.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        TempData["MensajeExito"] =
            "El historial de visas se guardó correctamente.";

        return RedirectToPage(
            "/FormulariosVisa/HistorialVisa",
            new { id });
    }

    public async Task<IActionResult> OnPostAgregarVisitaAsync(int id)
    {
        if (!await CargarTramiteAsync(id))
        {
            return NotFound();
        }

        ModelState.Clear();

        if (!NuevaVisita.FechaVisita.HasValue)
        {
            ModelState.AddModelError(
                "NuevaVisita.FechaVisita",
                "Selecciona la fecha de la visita.");
        }

        if (NuevaVisita.FechaVisita > DateTime.Today)
        {
            ModelState.AddModelError(
                "NuevaVisita.FechaVisita",
                "La visita no puede tener una fecha futura.");
        }

        if (string.IsNullOrWhiteSpace(NuevaVisita.DuracionEstancia))
        {
            ModelState.AddModelError(
                "NuevaVisita.DuracionEstancia",
                "Indica la duración de la estancia.");
        }
        else if (NuevaVisita.DuracionEstancia.Length > 100)
        {
            ModelState.AddModelError(
                "NuevaVisita.DuracionEstancia",
                "La duración no puede superar 100 caracteres.");
        }

        var formulario = await _context.FormulariosVisa
            .FirstOrDefaultAsync(f => f.TramiteId == id);

        if (formulario != null &&
            formulario.EstadoCaptura != "Borrador")
        {
            return BadRequest(
                "El formulario ya no permite modificaciones.");
        }

        if (!ModelState.IsValid)
        {
            if (formulario != null)
            {
                FormularioExistente = true;
                await CargarDatosFormularioAsync(formulario);
            }

            await CargarVisitasAsync(id);
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

        var visita = new VisitaAnteriorEEUU
        {
            FormularioVisaId = formulario.FormularioVisaId,
            FechaVisita = NuevaVisita.FechaVisita,
            DuracionEstancia =
                NuevaVisita.DuracionEstancia!.Trim()
        };

        _context.VisitasAnterioresEEUU.Add(visita);

        formulario.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        TempData["MensajeExito"] =
            "La visita anterior se registró correctamente.";

        return RedirectToPage(
            "/FormulariosVisa/HistorialVisa",
            new { id });
    }

    public async Task<IActionResult> OnPostEliminarVisitaAsync(
        int id,
        int visitaId)
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

        var visita = await _context.VisitasAnterioresEEUU
            .FirstOrDefaultAsync(v =>
                v.VisitaAnteriorEEUUId == visitaId &&
                v.FormularioVisaId == formulario.FormularioVisaId);

        if (visita == null)
        {
            return NotFound();
        }

        _context.VisitasAnterioresEEUU.Remove(visita);

        formulario.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        TempData["MensajeExito"] =
            "La visita se eliminó correctamente.";

        return RedirectToPage(
            "/FormulariosVisa/HistorialVisa",
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

    private async Task CargarVisitasAsync(int id)
    {
        Visitas = await _context.VisitasAnterioresEEUU
            .AsNoTracking()
            .Where(v => v.FormularioVisa.TramiteId == id)
            .OrderByDescending(v => v.FechaVisita)
            .Select(v => new VisitaRegistrada
            {
                VisitaAnteriorEEUUId = v.VisitaAnteriorEEUUId,
                FechaVisita = v.FechaVisita,
                DuracionEstancia = v.DuracionEstancia
            })
            .ToListAsync();
    }

    private Task CargarDatosFormularioAsync(FormularioVisa formulario)
    {
        Input = new DatosEntrada
        {
            HaVisitadoEstadosUnidos =
                formulario.HaVisitadoEstadosUnidos,

            HaTenidoVisa = formulario.HaTenidoVisa,
            VisaNegada = formulario.VisaNegada,
            FechaNegativa = formulario.FechaNegativa,
            DetalleNegativa = formulario.DetalleNegativa,
            VisaCancelada = formulario.VisaCancelada,

            FechaCancelacionVisa =
                formulario.FechaCancelacionVisa,

            DetalleCancelacionVisa =
                formulario.DetalleCancelacionVisa,

            VisaExtraviadaORobada =
                formulario.VisaExtraviadaORobada,

            DetalleExtravíoVisa =
                formulario.DetalleExtravíoVisa
        };

        return Task.CompletedTask;
    }

    private static string? Limpiar(string? valor)
    {
        return string.IsNullOrWhiteSpace(valor)
            ? null
            : valor.Trim();
    }
}
