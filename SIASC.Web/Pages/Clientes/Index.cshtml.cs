
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SIASC.Domain.Constants;
using SIASC.Infrastructure.Data;

namespace SIASC.Web.Pages.Clientes;

[Authorize]
public class IndexModel : PageModel
{
    private readonly SIASCDbContext _context;

    public IndexModel(SIASCDbContext context)
    {
        _context = context;
    }

    [BindProperty(SupportsGet = true)]
    public string? Busqueda { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool MostrarInactivos { get; set; }

    public List<ClienteListado> Clientes { get; set; } = new();

    public int TotalClientes { get; set; }

    public int ClientesActivos { get; set; }

    public class ClienteListado
    {
        public int ClienteId { get; set; }

        public string NombreCompleto { get; set; } = string.Empty;

        public string Telefono { get; set; } = string.Empty;

        public string? CorreoElectronico { get; set; }

        public string? RecomendadoPor { get; set; }

        public DateTime FechaRegistro { get; set; }

        public bool Activo { get; set; }

        public int NumeroTramites { get; set; }
    }

    public async Task OnGetAsync()
    {
        TotalClientes = await _context.Clientes
            .AsNoTracking()
            .CountAsync();

        ClientesActivos = await _context.Clientes
            .AsNoTracking()
            .CountAsync(c => c.Activo);

        var consulta = _context.Clientes
            .AsNoTracking()
            .AsQueryable();

        if (!MostrarInactivos)
        {
            consulta = consulta.Where(c => c.Activo);
        }

        if (!string.IsNullOrWhiteSpace(Busqueda))
        {
            var texto = Busqueda.Trim();

            consulta = consulta.Where(c =>
                c.NombreCompleto.Contains(texto) ||
                c.Telefono.Contains(texto) ||
                (c.CorreoElectronico != null &&
                 c.CorreoElectronico.Contains(texto)));
        }

        Clientes = await consulta
            .OrderBy(c => c.NombreCompleto)
            .Select(c => new ClienteListado
            {
                ClienteId = c.ClienteId,
                NombreCompleto = c.NombreCompleto,
                Telefono = c.Telefono,
                CorreoElectronico = c.CorreoElectronico,
                RecomendadoPor = c.RecomendadoPor,
                FechaRegistro = c.FechaRegistro,
                Activo = c.Activo,
                NumeroTramites = c.Tramites.Count()
            })
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostCambiarEstadoAsync(
        int id,
        bool activar)
    {
        if (!User.IsInRole(RolesSistema.Administrador))
        {
            return Forbid();
        }

        var cliente = await _context.Clientes
            .FirstOrDefaultAsync(c => c.ClienteId == id);

        if (cliente == null)
        {
            return NotFound();
        }

        if (cliente.Activo == activar)
        {
            TempData["MensajeExito"] =
                "El cliente ya tiene el estado solicitado.";

            return RedirectToPage();
        }

        cliente.Activo = activar;
        cliente.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        TempData["MensajeExito"] = activar
            ? "El cliente se reactivó correctamente."
            : "El cliente se desactivó correctamente.";

        return RedirectToPage(
            pageName: null,
            pageHandler: null,
            routeValues: new { MostrarInactivos = true },
            fragment: null);
    }
}
