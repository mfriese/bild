using ReactiveUI;
using System.ComponentModel;

namespace Bild.ViewModels
{
	public class ViewModelBase : ReactiveObject
	{
		protected void SendPropertyChanged(string propertyName)
			=> (this as IReactiveObject)?.RaisePropertyChanged(new PropertyChangedEventArgs(propertyName));
	}
}
