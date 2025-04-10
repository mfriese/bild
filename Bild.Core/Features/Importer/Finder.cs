using Bild.Core.Features.Files;

namespace Bild.Core.Features.Importer;

public static class Finder
{
    public static IEnumerable<Files.File> FindFiles(Dir root)
        => root.Dirs.Select(FindFiles).SelectMany(ff => ff).Concat(root.Files);

    public static IEnumerable<Files.File> FindFiles(string path)
        => FindFiles(new Dir(path));
}
