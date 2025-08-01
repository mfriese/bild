using Bild.Core.Interactors.Hashing;

namespace Bild.Core.Features.Files;

public static class MediaFileExtensions
{
    public static MediaFile CopyTo(this MediaFile file, MediaDir targetDir)
        => file.CopyOrMove(targetDir, true);

    public static MediaFile MoveTo(this MediaFile file, MediaDir targetDir)
        => file.CopyOrMove(targetDir, false);

    private static MediaFile CopyOrMove(this MediaFile file, MediaDir targetDir, bool makeCopy)
    {
        if (!File.Exists(file.AbsolutePath))
            return null;

        var targetPath = targetDir.AbsolutePath;

        if (!Directory.Exists(targetPath))
            return null;

        var targetExtension = file.ExifFileNameExtension ?? file.Extension;
        var targetFilePath = Path.Combine(targetPath, $"{file.Filename}.{targetExtension}");

        var fileExists = File.Exists(targetFilePath);

        if (makeCopy)
        {
            if (!fileExists)
            {
                File.Copy(file.AbsolutePath, targetFilePath, false);

                return new MediaFile(targetFilePath);
            }
        }
        else
        {
            if (!fileExists)
            {
                File.Move(file.AbsolutePath, targetFilePath, false);
            }
            else
            {
                File.Delete(file.AbsolutePath);
            }
            
            return new MediaFile(targetFilePath);
        }

        return null;
    }
}
