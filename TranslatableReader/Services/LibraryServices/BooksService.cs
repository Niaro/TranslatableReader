using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.Storage.Search;
using Template10.Utils;
using TranslatableReader.Models;

namespace TranslatableReader.Services
{
	public class BooksService
	{
		private const string LibraryStorageKey = "LibraryStorage";
		private static StorageFolder _libraryStorage;
		private static ObservableCollection<Book> _books;

		private static BooksService _instance;
		public static BooksService Instance
		{
			get
			{
				if (_instance == null)
					return _instance = new BooksService();
				return _instance;
			}
		}

		private BooksService()
		{
			_books = Task.Run(() => RecoverBooksFromStorageAsync()).Result;
		}

		public async Task<ObservableCollection<Book>> GetBooksAsync()
		{
			if (_books != null)
				return _books;
			return _books = await RecoverBooksFromStorageAsync();
		}

		public async void AddBook()
		{
			var filePicker = new FileOpenPicker
			{
				ViewMode = PickerViewMode.Thumbnail,
				SuggestedStartLocation = PickerLocationId.DocumentsLibrary
			};

			filePicker.FileTypeFilter.Add(".txt");
			filePicker.FileTypeFilter.Add(".fb2");

			var addingBooksOrigins = await filePicker.PickMultipleFilesAsync();
			addingBooksOrigins.ForEach(AddOriginToLibrary);
		}

		private static async void AddOriginToLibrary(StorageFile bookOrigin)
		{
			var newBook = new Book(bookOrigin);

			if (_books.Contains(newBook)) return;

			_books.Add(newBook);

			await AddBookToLibraryAsync(newBook);
		}

		public async Task RemoveBooksFromLibraryAsync(IEnumerable<Book> books)
		{
			foreach (var book in books)
			{
				await book.LibraryStorageFile.DeleteAsync();
				_books.Remove(_books.Single(b=>b.AccessToken == book.AccessToken));
			}
		}

		public void OpenBook()
		{

		}

		public void Search()
		{

		}
		//public ObservableCollection<Message> Search(string value) => GetMessages()
		//	.Where(x => x.Subject.ToLower().Contains(value?.ToLower() ?? string.Empty)
		//				|| x.From.ToLower().Contains(value?.ToLower() ?? string.Empty)
		//				|| x.Body.ToLower().Contains(value?.ToLower() ?? string.Empty))
		//	.ToObservableCollection();

		//public void DeleteMessage(Message selected)
		//{
		//	GetMessages().Remove(selected);
		//}

		//public Message GetMessage(string id) => GetMessages().FirstOrDefault(x => x.Id.Equals(id));

		private static async Task<StorageFolder> GetLibraryStorageAsync()
		{
			if (_libraryStorage == null)
				_libraryStorage = (StorageFolder)await ApplicationData.Current.LocalFolder.TryGetItemAsync(LibraryStorageKey) ??
												 await ApplicationData.Current.LocalFolder.CreateFolderAsync(LibraryStorageKey);

			return _libraryStorage;
		}

		private static async Task<ObservableCollection<Book>> RecoverBooksFromStorageAsync()
		{
			var books = new ObservableCollection<Book>();

			var storagedLibraryBooks = await (await GetLibraryStorageAsync()).GetFilesAsync();
			foreach (var storagedBook in storagedLibraryBooks)
				books.Add(await Book.ConvertToBook(storagedBook));

			return books;
		}

		private static async Task AddBookToLibraryAsync(Book book)
		{
			await book.CreateLibraryBookAsync(await GetLibraryStorageAsync());
		}

	}
}