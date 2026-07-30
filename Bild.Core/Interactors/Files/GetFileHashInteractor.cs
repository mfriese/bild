using Bild.Core.Features.Files;
using System.Security.Cryptography;

namespace Bild.Core.Interactors.Files;

public class GetFileHashInteractor
{
    public string Perform(MediaFile file)
    {
        if (!file.Exists)
            return null;

        using var stream = file.ReadAsStream();
        return Perform(stream);
    }

    public string Perform(Stream stream)
    {
        if (stream is null || !stream.CanRead)
            return null;

        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(stream);

        return Convert.ToHexString(hash);
    }
}
