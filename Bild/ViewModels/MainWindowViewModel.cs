using ReactiveUI;
using System.Reactive;

namespace Bild.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		public MainWindowViewModel()
		{
			ImportFolder = ReactiveCommand.Create(ImportFolderImpl);
			OpenProject = ReactiveCommand.Create(OpenProjectImpl);
		}

		public ReactiveCommand<Unit, Unit> ImportFolder { get; }
		public ReactiveCommand<Unit, Unit> OpenProject { get; }

		void ImportFolderImpl()
		{

		}

		void OpenProjectImpl()
		{

		}

		public string Greeting => "Nothing here, yet!";
	}
}
