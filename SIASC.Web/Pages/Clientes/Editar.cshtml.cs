
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SIASC.Infrastructure.Data;

namespace SIASC.Web.Pages.Clientes;

[Authorize]
public class EditarModel : PageModel
{
    private readonly SIASCDbContext _context;

    public EditarModel(SIASCDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public DatosCliente Input { get; set; } = new();

    public int ClienteId { get; set; }

    public class DatosCliente
    {
        [Required(ErrorMessage = "El nombre completo es obligatorio.")]
        [StringLength(200, ErrorMessage =
            "El nombre no puede superar los 200 caracteres.")]
        [Display(Name = "Nombre completo")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [StringLength(20, MinimumLength = 10,
            ErrorMessage = "El teléfono debe tener entre 10 y 20 caracteres.")]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage =
            "Ingresa un correo electrónico válido.")]
        [StringLength(200)]
        [Display(Name = "Correo electrónico")]
        public string? CorreoElectronico { get; set; }

        [StringLength(200)]
        [Display(Name = "Recomendado por")]
        public string? RecomendadoPor { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var cliente = await _context.Clientes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.ClienteId == id);

        if (cliente == null)
        {
            return NotFound();
        }

        ClienteId = cliente.ClienteId;

        Input = new DatosCliente
        {
            NombreCompleto = cliente.NombreCompleto,
            Telefono = cliente.Telefono,
            CorreoElectronico = cliente.CorreoElectronico,
            RecomendadoPor = cliente.RecomendadoPor
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        ClienteId = id;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var cliente = await _context.Clientes
            .FirstOrDefaultAsync(c => c.ClienteId == id);

        if (cliente == null)
        {
            return NotFound();
        }

        var nombre = Input.NombreCompleto.Trim();
        var telefono = Input.Telefono.Trim();

        if (string.IsNullOrWhiteSpace(nombre))
        {
            ModelState.AddModelError(
                "Input.NombreCompleto",
                "El nombre completo es obligatorio.");

            return Page();
        }

        if (string.IsNullOrWhiteSpace(telefono))
        {
            ModelState.AddModelError(
                "Input.Telefono",
                "El teléfono es obligatorio.");

            return Page();
        }

        cliente.NombreCompleto = nombre;
        cliente.Telefono = telefono;

        cliente.CorreoElectronico =
            string.IsNullOrWhiteSpace(Input.CorreoElectronico)
                ? null
                : Input.CorreoElectronico.Trim();

        cliente.RecomendadoPor =
            string.IsNullOrWhiteSpace(Input.RecomendadoPor)
                ? null
                : Input.RecomendadoPor.Trim();

        cliente.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        TempData["MensajeExito"] =
            "Los datos del cliente se actualizaron correctamente.";

        return RedirectToPage("/Clientes/Index");
    }
}
