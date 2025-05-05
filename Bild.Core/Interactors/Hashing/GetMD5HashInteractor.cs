using System.Security.Cryptography;

namespace Bild.Core.Interactors.Hashing;

public class GetMD5HashInteractor
{
    public string Perform(MD5 md5, string filePath)
    {
        using var stream = File.OpenRead(filePath);
        var hashBytes = md5.ComputeHash(stream);
        return Convert.ToHexStringLower(hashBytes);
    }
}
