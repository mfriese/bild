using ReactiveUI;
using System.ComponentModel;

namespace Bild.ViewModels
{
	public class ViewModelBase : ReactiveObject
	{
		protected void SetProperty<T>(ref T value, T newValue, string propertyName)
		{
			if (!Equals(value, newValue))
			{
				value = newValue;

				(this as IReactiveObject)?.RaisePropertyChanged(
					new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
