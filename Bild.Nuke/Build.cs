using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tools.DotNet;
using System.IO;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

class Build : NukeBuild
{
    AbsolutePath ProjectDir => RootDirectory / "Bild";
    AbsolutePath ProjectFile => ProjectDir / "Bild.csproj";

    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            Directory.Delete(ProjectDir / "bin", true);
            Directory.Delete(ProjectDir / "obj", true);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(ProjectFile));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(ProjectFile)
                .SetConfiguration(Configuration)
                .SetWarningLevel(0)
                .EnableNoRestore());
        });

}
