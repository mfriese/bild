namespace Bild.Core.Features.Files;

public class MediaDir(string path)
{
    public string AbsolutePath { get; } = Path.GetFullPath(path);

    public IEnumerable<MediaDir> Dirs => FindDirectories();
    public IEnumerable<MediaFile> Files => FindFiles();

    private IEnumerable<MediaDir> FindDirectories()
    {
        IEnumerable<MediaDir> findings;

        if (!Directory.Exists(AbsolutePath))
        {
            findings = [];
        }
        else
        {
            try
            {
                findings = Directory.
                    EnumerateDirectories(AbsolutePath).
                    Where(d => d != "." && d != "..").
                    Select(d => new MediaDir(d));
            }
            catch (UnauthorizedAccessException)
            {
                findings = [];
            }
        }

        return findings;
    }

    private IEnumerable<MediaFile> FindFiles()
    {
        IEnumerable<MediaFile> findings;

        if (!Directory.Exists(AbsolutePath))
        {
            // No findings, folder does not exist
            findings = [];
        }
        else
        {
            try
            {
                findings = Directory.
                    EnumerateFiles(AbsolutePath).
                    Select(f => new MediaFile(f));
            }
            catch (UnauthorizedAccessException)
            {
                findings = [];
            }
        }

        return findings;
    }
}
