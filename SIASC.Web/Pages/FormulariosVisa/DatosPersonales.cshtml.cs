
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SIASC.Domain.Entities;
using SIASC.Infrastructure.Data;

namespace SIASC.Web.Pages.FormulariosVisa;

[Authorize]
public class DatosPersonalesModel : PageModel
{
    private readonly SIASCDbContext _context;

    public DatosPersonalesModel(SIASCDbContext context)
    {
        _context = context;
    }

    public int TramiteId { get; set; }

    public string NombreCliente { get; set; } = string.Empty;

    public string NombreServicio { get; set; } = string.Empty;

    public bool FormularioExistente { get; set; }

    [BindProperty]
    public DatosEntrada Input { get; set; } = new();

    public class DatosEntrada
    {
        [StringLength(200)]
        [Display(Name = "Nombre según pasaporte")]
        public string? NombreSegunPasaporte { get; set; }

        [StringLength(30)]
        [Display(Name = "Sexo")]
        public string? Sexo { get; set; }

        [StringLength(50)]
        [Display(Name = "Estado civil")]
        public string? EstadoCivil { get; set; }

        [StringLength(100)]
        [Display(Name = "Nacionalidad")]
        public string? Nacionalidad { get; set; }

        [Display(Name = "¿Tiene otra nacionalidad?")]
        public bool? TieneOtraNacionalidad { get; set; }

        [StringLength(100)]
        [Display(Name = "Otra nacionalidad")]
        public string? OtraNacionalidad { get; set; }

        [Display(Name = "¿Reside en otro país?")]
        public bool? ResideEnOtroPais { get; set; }

        [StringLength(100)]
        [Display(Name = "País de residencia")]
        public string? PaisResidencia { get; set; }

        [StringLength(500)]
        [Display(Name = "Domicilio particular")]
        public string? DomicilioParticular { get; set; }

        [StringLength(30)]
        [Display(Name = "Teléfono particular")]
        public string? TelefonoParticular { get; set; }

        [StringLength(30)]
        [Display(Name = "Teléfono adicional")]
        public string? TelefonoAdicional { get; set; }

        [StringLength(200)]
        [EmailAddress(ErrorMessage = "Ingresa un correo válido.")]
        [Display(Name = "Correo personal")]
        public string? CorreoPersonal { get; set; }
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
                NombreSegunPasaporte = formulario.NombreSegunPasaporte,
                Sexo = formulario.Sexo,
                EstadoCivil = formulario.EstadoCivil,
                Nacionalidad = formulario.Nacionalidad,
                TieneOtraNacionalidad = formulario.TieneOtraNacionalidad,
                OtraNacionalidad = formulario.OtraNacionalidad,
                ResideEnOtroPais = formulario.ResideEnOtroPais,
                PaisResidencia = formulario.PaisResidencia,
                DomicilioParticular = formulario.DomicilioParticular,
                TelefonoParticular = formulario.TelefonoParticular,
                TelefonoAdicional = formulario.TelefonoAdicional,
                CorreoPersonal = formulario.CorreoPersonal
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
                    "Este formulario ya no está en borrador y no puede editarse desde esta pantalla.");

                return Page();
            }
        }

        formulario.NombreSegunPasaporte =
            Limpiar(Input.NombreSegunPasaporte);

        formulario.Sexo = Limpiar(Input.Sexo);
        formulario.EstadoCivil = Limpiar(Input.EstadoCivil);
        formulario.Nacionalidad = Limpiar(Input.Nacionalidad);

        formulario.TieneOtraNacionalidad =
            Input.TieneOtraNacionalidad;

        formulario.OtraNacionalidad =
            Input.TieneOtraNacionalidad == true
                ? Limpiar(Input.OtraNacionalidad)
                : null;

        formulario.ResideEnOtroPais = Input.ResideEnOtroPais;

        formulario.PaisResidencia =
            Input.ResideEnOtroPais == true
                ? Limpiar(Input.PaisResidencia)
                : null;

        formulario.DomicilioParticular =
            Limpiar(Input.DomicilioParticular);

        formulario.TelefonoParticular =
            Limpiar(Input.TelefonoParticular);

        formulario.TelefonoAdicional =
            Limpiar(Input.TelefonoAdicional);

        formulario.CorreoPersonal =
            Limpiar(Input.CorreoPersonal);

        formulario.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        TempData["MensajeExito"] =
            "Los datos personales y de contacto se guardaron correctamente.";

        return RedirectToPage(
            "/FormulariosVisa/DatosPersonales",
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
                NombreCliente = t.Cliente.NombreCompleto,
                NombreServicio = t.TipoServicio.Nombre
            })
            .FirstOrDefaultAsync();

        if (tramite == null)
        {
            return false;
        }

        TramiteId = tramite.TramiteId;
        NombreCliente = tramite.NombreCliente;
        NombreServicio = tramite.NombreServicio;

        return true;
    }

    private static string? Limpiar(string? valor)
    {
        return string.IsNullOrWhiteSpace(valor)
            ? null
            : valor.Trim();
    }
}
