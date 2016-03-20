using Windows.Foundation;
using Windows.UI.Xaml;
using TranslatableReader.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace TranslatableReader.Views
{
	public sealed partial class BookPage : Page
	{
		// strongly-typed view models enable x:bind
		public BookPageViewModel ViewModel => DataContext as BookPageViewModel;

		public BookPage()
		{
			InitializeComponent();
			NavigationCacheMode = NavigationCacheMode.Disabled;
			
			ViewModel.OnBookLoad += (o, e) =>
			{
				ViewModel.BookParagraphs.ForEach(BookHolder.Blocks.Add);
			
				Shell.SetBusy(false);
			};
		}

	

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			Shell.HamburgerMenu.IsFullScreen = true;
			Shell.SetBusy(true);
			
			//int index;
			//if (int.TryParse(e.Parameter?.ToString(), out index))
			//	MyPivot.SelectedIndex = index;
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			Shell.HamburgerMenu.IsFullScreen = false;
		}

		private void RichTextBlock_OnSelectionChanged(object sender, RoutedEventArgs e)
		{
			
		}

		private void BookHolder_OnLayoutUpdated(object sender, object e)
		{
			var t = 1;
		}

		private void BookHolder_OnManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
		{
			var t = 1;
		}

		private void ScrollViewer_OnDirectManipulationCompleted(object sender, object e)
		{
			var t = 1;
		}

		private void ScrollViewer_OnViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
		{
			var t = BookHolder.GetPositionFromPoint(new Point(0, ScrollViewer.VerticalOffset));
		}
	}
}

