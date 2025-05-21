using System.Security.Cryptography;

namespace Bild.Core.Interactors.Hashing;

public class GetHashInteractor
{
    private MD5 MD5 { get; } = MD5.Create();

    public string Perform(string filePath)
    {
        using var stream = File.OpenRead(filePath);
        var hashBytes = MD5.ComputeHash(stream);
        return Convert.ToHexStringLower(hashBytes);
    }
}
