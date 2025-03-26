using Spectre.Console;
using System.Security.Cryptography;

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

        var files = Directory.GetFiles(rootPath, "*", SearchOption.AllDirectories);

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

        using var md5 = MD5.Create();
        GetHashInteractor getHash = new();

        AnsiConsole.MarkupLine($"Calculating hashes in: '{rootPath}'");

        progress.Start(ctx =>
        {
            var task = ctx.AddTask($"[green]{files.Length} files[/]", maxValue: files.Length);

            foreach (var file in files)
            {
                Task.Delay(200).Wait();

                try
                {
                    string hash = getHash.Perform(md5, file);
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
