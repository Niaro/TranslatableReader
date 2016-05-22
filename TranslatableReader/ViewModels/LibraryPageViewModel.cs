using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TranslatableReader.Models;
using TranslatableReader.Services;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace TranslatableReader.ViewModels
{
	public class LibraryPageViewModel : Mvvm.ViewModelBase
	{
		public BooksService BooksService = BooksService.Instance;

		private string _value = string.Empty;

		public LibraryPageViewModel()
		{
			Books = BooksService.Books;
			if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
				Value = "Designtime value";
		}

		public ObservableCollection<Book> Books { get; set; }

		public string Value
		{
			get { return _value; }
			set { Set(ref _value, value); }
		}

		public override async Task OnNavigatedFromAsync(IDictionary<string, object> state, bool suspending)
		{
			if (suspending)
				state[nameof(Value)] = Value;
			await Task.CompletedTask;
		}

		public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
		{
			//if (state.ContainsKey(nameof(Value)))
			//	Value = state[nameof(Value)]?.ToString();
			//state.Clear();

			await Task.CompletedTask;
		}

		public async Task OpenBookAsync(Book book)
		{
			await NavigationService.NavigateAsync(typeof(Views.BookPage), book);
		}

		public async Task RemoveBooksAsync(List<Book> books)
		{
			await BooksService.RemoveBooksFromLibraryAsync(books);
			books.ForEach(book => Books.Remove(book));
		}
	}
}