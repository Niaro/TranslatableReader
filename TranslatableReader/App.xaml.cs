using System;
using Windows.UI.Xaml;
using System.Threading.Tasks;
using TranslatableReader.Services.SettingsServices;
using Windows.ApplicationModel.Activation;

namespace TranslatableReader
{
	/// Documentation on APIs used in this page:
	/// https://github.com/Windows-XAML/Template10/wiki

	sealed partial class App
	{
		ISettingsService _settings;

		public App()
		{
			Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
				Microsoft.ApplicationInsights.WindowsCollectors.Metadata |
				Microsoft.ApplicationInsights.WindowsCollectors.Session);
			InitializeComponent();
			SplashFactory = (e) => new Views.Splash(e);

			#region App settings

			_settings = SettingsService.Instance;
			RequestedTheme = _settings.AppTheme;
			CacheMaxDuration = _settings.CacheMaxDuration;
			ShowShellBackButton = _settings.UseShellBackButton;

			#endregion
		}

		// runs even if restored from state
		public override async Task OnInitializeAsync(IActivatedEventArgs args)
		{
			// setup hamburger shell
			var nav = NavigationServiceFactory(BackButton.Attach, ExistingContent.Include);
			Window.Current.Content = new Views.Shell(nav);
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
	}
}
