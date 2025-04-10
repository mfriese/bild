namespace Bild.Core.Features.Files;

public static class FileExtensions
{
    public static File Rename(this File file, string newFilename)
    {
        var directoryName = Path.GetDirectoryName(file.AbsolutePath);
        var newFilepath = Path.Combine(directoryName, newFilename);

        try
        {
            System.IO.File.Move(file.AbsolutePath, newFilepath, false);
        }
        catch (Exception)
        {
            return null;
        }

        return new File(newFilepath);
    }
}
