using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIASC.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    NombreCompleto = table.Column<string>(type: "TEXT", nullable: false),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    SecurityStamp = table.Column<string>(type: "TEXT", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    ClienteId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NombreCompleto = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Telefono = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    CorreoElectronico = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    RecomendadoPor = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.ClienteId);
                });

            migrationBuilder.CreateTable(
                name: "EstadosTramite",
                columns: table => new
                {
                    EstadoId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Codigo = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    EsFinal = table.Column<bool>(type: "INTEGER", nullable: false),
                    OrdenVisual = table.Column<int>(type: "INTEGER", nullable: false),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosTramite", x => x.EstadoId);
                });

            migrationBuilder.CreateTable(
                name: "TiposServicio",
                columns: table => new
                {
                    TipoServicioId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Codigo = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposServicio", x => x.TipoServicioId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderKey = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EstadosPorServicio",
                columns: table => new
                {
                    TipoServicioId = table.Column<int>(type: "INTEGER", nullable: false),
                    EstadoId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosPorServicio", x => new { x.TipoServicioId, x.EstadoId });
                    table.ForeignKey(
                        name: "FK_EstadosPorServicio_EstadosTramite_EstadoId",
                        column: x => x.EstadoId,
                        principalTable: "EstadosTramite",
                        principalColumn: "EstadoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EstadosPorServicio_TiposServicio_TipoServicioId",
                        column: x => x.TipoServicioId,
                        principalTable: "TiposServicio",
                        principalColumn: "TipoServicioId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tramites",
                columns: table => new
                {
                    TramiteId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClienteId = table.Column<int>(type: "INTEGER", nullable: false),
                    TipoServicioId = table.Column<int>(type: "INTEGER", nullable: false),
                    EstadoActualId = table.Column<int>(type: "INTEGER", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaUltimaActualizacion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaCierre = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CostoTotalCentavos = table.Column<long>(type: "INTEGER", nullable: false),
                    ObservacionesGenerales = table.Column<string>(type: "TEXT", maxLength: 3000, nullable: true),
                    MotivoCancelacion = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tramites", x => x.TramiteId);
                    table.CheckConstraint("CK_Tramites_CostoNoNegativo", "CostoTotalCentavos >= 0");
                    table.ForeignKey(
                        name: "FK_Tramites_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "ClienteId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tramites_EstadosTramite_EstadoActualId",
                        column: x => x.EstadoActualId,
                        principalTable: "EstadosTramite",
                        principalColumn: "EstadoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tramites_TiposServicio_TipoServicioId",
                        column: x => x.TipoServicioId,
                        principalTable: "TiposServicio",
                        principalColumn: "TipoServicioId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TransicionesEstado",
                columns: table => new
                {
                    TransicionId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TipoServicioId = table.Column<int>(type: "INTEGER", nullable: false),
                    EstadoOrigenId = table.Column<int>(type: "INTEGER", nullable: false),
                    EstadoDestinoId = table.Column<int>(type: "INTEGER", nullable: false),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransicionesEstado", x => x.TransicionId);
                    table.CheckConstraint("CK_Transiciones_EstadosDiferentes", "EstadoOrigenId <> EstadoDestinoId");
                    table.ForeignKey(
                        name: "FK_TransicionesEstado_EstadosTramite_EstadoDestinoId",
                        column: x => x.EstadoDestinoId,
                        principalTable: "EstadosTramite",
                        principalColumn: "EstadoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransicionesEstado_EstadosTramite_EstadoOrigenId",
                        column: x => x.EstadoOrigenId,
                        principalTable: "EstadosTramite",
                        principalColumn: "EstadoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransicionesEstado_TiposServicio_TipoServicioId",
                        column: x => x.TipoServicioId,
                        principalTable: "TiposServicio",
                        principalColumn: "TipoServicioId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Citas",
                columns: table => new
                {
                    CitaId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TramiteId = table.Column<int>(type: "INTEGER", nullable: false),
                    TipoCita = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    FechaTentativa = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FechaConfirmada = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Lugar = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    EstadoCita = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    Observaciones = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Citas", x => x.CitaId);
                    table.ForeignKey(
                        name: "FK_Citas_Tramites_TramiteId",
                        column: x => x.TramiteId,
                        principalTable: "Tramites",
                        principalColumn: "TramiteId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DatosPasaporte",
                columns: table => new
                {
                    DatosPasaporteId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TramiteId = table.Column<int>(type: "INTEGER", nullable: false),
                    CURP = table.Column<string>(type: "TEXT", maxLength: 18, nullable: false),
                    CURPVerificada = table.Column<bool>(type: "INTEGER", nullable: false),
                    FechaVerificacion = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Observaciones = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatosPasaporte", x => x.DatosPasaporteId);
                    table.ForeignKey(
                        name: "FK_DatosPasaporte_Tramites_TramiteId",
                        column: x => x.TramiteId,
                        principalTable: "Tramites",
                        principalColumn: "TramiteId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Documentos",
                columns: table => new
                {
                    DocumentoId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TramiteId = table.Column<int>(type: "INTEGER", nullable: false),
                    TipoDocumento = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    NombreOriginal = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    NombreAlmacenado = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    TipoMime = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    TamanoBytes = table.Column<long>(type: "INTEGER", nullable: false),
                    HashSHA256 = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    FechaCarga = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Observaciones = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documentos", x => x.DocumentoId);
                    table.CheckConstraint("CK_Documentos_TamanoPositivo", "TamanoBytes > 0");
                    table.ForeignKey(
                        name: "FK_Documentos_Tramites_TramiteId",
                        column: x => x.TramiteId,
                        principalTable: "Tramites",
                        principalColumn: "TramiteId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FormulariosVisa",
                columns: table => new
                {
                    FormularioVisaId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TramiteId = table.Column<int>(type: "INTEGER", nullable: false),
                    NombreSegunPasaporte = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Sexo = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    EstadoCivil = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Nacionalidad = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    TieneOtraNacionalidad = table.Column<bool>(type: "INTEGER", nullable: true),
                    OtraNacionalidad = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ResideEnOtroPais = table.Column<bool>(type: "INTEGER", nullable: true),
                    PaisResidencia = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    TienePlanesViaje = table.Column<bool>(type: "INTEGER", nullable: true),
                    FechaViaje = table.Column<DateTime>(type: "TEXT", nullable: true),
                    MotivoViaje = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CiudadDestino = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    Hospedaje = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    EmpresaVisita = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    DireccionEmpresaVisita = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    TelefonoEmpresaVisita = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    CorreoEmpresaVisita = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    ResponsableGastos = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    NombrePatrocinador = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    ParentescoPatrocinador = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    TelefonoPatrocinador = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    HaVisitadoEstadosUnidos = table.Column<bool>(type: "INTEGER", nullable: true),
                    HaTenidoVisa = table.Column<bool>(type: "INTEGER", nullable: true),
                    VisaNegada = table.Column<bool>(type: "INTEGER", nullable: true),
                    FechaNegativa = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DetalleNegativa = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    VisaCancelada = table.Column<bool>(type: "INTEGER", nullable: true),
                    FechaCancelacionVisa = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DetalleCancelacionVisa = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    VisaExtraviadaORobada = table.Column<bool>(type: "INTEGER", nullable: true),
                    DetalleExtravíoVisa = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    DomicilioParticular = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    TelefonoParticular = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    TelefonoAdicional = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    CorreoPersonal = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    NombrePadre = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    FechaNacimientoPadre = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PadreEnEstadosUnidos = table.Column<bool>(type: "INTEGER", nullable: true),
                    NombreMadre = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    FechaNacimientoMadre = table.Column<DateTime>(type: "TEXT", nullable: true),
                    MadreEnEstadosUnidos = table.Column<bool>(type: "INTEGER", nullable: true),
                    TieneFamiliaresInmediatosEEUU = table.Column<bool>(type: "INTEGER", nullable: true),
                    TieneOtrosFamiliaresEEUU = table.Column<bool>(type: "INTEGER", nullable: true),
                    NombreConyuge = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    FechaNacimientoConyuge = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LugarNacimientoConyuge = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    FechaMatrimonio = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FechaDivorcio = table.Column<DateTime>(type: "TEXT", nullable: true),
                    MotivoDivorcio = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Ocupacion = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    NombreEmpresaActual = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    DireccionEmpresaActual = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    TelefonoEmpresaActual = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    PuestoActual = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    FechaInicioEmpleo = table.Column<DateTime>(type: "TEXT", nullable: true),
                    NombreInstitucion = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    DireccionInstitucion = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    TelefonoInstitucion = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    CarreraCurso = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    FechaInicioEstudios = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FechaFinEstudios = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EstadoCaptura = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false, defaultValue: "Borrador"),
                    FechaCreacion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaRevision = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormulariosVisa", x => x.FormularioVisaId);
                    table.ForeignKey(
                        name: "FK_FormulariosVisa_Tramites_TramiteId",
                        column: x => x.TramiteId,
                        principalTable: "Tramites",
                        principalColumn: "TramiteId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HistorialCostos",
                columns: table => new
                {
                    HistorialCostoId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TramiteId = table.Column<int>(type: "INTEGER", nullable: false),
                    CostoAnteriorCentavos = table.Column<long>(type: "INTEGER", nullable: false),
                    CostoNuevoCentavos = table.Column<long>(type: "INTEGER", nullable: false),
                    FechaCambio = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Motivo = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    UsuarioId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialCostos", x => x.HistorialCostoId);
                    table.CheckConstraint("CK_HistorialCostos_AnteriorNoNegativo", "CostoAnteriorCentavos >= 0");
                    table.CheckConstraint("CK_HistorialCostos_NuevoNoNegativo", "CostoNuevoCentavos >= 0");
                    table.ForeignKey(
                        name: "FK_HistorialCostos_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistorialCostos_Tramites_TramiteId",
                        column: x => x.TramiteId,
                        principalTable: "Tramites",
                        principalColumn: "TramiteId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HistorialEstados",
                columns: table => new
                {
                    HistorialId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TramiteId = table.Column<int>(type: "INTEGER", nullable: false),
                    EstadoAnteriorId = table.Column<int>(type: "INTEGER", nullable: true),
                    EstadoNuevoId = table.Column<int>(type: "INTEGER", nullable: false),
                    FechaCambio = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UsuarioId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    MotivoCambio = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialEstados", x => x.HistorialId);
                    table.ForeignKey(
                        name: "FK_HistorialEstados_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistorialEstados_EstadosTramite_EstadoAnteriorId",
                        column: x => x.EstadoAnteriorId,
                        principalTable: "EstadosTramite",
                        principalColumn: "EstadoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistorialEstados_EstadosTramite_EstadoNuevoId",
                        column: x => x.EstadoNuevoId,
                        principalTable: "EstadosTramite",
                        principalColumn: "EstadoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistorialEstados_Tramites_TramiteId",
                        column: x => x.TramiteId,
                        principalTable: "Tramites",
                        principalColumn: "TramiteId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pagos",
                columns: table => new
                {
                    PagoId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TramiteId = table.Column<int>(type: "INTEGER", nullable: false),
                    MontoCentavos = table.Column<long>(type: "INTEGER", nullable: false),
                    FechaPago = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MetodoPago = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    TipoPago = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    Referencia = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    Observaciones = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Anulado = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    FechaAnulacion = table.Column<DateTime>(type: "TEXT", nullable: true),
                    MotivoAnulacion = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagos", x => x.PagoId);
                    table.CheckConstraint("CK_Pagos_MontoPositivo", "MontoCentavos > 0");
                    table.ForeignKey(
                        name: "FK_Pagos_Tramites_TramiteId",
                        column: x => x.TramiteId,
                        principalTable: "Tramites",
                        principalColumn: "TramiteId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pendientes",
                columns: table => new
                {
                    PendienteId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TramiteId = table.Column<int>(type: "INTEGER", nullable: true),
                    Titulo = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    FechaVencimiento = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Prioridad = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    Estado = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaFinalizacion = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pendientes", x => x.PendienteId);
                    table.ForeignKey(
                        name: "FK_Pendientes_Tramites_TramiteId",
                        column: x => x.TramiteId,
                        principalTable: "Tramites",
                        principalColumn: "TramiteId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Seguimientos",
                columns: table => new
                {
                    SeguimientoId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TramiteId = table.Column<int>(type: "INTEGER", nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", maxLength: 3000, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UsuarioId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seguimientos", x => x.SeguimientoId);
                    table.ForeignKey(
                        name: "FK_Seguimientos_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Seguimientos_Tramites_TramiteId",
                        column: x => x.TramiteId,
                        principalTable: "Tramites",
                        principalColumn: "TramiteId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AcompanantesViaje",
                columns: table => new
                {
                    AcompananteViajeId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FormularioVisaId = table.Column<int>(type: "INTEGER", nullable: false),
                    NombreCompleto = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Parentesco = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcompanantesViaje", x => x.AcompananteViajeId);
                    table.ForeignKey(
                        name: "FK_AcompanantesViaje_FormulariosVisa_FormularioVisaId",
                        column: x => x.FormularioVisaId,
                        principalTable: "FormulariosVisa",
                        principalColumn: "FormularioVisaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmpleosAnteriores",
                columns: table => new
                {
                    EmpleoAnteriorId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FormularioVisaId = table.Column<int>(type: "INTEGER", nullable: false),
                    NombreEmpresa = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Direccion = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Telefono = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    Puesto = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    NombreJefeInmediato = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    FechaInicio = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FechaTermino = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpleosAnteriores", x => x.EmpleoAnteriorId);
                    table.ForeignKey(
                        name: "FK_EmpleosAnteriores_FormulariosVisa_FormularioVisaId",
                        column: x => x.FormularioVisaId,
                        principalTable: "FormulariosVisa",
                        principalColumn: "FormularioVisaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FamiliaresVisa",
                columns: table => new
                {
                    FamiliarVisaId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FormularioVisaId = table.Column<int>(type: "INTEGER", nullable: false),
                    NombreCompleto = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Parentesco = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    EstatusLegalEEUU = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FamiliaresVisa", x => x.FamiliarVisaId);
                    table.ForeignKey(
                        name: "FK_FamiliaresVisa_FormulariosVisa_FormularioVisaId",
                        column: x => x.FormularioVisaId,
                        principalTable: "FormulariosVisa",
                        principalColumn: "FormularioVisaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IdiomasVisa",
                columns: table => new
                {
                    IdiomaVisaId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FormularioVisaId = table.Column<int>(type: "INTEGER", nullable: false),
                    Idioma = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdiomasVisa", x => x.IdiomaVisaId);
                    table.ForeignKey(
                        name: "FK_IdiomasVisa_FormulariosVisa_FormularioVisaId",
                        column: x => x.FormularioVisaId,
                        principalTable: "FormulariosVisa",
                        principalColumn: "FormularioVisaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaisesVisitados",
                columns: table => new
                {
                    PaisVisitadoId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FormularioVisaId = table.Column<int>(type: "INTEGER", nullable: false),
                    Pais = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaisesVisitados", x => x.PaisVisitadoId);
                    table.ForeignKey(
                        name: "FK_PaisesVisitados_FormulariosVisa_FormularioVisaId",
                        column: x => x.FormularioVisaId,
                        principalTable: "FormulariosVisa",
                        principalColumn: "FormularioVisaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RedesSocialesVisa",
                columns: table => new
                {
                    RedSocialVisaId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FormularioVisaId = table.Column<int>(type: "INTEGER", nullable: false),
                    Plataforma = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    NombreUsuario = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RedesSocialesVisa", x => x.RedSocialVisaId);
                    table.ForeignKey(
                        name: "FK_RedesSocialesVisa_FormulariosVisa_FormularioVisaId",
                        column: x => x.FormularioVisaId,
                        principalTable: "FormulariosVisa",
                        principalColumn: "FormularioVisaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VisitasAnterioresEEUU",
                columns: table => new
                {
                    VisitaAnteriorEEUUId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FormularioVisaId = table.Column<int>(type: "INTEGER", nullable: false),
                    FechaVisita = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DuracionEstancia = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitasAnterioresEEUU", x => x.VisitaAnteriorEEUUId);
                    table.ForeignKey(
                        name: "FK_VisitasAnterioresEEUU_FormulariosVisa_FormularioVisaId",
                        column: x => x.FormularioVisaId,
                        principalTable: "FormulariosVisa",
                        principalColumn: "FormularioVisaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HistorialCambiosPagos",
                columns: table => new
                {
                    CambioPagoId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PagoId = table.Column<int>(type: "INTEGER", nullable: false),
                    UsuarioId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    TipoCambio = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    DatosAnteriores = table.Column<string>(type: "TEXT", nullable: false),
                    DatosNuevos = table.Column<string>(type: "TEXT", nullable: true),
                    FechaCambio = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Motivo = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialCambiosPagos", x => x.CambioPagoId);
                    table.ForeignKey(
                        name: "FK_HistorialCambiosPagos_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistorialCambiosPagos_Pagos_PagoId",
                        column: x => x.PagoId,
                        principalTable: "Pagos",
                        principalColumn: "PagoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcompanantesViaje_FormularioVisaId",
                table: "AcompanantesViaje",
                column: "FormularioVisaId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Citas_FechaConfirmada",
                table: "Citas",
                column: "FechaConfirmada");

            migrationBuilder.CreateIndex(
                name: "IX_Citas_FechaTentativa",
                table: "Citas",
                column: "FechaTentativa");

            migrationBuilder.CreateIndex(
                name: "IX_Citas_TramiteId_EstadoCita",
                table: "Citas",
                columns: new[] { "TramiteId", "EstadoCita" });

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_NombreCompleto",
                table: "Clientes",
                column: "NombreCompleto");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Telefono",
                table: "Clientes",
                column: "Telefono");

            migrationBuilder.CreateIndex(
                name: "IX_DatosPasaporte_TramiteId",
                table: "DatosPasaporte",
                column: "TramiteId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documentos_NombreAlmacenado",
                table: "Documentos",
                column: "NombreAlmacenado",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documentos_TramiteId_TipoDocumento",
                table: "Documentos",
                columns: new[] { "TramiteId", "TipoDocumento" });

            migrationBuilder.CreateIndex(
                name: "IX_EmpleosAnteriores_FormularioVisaId",
                table: "EmpleosAnteriores",
                column: "FormularioVisaId");

            migrationBuilder.CreateIndex(
                name: "IX_EstadosPorServicio_EstadoId",
                table: "EstadosPorServicio",
                column: "EstadoId");

            migrationBuilder.CreateIndex(
                name: "IX_EstadosTramite_Codigo",
                table: "EstadosTramite",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FamiliaresVisa_FormularioVisaId",
                table: "FamiliaresVisa",
                column: "FormularioVisaId");

            migrationBuilder.CreateIndex(
                name: "IX_FormulariosVisa_EstadoCaptura",
                table: "FormulariosVisa",
                column: "EstadoCaptura");

            migrationBuilder.CreateIndex(
                name: "IX_FormulariosVisa_TramiteId",
                table: "FormulariosVisa",
                column: "TramiteId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HistorialCambiosPagos_FechaCambio",
                table: "HistorialCambiosPagos",
                column: "FechaCambio");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialCambiosPagos_PagoId",
                table: "HistorialCambiosPagos",
                column: "PagoId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialCambiosPagos_UsuarioId",
                table: "HistorialCambiosPagos",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialCostos_FechaCambio",
                table: "HistorialCostos",
                column: "FechaCambio");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialCostos_TramiteId",
                table: "HistorialCostos",
                column: "TramiteId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialCostos_UsuarioId",
                table: "HistorialCostos",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialEstados_EstadoAnteriorId",
                table: "HistorialEstados",
                column: "EstadoAnteriorId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialEstados_EstadoNuevoId",
                table: "HistorialEstados",
                column: "EstadoNuevoId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialEstados_TramiteId_FechaCambio",
                table: "HistorialEstados",
                columns: new[] { "TramiteId", "FechaCambio" });

            migrationBuilder.CreateIndex(
                name: "IX_HistorialEstados_UsuarioId",
                table: "HistorialEstados",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_IdiomasVisa_FormularioVisaId",
                table: "IdiomasVisa",
                column: "FormularioVisaId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_FechaPago",
                table: "Pagos",
                column: "FechaPago");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_TramiteId",
                table: "Pagos",
                column: "TramiteId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_TramiteId_Anulado",
                table: "Pagos",
                columns: new[] { "TramiteId", "Anulado" });

            migrationBuilder.CreateIndex(
                name: "IX_PaisesVisitados_FormularioVisaId",
                table: "PaisesVisitados",
                column: "FormularioVisaId");

            migrationBuilder.CreateIndex(
                name: "IX_Pendientes_Estado_FechaVencimiento",
                table: "Pendientes",
                columns: new[] { "Estado", "FechaVencimiento" });

            migrationBuilder.CreateIndex(
                name: "IX_Pendientes_FechaVencimiento",
                table: "Pendientes",
                column: "FechaVencimiento");

            migrationBuilder.CreateIndex(
                name: "IX_Pendientes_TramiteId",
                table: "Pendientes",
                column: "TramiteId");

            migrationBuilder.CreateIndex(
                name: "IX_RedesSocialesVisa_FormularioVisaId",
                table: "RedesSocialesVisa",
                column: "FormularioVisaId");

            migrationBuilder.CreateIndex(
                name: "IX_Seguimientos_TramiteId_FechaRegistro",
                table: "Seguimientos",
                columns: new[] { "TramiteId", "FechaRegistro" });

            migrationBuilder.CreateIndex(
                name: "IX_Seguimientos_UsuarioId",
                table: "Seguimientos",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_TiposServicio_Codigo",
                table: "TiposServicio",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tramites_ClienteId",
                table: "Tramites",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Tramites_EstadoActualId",
                table: "Tramites",
                column: "EstadoActualId");

            migrationBuilder.CreateIndex(
                name: "IX_Tramites_FechaInicio",
                table: "Tramites",
                column: "FechaInicio");

            migrationBuilder.CreateIndex(
                name: "IX_Tramites_TipoServicioId",
                table: "Tramites",
                column: "TipoServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_TransicionesEstado_EstadoDestinoId",
                table: "TransicionesEstado",
                column: "EstadoDestinoId");

            migrationBuilder.CreateIndex(
                name: "IX_TransicionesEstado_EstadoOrigenId",
                table: "TransicionesEstado",
                column: "EstadoOrigenId");

            migrationBuilder.CreateIndex(
                name: "IX_TransicionesEstado_TipoServicioId_EstadoOrigenId_EstadoDestinoId",
                table: "TransicionesEstado",
                columns: new[] { "TipoServicioId", "EstadoOrigenId", "EstadoDestinoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VisitasAnterioresEEUU_FormularioVisaId",
                table: "VisitasAnterioresEEUU",
                column: "FormularioVisaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcompanantesViaje");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Citas");

            migrationBuilder.DropTable(
                name: "DatosPasaporte");

            migrationBuilder.DropTable(
                name: "Documentos");

            migrationBuilder.DropTable(
                name: "EmpleosAnteriores");

            migrationBuilder.DropTable(
                name: "EstadosPorServicio");

            migrationBuilder.DropTable(
                name: "FamiliaresVisa");

            migrationBuilder.DropTable(
                name: "HistorialCambiosPagos");

            migrationBuilder.DropTable(
                name: "HistorialCostos");

            migrationBuilder.DropTable(
                name: "HistorialEstados");

            migrationBuilder.DropTable(
                name: "IdiomasVisa");

            migrationBuilder.DropTable(
                name: "PaisesVisitados");

            migrationBuilder.DropTable(
                name: "Pendientes");

            migrationBuilder.DropTable(
                name: "RedesSocialesVisa");

            migrationBuilder.DropTable(
                name: "Seguimientos");

            migrationBuilder.DropTable(
                name: "TransicionesEstado");

            migrationBuilder.DropTable(
                name: "VisitasAnterioresEEUU");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Pagos");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "FormulariosVisa");

            migrationBuilder.DropTable(
                name: "Tramites");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "EstadosTramite");

            migrationBuilder.DropTable(
                name: "TiposServicio");
        }
    }
}
