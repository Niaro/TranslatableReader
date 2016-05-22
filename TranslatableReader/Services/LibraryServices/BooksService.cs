using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Template10.Utils;
using TranslatableReader.Models;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;

namespace TranslatableReader.Services
{
	public class BooksService
	{
		private BooksService()
		{
		}

		public static BooksService Instance { get; private set; }

		public ObservableCollection<Book> Books { get; private set; }

		public async void AddBookAsync()
		{
			var filePicker = new FileOpenPicker
			{
				ViewMode = PickerViewMode.Thumbnail,
				SuggestedStartLocation = PickerLocationId.DocumentsLibrary
			};

			filePicker.FileTypeFilter.Add(".txt");
			filePicker.FileTypeFilter.Add(".fb2");

			var addingOriginsBooksFiles = await filePicker.PickMultipleFilesAsync();
			addingOriginsBooksFiles.ForEach(AddBookFileToLibrary);
		}

		public void OpenBook()
		{
		}

		public async Task RemoveBooksFromLibraryAsync(IEnumerable<Book> books)
		{
			foreach (var book in books)
			{
				await book.DeleteAsync();
				Books.Remove(Books.Single(b => Equals(b, book)));
			}
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

		private void AddBookFileToLibrary(StorageFile originFile)
		{
			var newBook = new Book(originFile);

			if (!Books.Contains(newBook))
				Books.Add(newBook);
		}

		private static async Task<ObservableCollection<Book>> RecoverBooksFromLibraryStorageAsync()
		{
			var books = new ObservableCollection<Book>();

			var bookFiles = await App.Library.GetFilesAsync();
			foreach (var bookFile in bookFiles)
			{
				var book = await Book.ReadFromFileAsync(bookFile);
				if (book != null)
					books.Add(book);
			}

			return books;
		}

		public static async Task<BooksService> InitializeAsync()
		{
			Instance = new BooksService
			{
				Books = await RecoverBooksFromLibraryStorageAsync()
			};
			return Instance;
		}
	}
}