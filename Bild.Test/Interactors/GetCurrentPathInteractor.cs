using System.Reflection;

namespace Bild.Test.Interactors;

public class GetCurrentPathInteractor
{
    public string Perform()
    {
        string path = Assembly.GetExecutingAssembly().Location;

        return Path.GetDirectoryName(path) ?? string.Empty;
    }
}
