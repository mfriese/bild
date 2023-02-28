using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Bild.Environment;
using Bild.Injection;
using Bild.Views;
using Splat;
using System.IO;

namespace Bild
{
	public partial class App : Application
	{
		public override void Initialize()
		{
			AvaloniaXamlLoader.Load(this);
		}

		public override void OnFrameworkInitializationCompleted()
		{
			// Initialize the application for first time runs
			OnFirstRunInitialize();

			// Connect class instances for injection.
			Locator.CurrentMutable.RegisterAllClassInstances();

			if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
			{
				desktop.MainWindow = Locator.Current.GetService<MainWindow>();

				desktop.Exit += OnExit;

				desktop.MainWindow.Show();
			}

			base.OnFrameworkInitializationCompleted();
		}

		protected void OnExit(object? sender, ControlledApplicationLifetimeExitEventArgs args)
		{
			//// Dispose of system tray icon before exiting
			//var tray = Locator.Current.GetService<TrayIcon>();
			//tray.Dispose();

			//ZorroConfigRepo.Save(Locator.Current.GetService<ZorroConfig>());
			//ZorroConfigRepo.Save(Locator.Current.GetService<UploadConfig>());
			//ZorroConfigRepo.Save(Locator.Current.GetService<UploadQueue>());

			//var worker = Locator.Current.GetService<UploadWorker>();
			//worker.Shutdown();
		}

		protected static void OnFirstRunInitialize()
		{
			var fileConfig = new FileConfig();

			if (!Directory.Exists(fileConfig.ConfigPath))
				Directory.CreateDirectory(fileConfig.ConfigPath);
		}
	}
}
