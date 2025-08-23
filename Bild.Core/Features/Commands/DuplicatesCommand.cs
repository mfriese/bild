using Bild.Core.Interactors.Directories;
using Bild.Core.Interactors.Hashing;
using Bild.Core.Interactors.UI;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Bild.Core.Features.Commands;

public class DuplicatesCommand : Command<DuplicatesSettings>
{
    public static string Name => "_Delete Duplicate Files";

    public override int Execute(CommandContext context, DuplicatesSettings settings)
    {
        WaitKeyPressInteractor waitKeyPress = new();

        AnsiConsole.MarkupLine("You have chosen to [red]delete[/] duplicate files. You " +
            "can cancel at any point along the way but once files are deleted they are " +
            "gone for good! We will only delete a file, if it has the [green]same hash " +
            "value[/] than another file, this means they are identical!\r\n");

        ImportPathOrSelectInteractor importPathOrSelect = new();
        var selectedDir = importPathOrSelect.Perform();

        if (string.IsNullOrEmpty(selectedDir?.AbsolutePath))
            return 0;

        GetAllHashesInteractor getAllHashes = new();
        var hashes = getAllHashes.Perform(selectedDir?.AbsolutePath);

        if (!hashes.Any(hh => 1 < hh.Count()))
        {
            AnsiConsole.MarkupLine($"[green]There are NO duplicates in {selectedDir}[/]");

            return waitKeyPress.Perform(0);
        }

        if (!AnsiConsole.Prompt(new ConfirmationPrompt("Proceed to delete duplicated files?")))
            return 0;

        var table = new Table()
            .Border(TableBorder.Rounded)
            .Expand()
            .BorderColor(Color.Grey)
            .AddColumn("[cyan]Hashcode[/]")
            .AddColumn("[grey]Kept file[/]")
            .AddColumn("[grey]Deleted count[/]");

        AnsiConsole.Live(table)
            .AutoClear(false)
            .Start(ctx =>
            {
                foreach (var group in hashes.Where(hh => 1 < hh.Count()))
                {
                    var keep = PickShortest(group);
                    var deleteCount = 0;

                    foreach (var file in group)
                    {
                        if (file != keep)
                        {
                            try
                            {
                                File.Delete(file);
                                ++deleteCount;
                            }
                            catch (Exception ex)
                            {
                                AnsiConsole.MarkupLine($"[red]Error deleting {file}: {ex.Message}[/]");
                            }
                        }
                    }

                    table.AddRow(group.Key, keep, $"{deleteCount}");
                    ctx.Refresh();
                }
            });

        return waitKeyPress.Perform(0);
    }

    private string PickShortest(IEnumerable<string> names)
        => names.OrderBy(tt => tt.Length).FirstOrDefault();
}
