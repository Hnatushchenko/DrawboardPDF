using DrawboardPDFApp.Repository;
using DrawboardPDFApp.Services;
using DrawboardPDFApp.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace DrawboardPDFApp
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Windows.UI.Xaml.Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            UnhandledException += OnUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedException;
            Services = ConfigureServices();
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Gets the current <see cref="App"/> instance in use
        /// </summary>
        public new static App Current => (App)Windows.UI.Xaml.Application.Current;

        /// <summary>
        /// Gets the <see cref="IServiceProvider"/> instance to resolve application services.
        /// </summary>
        public IServiceProvider Services { get; }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        private static void OnUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            
        }

        private static void OnUnobservedException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            
        }

        /// <summary>
        /// Configures the services for the application.
        /// </summary>
        private static IServiceProvider ConfigureServices()
        {
            var databasePath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "DrawboardPDFDatabase.db");

            var services = new ServiceCollection();

            services.AddHttpClient();
            services.AddSingleton<IPdfFileOpenPicker, PdfFileOpenPicker>();
            services.AddSingleton<IPublicClientApplicationProvider, PublicClientApplicationProvider>();
            services.AddSingleton<IPublicClientApplication>(serviceProvider =>
            {
                return serviceProvider.GetRequiredService<IPublicClientApplicationProvider>().PublicClientApplication;
            });
            services.AddSingleton<IAuthenticationProvider, CustomAuthenticationProvider>();
            services.AddSingleton<IDriveItemLocalSaver, DriveItemLocalSaver>();
            services.AddTransient<IAuthenticationResultProvider,  AuthenticationResultProvider>();
            services.AddSingleton<GraphServiceClient>(serviceProvider =>
            {
                var authenticationProvider = serviceProvider.GetRequiredService<IAuthenticationProvider>();
                var graphServiceClient = new GraphServiceClient(authenticationProvider);
                return graphServiceClient;
            });
            services.AddSingleton<IOpenedFilesHistoryKeeper, OpenedFilesHistoryKeeper>();
            services.AddTransient<IPdfCoversService, PdfCoversService>();
            services.AddTransient<IDocumentsUploader, DocumentsUploader>();
            services.AddTransient<IPdfOpener, PdfOpener>();
            services.AddSingleton<ILoginManager, LoginManager>();
            services.AddTransient<ICloudStorage, OneDriveStorage>();
            services.AddTransient<IConfiguration>(serviceProvider =>
            {
                var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, true).Build();
                return configuration;
            });
            services.AddSingleton<ISortingMethodsProvider, SortingMethodsProvider>();
            services.AddDbContext<IApplicationContext, ApplicationContext>(options =>
            {
                options.UseSqlite($"Data Source={databasePath}");
            });

            return services.BuildServiceProvider();
        }
    }
}
