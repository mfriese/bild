using Spectre.Console;

namespace Bild.Core.Interactors.UI;

public class GetProgressInteractor
{
    public Progress Perform()
        => AnsiConsole.Progress()
            .AutoClear(false)
            .AutoRefresh(true)
            .HideCompleted(false)
            .Columns(
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new RemainingTimeColumn(),
                new SpinnerColumn()
            );
}
