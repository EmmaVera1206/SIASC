
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SIASC.Infrastructure.Data;
using SIASC.Infrastructure.Identity;

namespace SIASC.Web.Pages.Tramites;

[Authorize]
public class DetalleModel : PageModel
{
    private readonly SIASCDbContext _context;
    private readonly UserManager<UsuarioSistema> _userManager;

    public DetalleModel(
        SIASCDbContext context,
        UserManager<UsuarioSistema> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public DatosTramite? Tramite { get; set; }

    public List<MovimientoEstado> Historial { get; set; } = new();

    public bool PuedeCambiarEstado { get; set; }

    public bool EsTramiteVisa { get; set; }
    public bool TieneFormularioVisa { get; set; }

    public bool EsTramitePasaporte { get; set; }
    public bool TieneDatosPasaporte { get; set; }

    public int TotalDocumentosActivos { get; set; }

    public List<CitaResumen> Citas { get; set; } = new();
    public List<PendienteResumen> Pendientes { get; set; } = new();

    public int TotalCitas => Citas.Count;

    public int TotalPendientesAbiertos =>
        Pendientes.Count(p => p.Estado != "Completado");

    // Control financiero
    public List<PagoResumen> Pagos { get; set; } = new();

    public long TotalPagadoCentavos =>
        Pagos
            .Where(p => !p.Anulado)
            .Sum(p => p.MontoCentavos);

    public long SaldoPendienteCentavos =>
        Math.Max(
            0,
            (Tramite?.CostoTotalCentavos ?? 0L) - TotalPagadoCentavos);

    public decimal TotalPagado =>
        TotalPagadoCentavos / 100m;

    public decimal SaldoPendiente =>
        SaldoPendienteCentavos / 100m;

    public int TotalPagosVigentes =>
        Pagos.Count(p => !p.Anulado);

    public int TotalPagosAnulados =>
        Pagos.Count(p => p.Anulado);

    public string EstadoFinanciero
    {
        get
        {
            if (SaldoPendienteCentavos == 0)
            {
                return "Liquidado";
            }

            if (TotalPagadoCentavos > 0)
            {
                return "Parcial";
            }

            return "Sin pagos";
        }
    }

    public decimal PorcentajePagado
    {
        get
        {
            if (Tramite == null ||
                Tramite.CostoTotalCentavos <= 0)
            {
                return 100m;
            }

            decimal porcentaje =
                TotalPagadoCentavos * 100m /
                Tramite.CostoTotalCentavos;

            return Math.Clamp(porcentaje, 0m, 100m);
        }
    }

    public class DatosTramite
    {
        public int TramiteId { get; set; }
        public int ClienteId { get; set; }
        public string NombreCliente { get; set; } = string.Empty;
        public string TelefonoCliente { get; set; } = string.Empty;
        public string? CorreoCliente { get; set; }
        public string NombreServicio { get; set; } = string.Empty;
        public string CodigoServicio { get; set; } = string.Empty;
        public string NombreEstado { get; set; } = string.Empty;
        public string CodigoEstado { get; set; } = string.Empty;
        public int TipoServicioId { get; set; }
        public int EstadoActualId { get; set; }
        public bool EstadoEsFinal { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaUltimaActualizacion { get; set; }
        public DateTime? FechaCierre { get; set; }
        public long CostoTotalCentavos { get; set; }

        public decimal CostoTotal =>
            CostoTotalCentavos / 100m;

        public string? ObservacionesGenerales { get; set; }
        public string? MotivoCancelacion { get; set; }
    }

    public class MovimientoEstado
    {
        public int HistorialId { get; set; }
        public string? EstadoAnterior { get; set; }
        public string EstadoNuevo { get; set; } = string.Empty;
        public DateTime FechaCambio { get; set; }
        public string UsuarioId { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;
        public string? MotivoCambio { get; set; }
    }

    public class CitaResumen
    {
        public int CitaId { get; set; }
        public string TipoCita { get; set; } = string.Empty;
        public string EstadoCita { get; set; } = string.Empty;
        public DateTime? FechaTentativa { get; set; }
        public DateTime? FechaConfirmada { get; set; }
        public string? Lugar { get; set; }

        public DateTime? FechaProgramada =>
            FechaConfirmada ?? FechaTentativa;
    }

    public class PendienteResumen
    {
        public int PendienteId { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Prioridad { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime? FechaVencimiento { get; set; }

        public bool EstaVencido =>
            Estado != "Completado" &&
            FechaVencimiento.HasValue &&
            FechaVencimiento.Value < DateTime.Now;
    }

    public class PagoResumen
    {
        public int PagoId { get; set; }
        public long MontoCentavos { get; set; }
        public DateTime FechaPago { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
        public string TipoPago { get; set; } = string.Empty;
        public string? Referencia { get; set; }
        public string? Observaciones { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool Anulado { get; set; }
        public DateTime? FechaAnulacion { get; set; }
        public string? MotivoAnulacion { get; set; }

        public decimal Monto =>
            MontoCentavos / 100m;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Tramite = await _context.Tramites
            .AsNoTracking()
            .Where(t => t.TramiteId == id)
            .Select(t => new DatosTramite
            {
                TramiteId = t.TramiteId,
                ClienteId = t.ClienteId,
                NombreCliente = t.Cliente.NombreCompleto,
                TelefonoCliente = t.Cliente.Telefono,
                CorreoCliente = t.Cliente.CorreoElectronico,
                NombreServicio = t.TipoServicio.Nombre,
                CodigoServicio = t.TipoServicio.Codigo,
                NombreEstado = t.EstadoActual.Nombre,
                CodigoEstado = t.EstadoActual.Codigo,
                TipoServicioId = t.TipoServicioId,
                EstadoActualId = t.EstadoActualId,
                EstadoEsFinal = t.EstadoActual.EsFinal,
                FechaInicio = t.FechaInicio,
                FechaUltimaActualizacion = t.FechaUltimaActualizacion,
                FechaCierre = t.FechaCierre,
                CostoTotalCentavos = t.CostoTotalCentavos,
                ObservacionesGenerales = t.ObservacionesGenerales,
                MotivoCancelacion = t.MotivoCancelacion
            })
            .FirstOrDefaultAsync();

        if (Tramite == null)
        {
            return NotFound();
        }

        EsTramiteVisa =
            Tramite.CodigoServicio == "VISA_AMERICANA";

        EsTramitePasaporte =
            Tramite.CodigoServicio == "PASAPORTE_PRIMERA_VEZ" ||
            Tramite.CodigoServicio == "PASAPORTE_RENOVACION";

        if (EsTramiteVisa)
        {
            TieneFormularioVisa = await _context.FormulariosVisa
                .AsNoTracking()
                .AnyAsync(f => f.TramiteId == id);
        }

        if (EsTramitePasaporte)
        {
            TieneDatosPasaporte = await _context.DatosPasaporte
                .AsNoTracking()
                .AnyAsync(d => d.TramiteId == id);
        }

        TotalDocumentosActivos = await _context.Documentos
            .AsNoTracking()
            .CountAsync(d => d.TramiteId == id && d.Activo);

        // Citas
        Citas = await _context.Citas
            .AsNoTracking()
            .Where(c => c.TramiteId == id)
            .Select(c => new CitaResumen
            {
                CitaId = c.CitaId,
                TipoCita = c.TipoCita,
                EstadoCita = c.EstadoCita,
                FechaTentativa = c.FechaTentativa,
                FechaConfirmada = c.FechaConfirmada,
                Lugar = c.Lugar
            })
            .ToListAsync();

        Citas = Citas
            .OrderBy(c =>
                c.EstadoCita == "Realizada" ||
                c.EstadoCita == "Cancelada" ? 1 : 0)
            .ThenBy(c => c.FechaProgramada ?? DateTime.MaxValue)
            .ThenByDescending(c => c.CitaId)
            .ToList();

        // Pendientes
        Pendientes = await _context.Pendientes
            .AsNoTracking()
            .Where(p => p.TramiteId == id)
            .Select(p => new PendienteResumen
            {
                PendienteId = p.PendienteId,
                Titulo = p.Titulo,
                Prioridad = p.Prioridad,
                Estado = p.Estado,
                FechaVencimiento = p.FechaVencimiento
            })
            .ToListAsync();

        Pendientes = Pendientes
            .OrderBy(p => p.Estado == "Completado" ? 1 : 0)
            .ThenBy(p => p.FechaVencimiento ?? DateTime.MaxValue)
            .ThenByDescending(p =>
                p.Prioridad == "Alta" ? 3 :
                p.Prioridad == "Media" ? 2 : 1)
            .ThenByDescending(p => p.PendienteId)
            .ToList();

        // Pagos del trámite
        Pagos = await _context.Pagos
            .AsNoTracking()
            .Where(p => p.TramiteId == id)
            .OrderByDescending(p => p.FechaPago)
            .ThenByDescending(p => p.PagoId)
            .Select(p => new PagoResumen
            {
                PagoId = p.PagoId,
                MontoCentavos = p.MontoCentavos,
                FechaPago = p.FechaPago,
                MetodoPago = p.MetodoPago,
                TipoPago = p.TipoPago,
                Referencia = p.Referencia,
                Observaciones = p.Observaciones,
                FechaRegistro = p.FechaRegistro,
                Anulado = p.Anulado,
                FechaAnulacion = p.FechaAnulacion,
                MotivoAnulacion = p.MotivoAnulacion
            })
            .ToListAsync();

        // Transiciones de estado
        if (!Tramite.EstadoEsFinal)
        {
            PuedeCambiarEstado = await _context.TransicionesEstado
                .AsNoTracking()
                .AnyAsync(t =>
                    t.TipoServicioId == Tramite.TipoServicioId &&
                    t.EstadoOrigenId == Tramite.EstadoActualId &&
                    t.Activo &&
                    t.EstadoDestino.Activo &&
                    _context.EstadosPorServicio.Any(e =>
                        e.TipoServicioId == Tramite.TipoServicioId &&
                        e.EstadoId == t.EstadoDestinoId));
        }

        // Historial de estados
        Historial = await _context.HistorialEstados
            .AsNoTracking()
            .Where(h => h.TramiteId == id)
            .OrderByDescending(h => h.FechaCambio)
            .ThenByDescending(h => h.HistorialId)
            .Select(h => new MovimientoEstado
            {
                HistorialId = h.HistorialId,
                EstadoAnterior = h.EstadoAnterior != null
                    ? h.EstadoAnterior.Nombre
                    : null,
                EstadoNuevo = h.EstadoNuevo.Nombre,
                FechaCambio = h.FechaCambio,
                UsuarioId = h.UsuarioId,
                MotivoCambio = h.MotivoCambio
            })
            .ToListAsync();

        var usuariosIds = Historial
            .Select(h => h.UsuarioId)
            .Where(usuarioId =>
                !string.IsNullOrWhiteSpace(usuarioId))
            .Distinct()
            .ToList();

        var usuarios = await _userManager.Users
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

        foreach (var movimiento in Historial)
        {
            movimiento.NombreUsuario =
                nombresUsuarios.TryGetValue(
                    movimiento.UsuarioId,
                    out var nombre)
                    ? nombre
                    : "Usuario no disponible";
        }

        return Page();
    }
}
