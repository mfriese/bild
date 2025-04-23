namespace Bild.Core.Interactors.Directories;

public class PathSelectorInteractor
{
    public string Perform()
    {
        DriveSelectorInteractor driveSelector = new();
        var selectedDrive = driveSelector.Perform();

        if (string.IsNullOrEmpty(selectedDrive))
            return string.Empty;

        DirectorySelectorInteractor directorySelector = new();
        var selectedDir = directorySelector.Perform(selectedDrive);

        return selectedDir;
    }
}
