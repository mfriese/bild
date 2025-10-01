using Bild.Core.Features.Files;
using CoenM.ImageHash;
using CoenM.ImageHash.HashAlgorithms;

namespace Bild.Core.Interactors.Files;
public class CompareImagesInteractor
{
    public double Perform(MediaFile first, MediaFile second)
    {
        try
        {
            var hashAlgorithm = new AverageHash();
            using var sourceStream = File.OpenRead(first.AbsolutePath);
            var sourceHash = hashAlgorithm.Hash(sourceStream);
            using var targetStream = File.OpenRead(second.AbsolutePath);
            var targetHash = hashAlgorithm.Hash(targetStream);
            return CompareHash.Similarity(sourceHash, targetHash);
        }
        catch (Exception)
        {
            // Hash algorithm might crash when source file is not flawless
            return 0d;
        }
    }
}
