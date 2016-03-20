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
		public string OriginAccessToken { get; set; } = default(string);
		public string Name { get; set; } = default(string);

		[JsonIgnore]
		public StorageFile OriginFile { get; set; } = default(StorageFile);

		[JsonIgnore]
		public StorageFile LibraryBookFile { get; set; }

		public Book()
		{
		}

		public Book(StorageFile originBookFile)
		{
			OriginFile = originBookFile;
			Name = OriginFile.Name;
			OriginAccessToken = StorageApplicationPermissions.FutureAccessList.Add(originBookFile); 
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
	}
}
