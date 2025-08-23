using Bild.Core.Features.Files;

namespace Bild.Core.Interactors.Files;

public class GetExifFilenameInteractor
{
    public string Perform(MediaFile file, bool randomSuffix = false)
    {
        string suffix = string.Empty;

        if (randomSuffix)
        {
            suffix = $"_{GenerateRandomString(4)}";
        }

        if (file.ExifCreationDate?.ToString("yyyyMMdd_hhmmss") is string date)
            return $"img_{date}{suffix}.{file.ExifFileNameExtension ?? file.Extension}";
        return null;
    }

    static string GenerateRandomString(int length)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random(DateTime.Now.Millisecond);

        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)])
            .ToArray());
    }
}
