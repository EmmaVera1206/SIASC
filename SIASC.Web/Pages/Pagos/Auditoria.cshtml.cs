
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SIASC.Infrastructure.Data;

namespace SIASC.Web.Pages.Pagos;

[Authorize]
public class AuditoriaModel : PageModel
{
    private readonly SIASCDbContext _context;

    public AuditoriaModel(SIASCDbContext context)
    {
        _context = context;
    }

    public List<MovimientoAuditoria> Movimientos { get; set; } = new();

    public int? TramiteId { get; set; }

    public int TotalMovimientos => Movimientos.Count;

    public int TotalAnulaciones =>
        Movimientos.Count(m => m.TipoCambio == "Anulacion");

    public class MovimientoAuditoria
    {
        public int CambioPagoId { get; set; }

        public int PagoId { get; set; }

        public int TramiteId { get; set; }

        public string NombreCliente { get; set; } = string.Empty;

        public string NombreServicio { get; set; } = string.Empty;

        public string TipoCambio { get; set; } = string.Empty;

        public string NombreUsuario { get; set; } = string.Empty;

        public DateTime FechaCambio { get; set; }

        public string Motivo { get; set; } = string.Empty;

        public string DatosAnteriores { get; set; } = string.Empty;

        public string? DatosNuevos { get; set; }

        public long MontoCentavos { get; set; }

        public decimal Monto => MontoCentavos / 100m;
    }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        TramiteId = id;

        if (id.HasValue)
        {
            bool existeTramite = await _context.Tramites
                .AsNoTracking()
                .AnyAsync(t => t.TramiteId == id.Value);

            if (!existeTramite)
            {
                return NotFound();
            }
        }

        var consulta = _context.HistorialCambiosPagos
            .AsNoTracking()
            .AsQueryable();

        if (id.HasValue)
        {
            consulta = consulta.Where(h =>
                h.Pago.TramiteId == id.Value);
        }

        var registros = await consulta
            .OrderByDescending(h => h.FechaCambio)
            .ThenByDescending(h => h.CambioPagoId)
            .Select(h => new
            {
                h.CambioPagoId,
                h.PagoId,
                h.Pago.TramiteId,
                NombreCliente = h.Pago.Tramite.Cliente.NombreCompleto,
                NombreServicio = h.Pago.Tramite.TipoServicio.Nombre,
                h.UsuarioId,
                h.TipoCambio,
                h.FechaCambio,
                h.Motivo,
                h.DatosAnteriores,
                h.DatosNuevos,
                h.Pago.MontoCentavos
            })
            .ToListAsync();

        var usuariosIds = registros
            .Select(r => r.UsuarioId)
            .Where(usuarioId =>
                !string.IsNullOrWhiteSpace(usuarioId))
            .Distinct()
            .ToList();

        var usuarios = await _context.Users
            .AsNoTracking()
            .Where(u => usuariosIds.Contains(u.Id))
            .Select(u => new
            {
                u.Id,
                u.NombreCompleto,
                u.UserName
            })
            .ToListAsync();

        var nombresUsuarios = usuarios.ToDictionary(
            u => u.Id,
            u => !string.IsNullOrWhiteSpace(u.NombreCompleto)
                ? u.NombreCompleto
                : (u.UserName ?? u.Id));

        Movimientos = registros.Select(r => new MovimientoAuditoria
        {
            CambioPagoId = r.CambioPagoId,
            PagoId = r.PagoId,
            TramiteId = r.TramiteId,
            NombreCliente = r.NombreCliente,
            NombreServicio = r.NombreServicio,
            TipoCambio = r.TipoCambio,
            NombreUsuario = nombresUsuarios.TryGetValue(
                r.UsuarioId,
                out var nombre)
                ? nombre
                : "Usuario no disponible",
            FechaCambio = r.FechaCambio,
            Motivo = r.Motivo,
            DatosAnteriores = FormatearJson(r.DatosAnteriores),
            DatosNuevos = FormatearJson(r.DatosNuevos),
            MontoCentavos = r.MontoCentavos
        }).ToList();

        return Page();
    }

    private static string FormatearJson(string? contenido)
    {
        if (string.IsNullOrWhiteSpace(contenido))
        {
            return "Sin información";
        }

        try
        {
            using var documento = JsonDocument.Parse(contenido);

            return JsonSerializer.Serialize(
                documento.RootElement,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });
        }
        catch (JsonException)
        {
            return contenido;
        }
    }
}
