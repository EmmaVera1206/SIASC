
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SIASC.Infrastructure.Data;

namespace SIASC.Web.Pages.Citas;

[Authorize]
public class IndexModel : PageModel
{
    private readonly SIASCDbContext _context;

    public IndexModel(SIASCDbContext context)
    {
        _context = context;
    }

    public List<CitaListado> Citas { get; set; } = new();

    public class CitaListado
    {
        public int CitaId { get; set; }

        public int TramiteId { get; set; }

        public string NombreCliente { get; set; } = string.Empty;

        public string NombreServicio { get; set; } = string.Empty;

        public string TipoCita { get; set; } = string.Empty;

        public string EstadoCita { get; set; } = string.Empty;

        public DateTime? FechaTentativa { get; set; }

        public DateTime? FechaConfirmada { get; set; }

        public string? Lugar { get; set; }

        public DateTime FechaRegistro { get; set; }

        public DateTime? FechaProgramada =>
            FechaConfirmada ?? FechaTentativa;

        public bool PuedeEditar =>
            EstadoCita == "Tentativa" ||
            EstadoCita == "Confirmada";
    }

    public async Task OnGetAsync()
    {
        Citas = await _context.Citas
            .AsNoTracking()
            .Select(c => new CitaListado
            {
                CitaId = c.CitaId,
                TramiteId = c.TramiteId,
                NombreCliente = c.Tramite.Cliente.NombreCompleto,
                NombreServicio = c.Tramite.TipoServicio.Nombre,
                TipoCita = c.TipoCita,
                EstadoCita = c.EstadoCita,
                FechaTentativa = c.FechaTentativa,
                FechaConfirmada = c.FechaConfirmada,
                Lugar = c.Lugar,
                FechaRegistro = c.FechaRegistro
            })
            .ToListAsync();

        Citas = Citas
            .OrderBy(c => c.FechaProgramada ?? DateTime.MaxValue)
            .ThenByDescending(c => c.CitaId)
            .ToList();
    }
}
