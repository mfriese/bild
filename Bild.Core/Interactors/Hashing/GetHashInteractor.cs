using System.Security.Cryptography;

namespace Bild.Core.Interactors.Hashing;

public class GetHashInteractor
{
    public string Perform(MD5 md5, string filePath)
    {
        using var stream = File.OpenRead(filePath);
        var hashBytes = md5.ComputeHash(stream);
        return Convert.ToHexStringLower(hashBytes);

    }
}
