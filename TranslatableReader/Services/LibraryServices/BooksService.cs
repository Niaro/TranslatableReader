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
using Newtonsoft.Json;
using Template10.Utils;
using TranslatableReader.Models;

namespace TranslatableReader.Services
{
	public class BooksService
	{
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
			_libraryStorage = Task.Run(() => InitilizeLibraryStorageAsync()).Result;
			_books = Task.Run(() => RecoverBooksFromLibraryStorageAsync()).Result;
		}

		private static async Task<StorageFolder> InitilizeLibraryStorageAsync()
		{
			return (StorageFolder)await ApplicationData.Current.LocalFolder.TryGetItemAsync("LibraryStorage") ??
								  await ApplicationData.Current.LocalFolder.CreateFolderAsync("LibraryStorage");
		}

		public async Task<ObservableCollection<Book>> GetBooksAsync()
		{
			if (_books != null)
				return _books;
			return _books = await RecoverBooksFromLibraryStorageAsync();
		}

		public Book GetBook(string bookOriginAccessToken)
		{
			return  _books.Single(b => b.OriginAccessToken == bookOriginAccessToken);
		}

		private static async Task<ObservableCollection<Book>> RecoverBooksFromLibraryStorageAsync()
		{
			var books = new ObservableCollection<Book>();

			var libraryBooksFiles = await _libraryStorage.GetFilesAsync();
			foreach (var libraryBookFile in libraryBooksFiles)
			{
				var book = await TryConvertLibraryBookFileToBookAsync(libraryBookFile);
				if (book != null)
					books.Add(book);
			}

			return books;
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

			var addingOriginsBooksFiles = await filePicker.PickMultipleFilesAsync();
			addingOriginsBooksFiles.ForEach(AddOriginBookFileToLibrary);
		}

		private static async void AddOriginBookFileToLibrary(StorageFile originBookFile)
		{
			var newBook = new Book(originBookFile);

			if (_books.Contains(newBook)) return;

			_books.Add(newBook);

			await CreateLibraryBookAsync(newBook);
		}

		private static async Task CreateLibraryBookAsync(Book book)
		{
			var serializedBook = JsonConvert.SerializeObject(book);
			var libraryBookFile = await _libraryStorage.CreateFileAsync(book.Name);
			await FileIO.WriteTextAsync(libraryBookFile, serializedBook);
		}

		private static async Task<Book> TryConvertLibraryBookFileToBookAsync(StorageFile libraryBookFile)
		{
			var serializedBook = await FileIO.ReadTextAsync(libraryBookFile);
			var book = JsonConvert.DeserializeObject<Book>(serializedBook);

			book.LibraryBookFile = libraryBookFile;
			try
			{
				book.OriginFile = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(book.OriginAccessToken);
				return book;
			}
			catch (Exception)
			{
				return null;
			}
		}


		public async Task RemoveBooksFromLibraryAsync(IEnumerable<Book> books)
		{
			foreach (var book in books)
			{
				await book.LibraryBookFile.DeleteAsync();
				_books.Remove(_books.Single(b=>b.OriginAccessToken == book.OriginAccessToken));
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

		
	}
}