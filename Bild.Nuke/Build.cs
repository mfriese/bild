using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tools.DotNet;
using System;
using System.Linq;
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
            Print("Deleting old binaries ...");

            RootDirectory
                .GlobDirectories("**/{bin,obj}")
                .Where(absPath => !$"{absPath}".Contains("Nuke"))
                .Select(absPath => Print(absPath, ConsoleColor.Magenta))
                .DeleteDirectories();

            Print("... cleanup complete!");
        });

    Target Restore => _ => _
        .After(Clean)
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(ProjectFile));
        });

    Target Compile => _ => _
        .DependsOn(Restore, Clean)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(ProjectFile)
                .SetConfiguration(Configuration)
                .SetWarningLevel(0)
                .EnableNoRestore());
        });

    Target Test =>
        _ =>
            _.DependsOn(Compile)
                .Executes(() =>
                {
                    string[] testProjects = ["Bild.Test"];

                    foreach (string testPath in testProjects)
                    {
                        AbsolutePath testProject = RootDirectory / testPath / $"{testPath}.csproj";

                        DotNetTest(s =>
                            s.SetProjectFile(testProject)
                                .SetConfiguration(Configuration)
                                .SetLoggers("trx")
                        //.SetResultsDirectory("TestResults")
                        );
                    }
                });

    private static T Print<T>(T thing, ConsoleColor color = ConsoleColor.Red)
    {
        // aktuelle Farbe sichern
        var oldColor = Console.ForegroundColor;

        try
        {
            Console.ForegroundColor = color;

            Console.WriteLine($"{thing}");
        }
        finally
        {
            Console.ForegroundColor = oldColor;
        }

        return thing;
    }
}
