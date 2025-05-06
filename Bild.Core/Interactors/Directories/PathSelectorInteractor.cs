namespace Bild.Core.Interactors.Directories;

public class PathSelectorInteractor
{
    public string Perform(string defaultDir = "")
    {
        if (string.IsNullOrEmpty(defaultDir))
        {
            DriveSelectorInteractor driveSelector = new();
            defaultDir = driveSelector.Perform();

            if (string.IsNullOrEmpty(defaultDir))
                return string.Empty;
        }

        DirectorySelectorInteractor directorySelector = new();
        var selectedDir = directorySelector.Perform(defaultDir);

        return selectedDir;
    }
}
