using Bild.Core.Features.Files;

namespace Bild.Core.Interactors.Directories;

public class PathSelectorInteractor
{
    public MediaDir Perform(string defaultDir = "")
    {
        var workingDir = new MediaDir(defaultDir);
        
        if (!workingDir.Exists)
        {
            DriveSelectorInteractor driveSelector = new();
            workingDir = driveSelector.Perform();

            if (!workingDir.Exists)
                return workingDir;
        }

        DirectorySelectorInteractor directorySelector = new();
        return directorySelector.Perform(workingDir);
    }
}
