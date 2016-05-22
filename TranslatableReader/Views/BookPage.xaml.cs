using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TranslatableReader.Models;
using TranslatableReader.ViewModels;
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
	public class ListViewRichTextBlockItem
	{
		public List<Paragraph> Source { get; set; } = new List<Paragraph>();

		public ListViewRichTextBlockItem()
		{
		}

		public ListViewRichTextBlockItem(Paragraph bp)
		{
			Source.Add(bp);
		}
	}

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
			  //ViewModel.ViewportBookParagraphs.ForEach(BookHolder.Blocks.Add);
			  //ViewModel.BookParagraphs.ForEach(BookHolder.Blocks.Add);
			  //List<string> test = (await FileIO.ReadTextAsync(ViewModel.Book.File)).Split(new char[] { '\n' }).ToList();
			  RichTextViewer.Source = ViewModel.BookParagraphs;
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
			//Debug.WriteLine($"RichTextBlock_OnSelectionChanged");
		}

		private void BookHolder_OnLayoutUpdated(object sender, object e)
		{
			//if (ScrollViewer.VerticalOffset == 0 && ViewModel.Book != null)
			//{
			//	ScrollViewer.ScrollToVerticalOffset(ViewModel.Book.Bookmark.GetVerticalOffset(ScrollViewer.ViewportWidth));
			//	Debug.WriteLine($"Bookmark.VerticalOffset: {ViewModel.Book.Bookmark.GetVerticalOffset(ScrollViewer.ViewportWidth)}");
			//};

			//Debug.WriteLine($"ScrollViewer.VerticalOffset: {ScrollViewer.VerticalOffset}");
		}

		private void BookHolder_OnManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
		{
			Debug.WriteLine($"BookHolder_OnManipulationCompleted");
		}

		private void ScrollViewer_OnDirectManipulationCompleted(object sender, object e)
		{
			Debug.WriteLine($"ScrollViewer_OnDirectManipulationCompleted");
		}

		private void ScrollViewer_OnViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
		{
			//var topOfViewportTextPointer = BookHolder.GetPositionFromPoint(new Point(0, ScrollViewer.VerticalOffset));

			//var start = topOfViewportTextPointer.GetPositionAtOffset(0, LogicalDirection.Forward);
			//var end = topOfViewportTextPointer.GetPositionAtOffset(Bookmark.TextWidth, LogicalDirection.Forward);

			//BookHolder.Select(start, end);
			//ViewModel.Book.Bookmark = new Bookmark(BookHolder.SelectedText, ScrollViewer.ViewportWidth, ScrollViewer.VerticalOffset);
			//BookHolder.Select(start, start);
		}
	}
}