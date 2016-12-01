using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using TranslatableReader.Models;
using TranslatableReader.ViewModels;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace TranslatableReader.Views
{
	public sealed partial class BookPage : Page
	{
		// strongly-typed view models enable x:bind
		public BookPageViewModel ViewModel => DataContext as BookPageViewModel;

		public bool IsInitialPositionSet = false;

		private static SimpleOrientation Orientation => SimpleOrientationSensor.GetDefault().GetCurrentOrientation();

		public BookPage()
		{
			InitializeComponent();
			NavigationCacheMode = NavigationCacheMode.Disabled;

			var bookLoad = Observable.FromEventPattern(ViewModel, "BookLoaded").Select(s => s.EventArgs as Book);
			var layoutUpdate = Observable.FromEventPattern(ScrollViewer, "LayoutUpdated");
			var sizeChanged = Observable.FromEventPattern(BookHolder, "SizeChanged");
			var orientationChange = Observable.FromEventPattern(SimpleOrientationSensor.GetDefault(), "OrientationChanged");

			bookLoad.Subscribe(_ => Shell.SetBusy(false));

			var restorePositionRequest = Observable.Merge(
					layoutUpdate.SkipWhile(o => ScrollViewer.ExtentHeight == 0).FirstAsync().Select(_ => (object)null),
					orientationChange.Skip(1).SelectMany(sizeChanged.FirstAsync()).Select(_ => (object)Orientation)
				);

			Observable.CombineLatest(
					bookLoad,
					restorePositionRequest,
					(book, orientation) => new { bookmark = book.Bookmark, orientation }
				)
				.ObserveOn(SynchronizationContext.Current)
				.Subscribe(RestoreBookmarkPosition);

			Observable
				.FromEventPattern(BookHolder, "SelectionChanged")
				.Throttle(TimeSpan.FromSeconds(.5))
				.ObserveOn(SynchronizationContext.Current)
				.Where(pattern => pattern.Sender is RichTextBlock)
				.Select(pattern => (RichTextBlock)pattern.Sender)
				.Subscribe(OnNextSelection);
		}

		private void OnNextSelection(RichTextBlock richTextBlock)
		{
			ViewModel.Translation = richTextBlock.SelectedText;
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			Shell.HamburgerMenu.IsFullScreen = true;
			Shell.SetBusy(true);
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			Shell.HamburgerMenu.IsFullScreen = false;
		}

		private void RestoreBookmarkPosition(dynamic args)
		{
			double offsetIndex = 1;
			Bookmark bookmark = args.bookmark;
			SimpleOrientation? orientation = args.orientation;

			if (orientation == null && bookmark.Orientation != Orientation)
				orientation = Orientation;

			if (orientation != null)
			{
				if (orientation == SimpleOrientation.Rotated90DegreesCounterclockwise || orientation == SimpleOrientation.Rotated270DegreesCounterclockwise)
					offsetIndex = 1.39;
				else
					offsetIndex = 0.72;
			}

			var offset = (bookmark.Position / offsetIndex) / (ScrollViewer.ExtentHeight / ScrollViewer.ActualHeight);
			ScrollViewer.ChangeView(null, offset, null, true);
		}

		private async void ScrollViewer_OnViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
		{
			var offset = ScrollViewer.ExtentHeight / ScrollViewer.ActualHeight * ScrollViewer.VerticalOffset;
			ViewModel.Book.Bookmark = new Bookmark(offset, Orientation);
			await ViewModel.Book.Metadata.SaveAsync();
		}

		private void ScrollViewer_PointerReleased(object sender, PointerRoutedEventArgs e)
		{

		}
	}
}