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
		private string _translation;
		private List<Paragraph> _paragraphs;

		public BooksService BooksService = BooksService.Instance;
		
		public event EventHandler<Book> BookLoaded;

		public BookPageViewModel()
		{
		}

		public Book Book { get; private set; }
	
		public List<Paragraph> Paragraphs
		{
			get { return _paragraphs; }
			set { Set(ref _paragraphs, value); }
		}

		public string Translation
		{
			get { return _translation; }
			set { Set(ref _translation, value); }
		}

		public override async Task OnNavigatedToAsync(object book, NavigationMode mode, IDictionary<string, object> state)
		{
			if (state.ContainsKey(nameof(Book)))
				Book = (Book)state[nameof(Book)];
			else
				Book = BooksService.Books.Single(b => b.Equals(book));

			Paragraphs = await BookDocumentBuilder.BuildAsync(Book);

			BookLoaded?.Invoke(this, Book);

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