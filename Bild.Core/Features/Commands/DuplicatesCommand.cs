using Bild.Core.Interactors.Directories;
using Bild.Core.Interactors.Hashing;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Bild.Core.Features.Commands;

public class DuplicatesCommand : Command<DuplicatesSettings>
{
    public static string Name => "Delete Duplicate Files";

    public override int Execute(CommandContext context, DuplicatesSettings settings)
    {
        AnsiConsole.MarkupLine("You have chosen to [red]delete[/] duplicate files. You " +
            "can cancel at any point along the way but once files are deleted they are " +
            "gone for good! We will only delete a file, if it has the [green]same hash " +
            "value[/] than another file, this means they are identical!\r\n");

        DriveSelectorInteractor driveSelector = new();
        var selectedDrive = driveSelector.Perform();

        if (string.IsNullOrEmpty(selectedDrive))
            return CancellationMessage(0);

        DirectorySelectorInteractor directorySelector = new();
        var selectedDir = directorySelector.Perform(selectedDrive);

        if (string.IsNullOrEmpty(selectedDir))
            return CancellationMessage(0);

        GetAllHashesInteractor getAllHashes = new();
        var hashes = getAllHashes.Perform(selectedDir);

        if (!hashes.Any(hh => 1 < hh.Count()))
        {
            AnsiConsole.MarkupLine($"[green]There are NO duplicates in {selectedDir}[/]");
            AnsiConsole.MarkupLine("Press [green]any key[/] to continue ...");
            Console.ReadKey(true);
            return 0;
        }

        char answer = AnsiConsole.Prompt(
            new TextPrompt<char>("How to proceed? [green](a)uto[/] / [yellow](m)anual[/] / [red](c)ancel[/]")
                .AllowEmpty()
                .ValidationErrorMessage("[red]Only a, m or c accepted![/]")
                .Validate(input =>
                    input == 'a' || input == 'm' || input == 'c'
                        ? ValidationResult.Success()
                        : ValidationResult.Error()
                )
            );

        if (answer == 'c')
            return CancellationMessage(0);

        if (answer == 'm')
            return CancellationMessage(0);

        if (answer == 'a')
            return HandleAutomatic(hashes);

        return CancellationMessage(0);
    }

    private int HandleAutomatic(List<IGrouping<string, string>> hashes)
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Grey)
            .AddColumn("[cyan]Hash[/]")
            .AddColumn("[grey]Behalten[/]");

        AnsiConsole.Live(table)
            .AutoClear(false)
            .Start(ctx =>
            {
                foreach (var group in hashes)
                {
                    var keep = PickShortes(group);

                    foreach (var file in group)
                    {
                        if (file != keep)
                        {
                            try
                            {
                                File.Delete(file);
                            }
                            catch (Exception ex)
                            {
                                AnsiConsole.MarkupLine($"[red]Error deleting {file}: {ex.Message}[/]");
                            }
                        }
                    }

                    table.AddRow(group.Key, keep);
                    ctx.Refresh();
                }
            });

        AnsiConsole.MarkupLine("Press [green]any key[/] to continue ...");
        Console.ReadKey(true);

        return CancellationMessage(0);
    }

    private string PickShortes(IEnumerable<string> names)
        => names.OrderBy(tt => tt.Length).FirstOrDefault();

    private int CancellationMessage(int returnValue)
    {
        AnsiConsole.MarkupLine("[red]Operation cancelled![/]");

        return returnValue;
    }
}
