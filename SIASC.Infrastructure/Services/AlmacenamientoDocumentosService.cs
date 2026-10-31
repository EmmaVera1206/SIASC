
using System.Security.Cryptography;

namespace SIASC.Infrastructure.Services;

public sealed class AlmacenamientoDocumentosService
{
    public const long TamanoMaximoBytes = 10 * 1024 * 1024;

    private readonly string _directorioBase;

    public AlmacenamientoDocumentosService(string directorioBase)
    {
        if (string.IsNullOrWhiteSpace(directorioBase))
        {
            throw new ArgumentException(
                "El directorio de almacenamiento es obligatorio.",
                nameof(directorioBase));
        }

        _directorioBase = Path.GetFullPath(directorioBase);

        Directory.CreateDirectory(_directorioBase);
    }

    public async Task<ResultadoAlmacenamiento> GuardarAsync(
        Stream contenido,
        string nombreOriginal,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(contenido);

        if (!contenido.CanRead)
        {
            throw new InvalidDataException(
                "No es posible leer el archivo.");
        }

        var extension = Path.GetExtension(nombreOriginal)
            .ToLowerInvariant();

        if (!EsExtensionPermitida(extension))
        {
            throw new InvalidDataException(
                "Solo se permiten archivos PDF, JPG, JPEG y PNG.");
        }

        var nombreAlmacenado =
            $"{Guid.NewGuid():N}{extension}";

        var rutaDestino = ObtenerRutaSegura(nombreAlmacenado);

        try
        {
            await using var destino = new FileStream(
                rutaDestino,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 81920,
                useAsync: true);

            using var hash = IncrementalHash.CreateHash(
                HashAlgorithmName.SHA256);

            var buffer = new byte[81920];
            var cabecera = new byte[8];

            var longitudCabecera = 0;
            long totalBytes = 0;

            int leidos;

            while ((leidos = await contenido.ReadAsync(
                buffer.AsMemory(),
                cancellationToken)) > 0)
            {
                totalBytes += leidos;

                if (totalBytes > TamanoMaximoBytes)
                {
                    throw new InvalidDataException(
                        "El archivo supera el límite de 10 MB.");
                }

                if (longitudCabecera < cabecera.Length)
                {
                    var cantidad = Math.Min(
                        leidos,
                        cabecera.Length - longitudCabecera);

                    Array.Copy(
                        buffer,
                        0,
                        cabecera,
                        longitudCabecera,
                        cantidad);

                    longitudCabecera += cantidad;
                }

                hash.AppendData(buffer, 0, leidos);

                await destino.WriteAsync(
                    buffer.AsMemory(0, leidos),
                    cancellationToken);
            }

            if (totalBytes == 0)
            {
                throw new InvalidDataException(
                    "No se permiten archivos vacíos.");
            }

            if (!CoincideFirmaArchivo(
                extension,
                cabecera.AsSpan(0, longitudCabecera)))
            {
                throw new InvalidDataException(
                    "El contenido del archivo no coincide con su formato.");
            }

            await destino.FlushAsync(cancellationToken);

            return new ResultadoAlmacenamiento
            {
                NombreAlmacenado = nombreAlmacenado,
                TipoMime = ObtenerTipoMime(extension),
                TamanoBytes = totalBytes,
                HashSHA256 = Convert.ToHexString(
                    hash.GetHashAndReset()).ToLowerInvariant()
            };
        }
        catch
        {
            if (File.Exists(rutaDestino))
            {
                File.Delete(rutaDestino);
            }

            throw;
        }
    }

    public Task<Stream> AbrirLecturaAsync(
        string nombreAlmacenado,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var ruta = ObtenerRutaSegura(nombreAlmacenado);

        Stream archivo = new FileStream(
            ruta,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 81920,
            useAsync: true);

        return Task.FromResult(archivo);
    }

    public Task EliminarAsync(
        string nombreAlmacenado,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var ruta = ObtenerRutaSegura(nombreAlmacenado);

        if (File.Exists(ruta))
        {
            File.Delete(ruta);
        }

        return Task.CompletedTask;
    }

    private string ObtenerRutaSegura(string nombreAlmacenado)
    {
        if (string.IsNullOrWhiteSpace(nombreAlmacenado))
        {
            throw new ArgumentException(
                "El nombre almacenado es obligatorio.");
        }

        var extension = Path.GetExtension(nombreAlmacenado)
            .ToLowerInvariant();

        var identificador = Path.GetFileNameWithoutExtension(
            nombreAlmacenado);

        if (!EsExtensionPermitida(extension) ||
            identificador.Length != 32 ||
            !Guid.TryParseExact(
                identificador,
                "N",
                out _))
        {
            throw new InvalidDataException(
                "El identificador del archivo no es válido.");
        }

        return Path.Combine(
            _directorioBase,
            nombreAlmacenado);
    }

    private static bool EsExtensionPermitida(string extension)
    {
        return extension is
            ".pdf" or ".jpg" or ".jpeg" or ".png";
    }

    private static string ObtenerTipoMime(string extension)
    {
        return extension switch
        {
            ".pdf" => "application/pdf",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            _ => throw new InvalidDataException(
                "Formato no permitido.")
        };
    }

    private static bool CoincideFirmaArchivo(
        string extension,
        ReadOnlySpan<byte> cabecera)
    {
        if (extension == ".pdf")
        {
            return cabecera.Length >= 5 &&
                   cabecera[0] == 0x25 &&
                   cabecera[1] == 0x50 &&
                   cabecera[2] == 0x44 &&
                   cabecera[3] == 0x46 &&
                   cabecera[4] == 0x2D;
        }

        if (extension is ".jpg" or ".jpeg")
        {
            return cabecera.Length >= 3 &&
                   cabecera[0] == 0xFF &&
                   cabecera[1] == 0xD8 &&
                   cabecera[2] == 0xFF;
        }

        if (extension == ".png")
        {
            byte[] firmaPng =
            [
                0x89, 0x50, 0x4E, 0x47,
                0x0D, 0x0A, 0x1A, 0x0A
            ];

            return cabecera.StartsWith(firmaPng);
        }

        return false;
    }
}

public sealed class ResultadoAlmacenamiento
{
    public string NombreAlmacenado { get; init; } =
        string.Empty;

    public string TipoMime { get; init; } =
        string.Empty;

    public long TamanoBytes { get; init; }

    public string HashSHA256 { get; init; } =
        string.Empty;
}
