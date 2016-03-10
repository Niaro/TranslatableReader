using TranslatableReader.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace TranslatableReader.Views
{
	public sealed partial class CurrentBookPage : Page
	{
		public CurrentBookPage()
		{
			InitializeComponent();
			NavigationCacheMode = NavigationCacheMode.Disabled;
		}

		// strongly-typed view models enable x:bind
		public CurrentBookPageViewModel ViewModel => DataContext as CurrentBookPageViewModel;

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			//int index;
			//if (int.TryParse(e.Parameter?.ToString(), out index))
			//	MyPivot.SelectedIndex = index;
		}
	}
}

