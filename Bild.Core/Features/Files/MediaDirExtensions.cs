namespace Bild.Core.Features.Files;

public static class MediaDirExtensions
{
    public static IEnumerable<MediaFile> FindFilesRecursive(this MediaDir directory)
    {
        var all = directory.Dirs.
            Select(FindFilesRecursive).
            SelectMany(ff => ff).
            Concat(directory.Files);

        return all;
    }
}