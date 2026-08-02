namespace Bild.Core.Features.Files;

public record DuplicateImagePair(
    MediaFile FileToKeep,
    MediaFile FileToDelete,
    string FileToKeepHash,
    string FileToDeleteHash,
    bool IsExactMatch,
    double Similarity);
