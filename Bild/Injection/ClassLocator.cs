using Bild.Core.Environment;
using Bild.Environment;
using Bild.ViewModels;
using Bild.Views;
using Splat;

namespace Bild.Injection
{
	public static class ClassLocator
	{
		public static void RegisterAllClassInstances(this IMutableDependencyResolver locator)
		{
			// Data classes to be registered
			Locator.CurrentMutable.RegisterConstant<IFileConfig>(new FileConfig());
			Locator.CurrentMutable.RegisterConstant(new Repository(
				Locator.Current.GetService<IFileConfig>()));

			// UI ViewModels to be registered
			Locator.CurrentMutable.RegisterConstant(new MainWindowViewModel(
				Locator.Current.GetService<Repository>()));

			// UI Classes to be registered
			Locator.CurrentMutable.RegisterConstant(new MainWindow()
			{
				DataContext = Locator.Current.GetService<MainWindowViewModel>()
			});
		}
	}
}
