namespace Bild.Core.Interactors.Files;

public class ResolveNameClashInteractor
{
    public string Perform(string targetDir, string targetFilename, string targetExtension)
    {
        var testPath = string.Empty;
        var clashCount = 0;
        var resultFilename = string.Empty;
        
        do
        {
            resultFilename = $"{targetFilename}{(clashCount == 0 ? string.Empty : $"_{clashCount}")}";
            testPath = Path.Combine(targetDir, $"{resultFilename}.{targetExtension}");
            clashCount++;
        } while (File.Exists(testPath));

        return resultFilename;
    }
}