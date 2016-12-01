using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using TranslatableReader.Models;
using Windows.UI.Xaml.Documents;

namespace TranslatableReader.Services
{
	interface IBookDocumentBuilder
	{
		List<Paragraph> Build(string bookOriginText);
	}

	public class BookDocumentBuilder
	{
		public double ParagraphTabIndent { get; } = 15;
		public Thickness ParagraphMargin { get; } = new Thickness(0, 0, 0, 0);

		internal static async Task<List<Paragraph>> BuildAsync(Book book)
		{
			var bookOriginText = await FileIO.ReadTextAsync(book.File);
			IBookDocumentBuilder bookDocumentBuilder;
			switch (book.File.FileType)
			{
				case ".txt":
					bookDocumentBuilder = new TxtBookDocumentBuilder();
					break;
				case ".fb2":
					bookDocumentBuilder = new Fb2BookDocumentBuilder();
					break;
				default:
					throw new FormatException("Not supported file format");
			}
			return bookDocumentBuilder.Build(bookOriginText);
		
		}
	}
}
