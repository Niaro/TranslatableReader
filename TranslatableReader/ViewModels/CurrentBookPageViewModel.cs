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
	public class CurrentBookPageViewModel : Mvvm.ViewModelBase
	{
		public BooksService BooksService = BooksService.Instance;

		public CurrentBookPageViewModel()
		{

		}

		string _value = string.Empty;

		public string Value
		{
			get { return _value; }
			set { Set(ref _value, value); }
		}

		public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
		{
			if (state.ContainsKey(nameof(Value)))
				Value = state[nameof(Value)]?.ToString();
			state.Clear();

			return Task.CompletedTask;
		}

		public override async Task OnNavigatedFromAsync(IDictionary<string, object> state, bool suspending)
		{
			if (suspending)
				state[nameof(Value)] = Value;
			await Task.Yield();
		}

		public void BooksListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			
		}
	}
}

