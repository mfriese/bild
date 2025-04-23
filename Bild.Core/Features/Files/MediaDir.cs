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
                findings = from dd in Directory.EnumerateDirectories(AbsolutePath)
                           where dd != "." && dd != ".."
                           select new MediaDir(dd);
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
                findings = from ff in Directory.EnumerateFiles(AbsolutePath)
                           select new MediaFile(ff);
            }
            catch (UnauthorizedAccessException)
            {
                findings = [];
            }
        }

        return findings;
    }
}
