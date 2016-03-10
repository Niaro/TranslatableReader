using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Newtonsoft.Json;
using Template10.Mvvm;

namespace TranslatableReader.Models
{
	public class Book : BindableBase
	{
		public string AccessToken { get; set; } = default(string);
		public string Name { get; set; } = default(string);

		[JsonIgnore]
		public StorageFile ContentFile { get; private set; } = default(StorageFile);

		[JsonIgnore]
		public StorageFile LibraryStorageFile { get; private set; }

		public Book()
		{
		}

		public Book(StorageFile bookContentFile)
		{
			ContentFile = bookContentFile;
			Name = ContentFile.Name;
			AccessToken = StorageApplicationPermissions.FutureAccessList.Add(bookContentFile); 
		}

		public static async Task<Book> ConvertToBook(StorageFile libraryStorageFile)
		{
			var serializedBook = await FileIO.ReadTextAsync(libraryStorageFile);
			var book = JsonConvert.DeserializeObject<Book>(serializedBook);
			book.LibraryStorageFile = libraryStorageFile;
			book.ContentFile = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(book.AccessToken); 
			return book;
		}

		public async Task CreateLibraryBookAsync(StorageFolder libraryStorage)
		{
			var serializedBook = JsonConvert.SerializeObject(this);
			LibraryStorageFile = await libraryStorage.CreateFileAsync(Name);
			await FileIO.WriteTextAsync(LibraryStorageFile, serializedBook);
		}

		public override bool Equals(object obj)
		{
			var isEqual = false;
			if (obj is Book)
			{
				var book = obj as Book;
				isEqual = book.AccessToken == AccessToken && book.Name == Name;
			}
			return isEqual;
		}
	}
}
