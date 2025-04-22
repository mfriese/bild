using System.Globalization;

namespace Bild.Core.Features.Utils;

public static class SettingsUtil
{
    public static string Long(string name)
        => $"--{name.ToLower(CultureInfo.InvariantCulture)}";

    public static string Short(string name)
        => $"-{name.ToLower(CultureInfo.InvariantCulture).First()}";
}
