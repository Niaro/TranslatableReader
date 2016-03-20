using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Text;
using Windows.UI.Text.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Navigation;
using TranslatableReader.Models;
using TranslatableReader.Services;

namespace TranslatableReader.ViewModels
{
	public class BookPageViewModel : Mvvm.ViewModelBase
	{
		public BooksService BooksService = BooksService.Instance;

		public List<Paragraph> BookParagraphs { get; set; } = new List<Paragraph>();

		public EventHandler OnBookLoad;

		public BookPageViewModel()
		{

		}

		public Book Book { get; private set; }

		public override async Task OnNavigatedToAsync(object bookOriginAccessToken, NavigationMode mode, IDictionary<string, object> state)
		{
			if (state.ContainsKey(nameof(Book)))
				Book = (Book)state[nameof(Book)];
			else
				Book = BooksService.GetBook((string)bookOriginAccessToken);

			BookParagraphs = await BookDocumentBuilder.BuildAsync(Book);
			OnBookLoad.Invoke(Book, new EventArgs());

			state.Clear();

			await Task.CompletedTask;
		}

		public override async Task OnNavigatedFromAsync(IDictionary<string, object> state, bool suspending)
		{
			if (suspending)
				state[nameof(Book)] = Book;

			await Task.CompletedTask;
		}
		
	}
}

