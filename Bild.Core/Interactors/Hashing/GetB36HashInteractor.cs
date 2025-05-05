using System.Security.Cryptography;

namespace Bild.Core.Interactors.Hashing;

public class GetB36HashInteractor
{
    private const string Base36Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public string Perform(MD5 md5, string filePath)
    {
        using var stream = File.OpenRead(filePath);
        var hashBytes = md5.ComputeHash(stream);
        var hash = Convert.ToHexStringLower(hashBytes);

        // Schritt 2: Die ersten 3 Bytes zu einer Ganzzahl machen (24 Bit → max 16.7 Mio)
        int number = (hash[0] << 16) | (hash[1] << 8) | hash[2];

        // Schritt 3: In Base36 umwandeln
        return ToBase36(number).PadLeft(4, '0').Substring(0, 4);
    }

    private static string ToBase36(int value)
    {
        var buffer = new char[7]; // max. 7 Stellen für int.MaxValue
        var ii = buffer.Length;

        do
        {
            buffer[--ii] = Base36Chars[value % 36];
            value /= 36;
        } while (value > 0);

        return new string(buffer, ii, buffer.Length - ii);
    }
}