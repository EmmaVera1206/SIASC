
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SIASC.Domain.Entities;
using SIASC.Infrastructure.Data;

namespace SIASC.Web.Pages.FormulariosVisa;

[Authorize]
public class PlanesViajeModel : PageModel
{
    private readonly SIASCDbContext _context;

    public PlanesViajeModel(SIASCDbContext context)
    {
        _context = context;
    }

    public int TramiteId { get; set; }

    public string NombreCliente { get; set; } = string.Empty;

    public bool FormularioExistente { get; set; }

    [BindProperty]
    public DatosEntrada Input { get; set; } = new();

    public class DatosEntrada
    {
        [Display(Name = "¿Tiene planes de viaje definidos?")]
        public bool? TienePlanesViaje { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha estimada del viaje")]
        public DateTime? FechaViaje { get; set; }

        [StringLength(1000)]
        [Display(Name = "Motivo del viaje")]
        public string? MotivoViaje { get; set; }

        [StringLength(150)]
        [Display(Name = "Ciudad de destino")]
        public string? CiudadDestino { get; set; }

        [StringLength(500)]
        [Display(Name = "Hospedaje o dirección de estancia")]
        public string? Hospedaje { get; set; }

        [StringLength(200)]
        [Display(Name = "Empresa que visitará")]
        public string? EmpresaVisita { get; set; }

        [StringLength(500)]
        [Display(Name = "Dirección de la empresa")]
        public string? DireccionEmpresaVisita { get; set; }

        [StringLength(30)]
        [Display(Name = "Teléfono de la empresa")]
        public string? TelefonoEmpresaVisita { get; set; }

        [StringLength(200)]
        [EmailAddress(ErrorMessage = "Ingresa un correo válido.")]
        [Display(Name = "Correo de la empresa")]
        public string? CorreoEmpresaVisita { get; set; }

        [StringLength(100)]
        [Display(Name = "¿Quién cubrirá los gastos?")]
        public string? ResponsableGastos { get; set; }

        [StringLength(200)]
        [Display(Name = "Nombre del patrocinador")]
        public string? NombrePatrocinador { get; set; }

        [StringLength(100)]
        [Display(Name = "Relación con el patrocinador")]
        public string? ParentescoPatrocinador { get; set; }

        [StringLength(30)]
        [Display(Name = "Teléfono del patrocinador")]
        public string? TelefonoPatrocinador { get; set; }
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

            Input = new DatosEntrada
            {
                TienePlanesViaje = formulario.TienePlanesViaje,
                FechaViaje = formulario.FechaViaje,
                MotivoViaje = formulario.MotivoViaje,
                CiudadDestino = formulario.CiudadDestino,
                Hospedaje = formulario.Hospedaje,
                EmpresaVisita = formulario.EmpresaVisita,
                DireccionEmpresaVisita =
                    formulario.DireccionEmpresaVisita,
                TelefonoEmpresaVisita =
                    formulario.TelefonoEmpresaVisita,
                CorreoEmpresaVisita =
                    formulario.CorreoEmpresaVisita,
                ResponsableGastos = formulario.ResponsableGastos,
                NombrePatrocinador = formulario.NombrePatrocinador,
                ParentescoPatrocinador =
                    formulario.ParentescoPatrocinador,
                TelefonoPatrocinador =
                    formulario.TelefonoPatrocinador
            };
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!await CargarTramiteAsync(id))
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
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
        else
        {
            FormularioExistente = true;

            if (formulario.EstadoCaptura != "Borrador")
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Este formulario no está en borrador y no puede modificarse.");

                return Page();
            }
        }

        formulario.TienePlanesViaje = Input.TienePlanesViaje;
        formulario.FechaViaje = Input.FechaViaje;
        formulario.MotivoViaje = Limpiar(Input.MotivoViaje);
        formulario.CiudadDestino = Limpiar(Input.CiudadDestino);
        formulario.Hospedaje = Limpiar(Input.Hospedaje);
        formulario.EmpresaVisita = Limpiar(Input.EmpresaVisita);

        formulario.DireccionEmpresaVisita =
            Limpiar(Input.DireccionEmpresaVisita);

        formulario.TelefonoEmpresaVisita =
            Limpiar(Input.TelefonoEmpresaVisita);

        formulario.CorreoEmpresaVisita =
            Limpiar(Input.CorreoEmpresaVisita);

        formulario.ResponsableGastos =
            Limpiar(Input.ResponsableGastos);

        formulario.NombrePatrocinador =
            Limpiar(Input.NombrePatrocinador);

        formulario.ParentescoPatrocinador =
            Limpiar(Input.ParentescoPatrocinador);

        formulario.TelefonoPatrocinador =
            Limpiar(Input.TelefonoPatrocinador);

        formulario.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        TempData["MensajeExito"] =
            "Los planes y gastos del viaje se guardaron correctamente.";

        return RedirectToPage(
            "/FormulariosVisa/PlanesViaje",
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

    private static string? Limpiar(string? valor)
    {
        return string.IsNullOrWhiteSpace(valor)
            ? null
            : valor.Trim();
    }
}
