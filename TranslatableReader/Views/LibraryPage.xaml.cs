using System.Linq;
using Windows.UI.Xaml;
using TranslatableReader.ViewModels;
using Windows.UI.Xaml.Controls;
using Template10.Mvvm;
using TranslatableReader.Models;

namespace TranslatableReader.Views
{
	public sealed partial class LibraryPage : Page
	{
		public LibraryPage()
		{
			InitializeComponent();
			NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Disabled;

		}

		// strongly-typed view models enable x:bind
		public LibraryPageViewModel ViewModel => DataContext as LibraryPageViewModel;

		#region Bottom AppBarBtns animation
		private bool _isMultiSelectMode = false;
		public void ToggleMultiSelectMode()
		{
			_isMultiSelectMode = !_isMultiSelectMode;
			if (_isMultiSelectMode)
			{
				BooksListView.SelectionMode = ListViewSelectionMode.Multiple;
				VisualStateManager.GoToState(ThisPage, "hideNormalModeBtns", true);
			}
			else
			{
				BooksListView.SelectionMode = ListViewSelectionMode.None;
				VisualStateManager.GoToState(ThisPage, "hideMultiSelectModeBtns", true);
			}

		}

		private void hideNormalModeBtns_OnCompleted(object sender, object e)
		{
			ToggleAppBarBtnsVisibility();
			VisualStateManager.GoToState(ThisPage, "showMultiSelectModeBtns", true);
		}
		private void hideMultiSelectModeBtns_OnCompleted(object sender, object e)
		{
			ToggleAppBarBtnsVisibility();
			VisualStateManager.GoToState(ThisPage, "showNormalModeBtns", true);
		}

		private void ToggleAppBarBtnsVisibility()
		{
			AddBookBtn.Visibility = _isMultiSelectMode ? Visibility.Collapsed : Visibility.Visible;
			TurnOnMultiSelectModeBtn.Visibility = _isMultiSelectMode ? Visibility.Collapsed : Visibility.Visible;
			RemoveBooksBtn.Visibility = _isMultiSelectMode ? Visibility.Visible : Visibility.Collapsed;
			TurnOffMultiSelectModeBtn.Visibility = _isMultiSelectMode ? Visibility.Visible : Visibility.Collapsed;
		}
		#endregion

		DelegateCommand _removeBooksCommand;
		public DelegateCommand RemoveBooksCommand => _removeBooksCommand ?? (_removeBooksCommand = new DelegateCommand(async () =>
		{
			if (BooksListView.SelectedItems.Count > 0)
				await ViewModel.RemoveBooksAsync(BooksListView.SelectedItems.Cast<Book>().ToList());
		}, () => BooksListView.SelectedItems.Count > 0));

		private void BooksListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			RemoveBooksCommand.RaiseCanExecuteChanged();
		}

		private void BooksListView_OnItemClick(object sender, ItemClickEventArgs e)
		{
			if (!_isMultiSelectMode)
				ViewModel.OpenBook((Book)e.ClickedItem);
		}
	}
}
