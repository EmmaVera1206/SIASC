
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SIASC.Domain.Entities;
using SIASC.Infrastructure.Data;

namespace SIASC.Web.Pages.Clientes;

[Authorize]
public class CrearModel : PageModel
{
    private readonly SIASCDbContext _context;

    public CrearModel(SIASCDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public DatosCliente Input { get; set; } = new();

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

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
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

        var fechaActual = DateTime.UtcNow;

        var cliente = new Cliente
        {
            NombreCompleto = nombre,
            Telefono = telefono,
            CorreoElectronico =
                string.IsNullOrWhiteSpace(Input.CorreoElectronico)
                    ? null
                    : Input.CorreoElectronico.Trim(),

            RecomendadoPor =
                string.IsNullOrWhiteSpace(Input.RecomendadoPor)
                    ? null
                    : Input.RecomendadoPor.Trim(),

            FechaRegistro = fechaActual,
            FechaActualizacion = fechaActual,
            Activo = true
        };

        _context.Clientes.Add(cliente);

        await _context.SaveChangesAsync();

        TempData["MensajeExito"] =
            "El cliente se registró correctamente.";

        return RedirectToPage("/Clientes/Index");
    }
}

