using Bild.Core.Features.Files;

namespace Bild.Core.Features.Importer;

public static class Finder
{
    public static IEnumerable<MediaFile> FindFiles(MediaDir root)
    {
        var all = root.Dirs.
            Select(FindFiles).
            SelectMany(ff => ff).
            Concat(root.Files);

        return all;
    }

    public static IEnumerable<MediaFile> FindFiles(string path)
        => FindFiles(new MediaDir(path));
}
