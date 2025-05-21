using Bild.Core.Features.Importer;
using Spectre.Console;

namespace Bild.Core.Interactors.Hashing;

public class GetAllHashesInteractor
{
    public List<IGrouping<string, string>> Perform(string rootPath)
    {
        List<Tuple<string, string>> hashes = [];

        if (!Directory.Exists(rootPath))
        {
            AnsiConsole.MarkupLine($"[red]Selected path '{rootPath}' does not exist![/]");

            return hashes.GroupBy(hh => hh.Item1, hh => hh.Item2).ToList();
        }

        var files = Finder.FindFiles(rootPath).Select(ff => ff.AbsolutePath).ToArray();

        var progress = AnsiConsole.Progress()
            .AutoClear(false)
            .AutoRefresh(true)
            .HideCompleted(false)
            .Columns(
            [
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new RemainingTimeColumn(),
                new SpinnerColumn()
            ]);

        GetHashInteractor getMd5Hash = new();

        AnsiConsole.MarkupLine($"Calculating hashes in: '{rootPath}'");

        progress.Start(ctx =>
        {
            var task = ctx.AddTask($"[green]{files.Length} files[/]", maxValue: files.Length);

            foreach (var file in files)
            {
                try
                {
                    string hash = getMd5Hash.Perform(file);
                    hashes.Add(new Tuple<string, string>(hash, file));
                    task.Value = hashes.Count;
                }
                catch (Exception ex)
                {
                    AnsiConsole.Markup($"[red]Error when hashing {file}: {ex.Message}[/]");
                }
            }
        });

        var resultGroup = hashes.GroupBy(hh => hh.Item1, hh => hh.Item2).ToList();

        AnsiConsole.MarkupLine($"Found [green]{files.Length}[/] files, " +
            $"[red]{resultGroup.Count}[/] are unique.");

        AnsiConsole.WriteLine();

        return resultGroup;
    }
}
