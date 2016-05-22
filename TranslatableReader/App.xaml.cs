using System;
using System.Threading.Tasks;
using TranslatableReader.Services;
using TranslatableReader.Services.SettingsServices;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI.Xaml;

namespace TranslatableReader
{
	/// Documentation on APIs used in this page:
	/// https://github.com/Windows-XAML/Template10/wiki

	sealed partial class App
	{
		private readonly ISettingsService _settings = SettingsService.Instance;

		public static StorageFolder Library;
		public static BooksService BooksService;

		public App()
		{
			Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
				Microsoft.ApplicationInsights.WindowsCollectors.Metadata |
				Microsoft.ApplicationInsights.WindowsCollectors.Session);

			InitializeComponent();
			SplashFactory = (e) => new Views.Splash(e);

			#region App settings

			RequestedTheme = _settings.AppTheme;
			CacheMaxDuration = _settings.CacheMaxDuration;
			ShowShellBackButton = _settings.UseShellBackButton;

			#endregion App settings
		}

		// runs even if restored from state
		public override async Task OnInitializeAsync(IActivatedEventArgs args)
		{
			// setup hamburger shell
			var nav = NavigationServiceFactory(BackButton.Attach, ExistingContent.Include);
			Window.Current.Content = new Views.Shell(nav);

			Library = await InitilizeLibraryStorageAsync();
			BooksService = await BooksService.InitializeAsync();

			await Task.Yield();
		}

		// runs only when not restored from state
		public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
		{
			// perform long-running load
			await Task.Delay(0);

			if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
				await Windows.UI.ViewManagement.StatusBar.GetForCurrentView().HideAsync();

			// navigate to first page
			NavigationService.Navigate(typeof(Views.LibraryPage));
		}

		private static async Task<StorageFolder> InitilizeLibraryStorageAsync()
		{
			return (StorageFolder)await ApplicationData.Current.LocalFolder.TryGetItemAsync("LibraryStorage") ??
								  await ApplicationData.Current.LocalFolder.CreateFolderAsync("LibraryStorage");
		}
	}
}