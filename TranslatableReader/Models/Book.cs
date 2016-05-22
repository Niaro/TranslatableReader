using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.Storage;
using Windows.Storage.AccessCache;

namespace TranslatableReader.Models
{
	public class Book : BindableBase
	{
		private Bookmark _bookmark = new Bookmark();
		private string _name = default(string);
		private string _originAccessToken = default(string);

		public Bookmark Bookmark
		{
			get { return _bookmark; }
			set { Set(ref _bookmark, value); }
		}

		public string Name
		{
			get { return _name; }
			set { Set(ref _name, value); }
		}

		public string OriginAccessToken
		{
			get { return _originAccessToken; }
			set { Set(ref _originAccessToken, value); }
		}

		[JsonIgnore]
		public StorageFile File { get; set; } = default(StorageFile);

		[JsonIgnore]
		public BookMetadata Metadata { get; set; }

		public Book()
		{
		}

		public Book(StorageFile origin)
		{
			_originAccessToken = StorageApplicationPermissions.FutureAccessList.Add(origin);
			Initialize(origin);
		}

		public async Task DeleteAsync()
		{
			await Metadata.DeleteAsync();
		}

		public override bool Equals(object obj)
		{
			var isEqual = false;
			if (obj is Book)
			{
				var book = obj as Book;
				isEqual = book.OriginAccessToken == OriginAccessToken && book.Name == Name;
			}
			return isEqual;
		}

		private async void Initialize(StorageFile origin, bool isRestore = false)
		{
			File = origin;
			_name = File.DisplayName;
			Metadata = new BookMetadata(this);
			if (!isRestore)
				await Metadata.SaveAsync();
		}

		private async Task<Book> RestoreAfterJsonDeserializeAsync()
		{
			Initialize(await StorageApplicationPermissions.FutureAccessList.GetFileAsync(_originAccessToken), true);
			return this;
		}

		public static async Task<Book> ReadFromFileAsync(StorageFile bookFile)
		{
			var serializedBook = await FileIO.ReadTextAsync(bookFile);
			try
			{
				return await JsonConvert.DeserializeObject<Book>(serializedBook).RestoreAfterJsonDeserializeAsync();
			}
			catch (Exception)
			{
				return null;
			}
		}
	}

	public class BookMetadata
	{
		private Book Book { get; }

		private IStorageFile File { get; set; }

		private string Name { get; }

		public BookMetadata(Book book)
		{
			Book = book;
			Name = $"{book.Name}.ini";
		}

		public async Task DeleteAsync()
		{
			await File?.DeleteAsync();
		}

		public async Task SaveAsync()
		{
			await FileIO.WriteTextAsync(await GetFileAsync(), JsonConvert.SerializeObject(Book));
		}

		private async Task<IStorageFile> GetFileAsync()
		{
			if (File != null) return File;
			try
			{
				File = await App.Library.GetFileAsync(Name);
			}
			catch (FileNotFoundException)
			{
				File = await App.Library.CreateFileAsync(Name);
			}
			return File;
		}
	}

	[JsonObject]
	public class Bookmark
	{
		public static int TextWidth = 150;

		public string Text { get; set; }
		public double VerticalOffset { get; set; }

		public double ViewportWidth { get; set; } = 1;

		public Bookmark()
		{
		}

		public Bookmark(string text, double viewportWidth, double verticalOffset)
		{
			Text = text;
			ViewportWidth = viewportWidth;
			VerticalOffset = verticalOffset;
		}

		public double GetVerticalOffset(double actualViewportWidth)
		{
			return actualViewportWidth * VerticalOffset / ViewportWidth;
		}
	}
}