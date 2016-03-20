using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using TranslatableReader.Models;
using TranslatableReader.Services;

namespace TranslatableReader.ViewModels
{
	public class LibraryPageViewModel : Mvvm.ViewModelBase
	{
		public BooksService BooksService = BooksService.Instance;

		public LibraryPageViewModel()
		{
			if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
				Value = "Designtime value";
		}

		string _value = string.Empty;

		public string Value
		{
			get { return _value; }
			set { Set(ref _value, value); }
		}

		public ObservableCollection<Book> Books { get; set; } = default(ObservableCollection<Book>);

		public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
		{
			//if (state.ContainsKey(nameof(Value)))
			//	Value = state[nameof(Value)]?.ToString();
			//state.Clear();

			Books = await BooksService.GetBooksAsync();

			await Task.CompletedTask;
		}

		public override async Task OnNavigatedFromAsync(IDictionary<string, object> state, bool suspending)
		{
			if (suspending)
				state[nameof(Value)] = Value;
			await Task.CompletedTask;
		}

		public async Task RemoveBooksAsync(List<Book> books)
		{
			await BooksService.RemoveBooksFromLibraryAsync(books);
			books.ForEach(book => Books.Remove(book));
		}

		public void OpenBook(Book book)
		{
			NavigationService.Navigate(typeof(Views.BookPage), book.OriginAccessToken);
		}
	}
}

