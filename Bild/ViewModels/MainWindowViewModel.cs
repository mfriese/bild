using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Bild.Core.Environment;
using Bild.Core.Files;
using Bild.Core.Importer;
using Bild.Environment;
using ReactiveUI;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive;

namespace Bild.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		public MainWindowViewModel(Repository repository)
		{
			ImportFolder = ReactiveCommand.Create(ImportFolderImpl);
			OpenProject = ReactiveCommand.Create(OpenProjectImpl);
			OpenFolder = ReactiveCommand.Create<string>(OpenFolderImpl);
			Repository = repository;
			
			Album = new Album(Repository.Settings);
			
		}

		public Repository Repository { get; }
		public ReactiveCommand<Unit, Unit> ImportFolder { get; }
		public ReactiveCommand<Unit, Unit> OpenProject { get; }
		public ReactiveCommand<string, Unit> OpenFolder { get; }

		void OpenFolderImpl(string absolutePath)
		{
			var proc = new Process();
			proc.StartInfo.FileName = SystemSpecifics.FileExplorer;
			proc.StartInfo.Arguments = absolutePath;
			proc.Start();
		}

		void ImportFolderImpl()
		{
			var lifetime = Avalonia.Application.Current?.ApplicationLifetime;

			if (lifetime is IClassicDesktopStyleApplicationLifetime desktop)
			{
				var dialog = new OpenFolderDialog();
				var findDirTask = dialog.ShowAsync(desktop.MainWindow);
				var foundDir = findDirTask.GetAwaiter().GetResult();

				if (!string.IsNullOrEmpty(foundDir))
				{
					var findings = ImportFinder.FindAll(foundDir);

					findings.ForEach(ff => ff.Treatment = ImportTreatment.Overwrite);
					
					findings.ForEach(ff => Album.ImportItem(ff));
				}
			}
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

					SetProperty(ref m_album, new Album(Repository.Settings), nameof(Album));
					SetProperty(ref m_selectedPath, null, nameof(SelectedPath));
					SetProperty(ref m_files, null, nameof(Files));
				}
			}
		}

		protected Album? m_album;
		public Album Album { get; protected set; }

		private Dir? m_selectedPath;
		public Dir? SelectedPath 
		{
			get => m_selectedPath;
			set
			{
				SetProperty(ref m_selectedPath, value, nameof(SelectedPath));
				SetProperty(ref m_files, m_selectedPath?.Files, nameof(Files));
			}
		}

		protected IEnumerable<File>? m_files;
		public IEnumerable<File>? Files
		{
			get => m_files;
			set => SetProperty(ref m_files, value, nameof(Files));
		}

		public string ProjectPath => Repository.Settings.ProjectFolder;
	}
}
