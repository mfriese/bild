using Splat;

namespace Bild.Injection
{
	public static class ClassLocator
	{
		public static void RegisterAllClassInstances(this IMutableDependencyResolver locator)
		{
			//var zorroConfig = ZorroConfigRepo.LoadZorroConfig();
			//var uploadQueue = ZorroConfigRepo.LoadUploadQueue();
			//var uploadConfig = ZorroConfigRepo.LoadUploadConfig();

			//Locator.CurrentMutable.RegisterConstant(zorroConfig);
			//Locator.CurrentMutable.RegisterConstant(uploadConfig);
			//Locator.CurrentMutable.RegisterConstant(uploadQueue);
			//Locator.CurrentMutable.RegisterConstant<IMsgBus>(new MsgBus("Z:UrlHandler"));

			//Locator.CurrentMutable.RegisterConstant<IClient>(new ZorroClient(
			//	zorroConfig, uploadConfig));

			//Locator.CurrentMutable.RegisterConstant(new GroupUploader(
			//	Locator.Current.GetService<ZorroConfig>(),
			//	Locator.Current.GetService<UploadQueue>(),
			//	Locator.Current.GetService<IClient>()));
			//Locator.CurrentMutable.RegisterConstant(new TrayIcon());
			//var trayUpdater = new LimiterCallback(new TrayInfoUpdateOperation(
			//	Locator.Current.GetService<TrayIcon>(),
			//	Locator.Current.GetService<UploadQueue>()), 1000);
			//Locator.CurrentMutable.RegisterConstant(new UploadWorker(
			//	Locator.Current.GetService<GroupUploader>(), trayUpdater));

			//// UI components
			//Locator.CurrentMutable.RegisterConstant(new MainWindow
			//{
			//	DataContext = new MainWindowViewModel(
			//	Locator.Current.GetService<UploadQueue>(),
			//	Locator.Current.GetService<UploadConfig>(),
			//	Locator.Current.GetService<IClient>())
			//});

			//Locator.CurrentMutable.RegisterConstant(new TrayWindow
			//{
			//	DataContext = new TrayWindowViewModel(
			//	Locator.Current.GetService<UploadQueue>())
			//});

			//Locator.CurrentMutable.Register(() => new SettingsWindow()
			//{
			//	DataContext = new SettingsWindowViewModel(
			//	Locator.Current.GetService<ZorroConfig>(),
			//	Locator.Current.GetService<UploadConfig>())
			//});
		}
	}
}
