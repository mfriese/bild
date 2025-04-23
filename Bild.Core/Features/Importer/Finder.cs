using Bild.Core.Features.Files;

namespace Bild.Core.Features.Importer;

public static class Finder
{
    public static IEnumerable<Files.MediaFile> FindFiles(MediaDir root)
        => root.Dirs.Select(FindFiles).SelectMany(ff => ff).Concat(root.Files);

    public static IEnumerable<Files.MediaFile> FindFiles(string path)
        => FindFiles(new MediaDir(path));
}
