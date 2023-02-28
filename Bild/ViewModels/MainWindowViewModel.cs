using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Bild.Core.Environment;
using Bild.Core.Files;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive;
using System.Threading.Tasks;

namespace Bild.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		public MainWindowViewModel(Repository repository)
		{
			ImportFolder = ReactiveCommand.Create(ImportFolderImpl);
			OpenProject = ReactiveCommand.Create(OpenProjectImpl);
			Repository = repository;
			
			Album = new Album(Repository.Settings);
		}

		public Repository Repository { get; }
		public ReactiveCommand<Unit, Unit> ImportFolder { get; }
		public ReactiveCommand<Unit, Unit> OpenProject { get; }

		void ImportFolderImpl()
		{

		}

		void OpenProjectImpl()
		{
			var lifetime = Avalonia.Application.Current?.ApplicationLifetime;

			if (lifetime is IClassicDesktopStyleApplicationLifetime desktop)
			{
				var dialog = new OpenFolderDialog();
				var findDirTask = dialog.ShowAsync(desktop.MainWindow);
				var foundDir = findDirTask.GetAwaiter().GetResult();

				if (!string.IsNullOrEmpty(foundDir))
				{
					var settings = Repository.Settings;
					settings.ProjectFolder = foundDir;
					Repository.Settings = settings;

					Album = new Album(Repository.Settings);
					SelectedPath = null;
					Files = null;

					(this as IReactiveObject)?.RaisePropertyChanged(new PropertyChangedEventArgs(nameof(Album)));
					(this as IReactiveObject)?.RaisePropertyChanged(new PropertyChangedEventArgs(nameof(SelectedPath)));
					(this as IReactiveObject)?.RaisePropertyChanged(new PropertyChangedEventArgs(nameof(Files)));
				}
			}
		}

		public Album Album { get; protected set; }

		private Dir? m_selectedPath;
		public Dir? SelectedPath 
		{
			get => m_selectedPath;
			set
			{
				m_selectedPath = value;
				Files = m_selectedPath?.Files ?? new List<File>();

				(this as IReactiveObject)?.RaisePropertyChanged(new PropertyChangedEventArgs(nameof(Files)));
			}
		}

		public IEnumerable<File>? Files { get; set; }
	}
}
