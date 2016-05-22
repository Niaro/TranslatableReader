using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TranslatableReader.Models;
using TranslatableReader.Services;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Text;
using Windows.UI.Text.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Provider;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Navigation;

namespace TranslatableReader.ViewModels
{
	public class BookPageViewModel : Mvvm.ViewModelBase
	{
		public BooksService BooksService = BooksService.Instance;

		public List<Paragraph> BookParagraphs { get; set; } = new List<Paragraph>();

		public List<Paragraph> ViewportBookParagraphs { get; set; } = new List<Paragraph>();
		public Paragraph ActiveParagraph { get; set; }

		public EventHandler OnBookLoad;

		public BookPageViewModel()
		{
		}

		public Book Book { get; private set; }

		public override async Task OnNavigatedToAsync(object book, NavigationMode mode, IDictionary<string, object> state)
		{
			if (state.ContainsKey(nameof(Book)))
				Book = (Book)state[nameof(Book)];
			else
				Book = BooksService.Books.Single(b => b.Equals(book));

			BookParagraphs = await BookDocumentBuilder.BuildAsync(Book);

			int indexTargetParagraph = 0;
			if (Book.Bookmark.Text != null)
			{
				ActiveParagraph = BookParagraphs.Find(p => p.Inlines.Any(x => ((Run)x).Text.Contains(Book.Bookmark.Text)));
				indexTargetParagraph = Math.Abs(BookParagraphs.IndexOf(ActiveParagraph) - 2);
				Debug.WriteLine(indexTargetParagraph + 2);
			}

			ViewportBookParagraphs = BookParagraphs.GetRange(indexTargetParagraph, Math.Min(indexTargetParagraph + 4, BookParagraphs.Count));

			OnBookLoad.Invoke(Book, new EventArgs());

			state.Clear();

			await Task.CompletedTask;
		}

		public override async Task OnNavigatedFromAsync(IDictionary<string, object> state, bool suspending)
		{
			if (suspending)
				state[nameof(Book)] = Book;

			await Book.Metadata.SaveAsync();

			await Task.CompletedTask;
		}
	}
}