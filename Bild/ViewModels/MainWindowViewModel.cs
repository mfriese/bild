using Bild.Core.Environment;
using Bild.Core.Files;
using ReactiveUI;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive;

namespace Bild.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		public MainWindowViewModel()
		{
			ImportFolder = ReactiveCommand.Create(ImportFolderImpl);
			OpenProject = ReactiveCommand.Create(OpenProjectImpl);

			Album = new Album(new Settings()
			{
				ProjectFolder = @"C:\tmp\proj"
			});
		}

		public ReactiveCommand<Unit, Unit> ImportFolder { get; }
		public ReactiveCommand<Unit, Unit> OpenProject { get; }

		void ImportFolderImpl()
		{

		}

		void OpenProjectImpl()
		{

		}

		public Album Album { get; }

		private Dir m_selectedPath;
		public Dir SelectedPath 
		{
			get => m_selectedPath;
			set
			{
				m_selectedPath = value;
				Files = m_selectedPath.Files;

				(this as IReactiveObject)?.RaisePropertyChanged(new PropertyChangedEventArgs(nameof(Files)));
			}
		}

		public IEnumerable<File> Files { get; set; }
	}
}
