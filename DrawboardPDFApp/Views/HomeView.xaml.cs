using DrawboardPDFApp.Repository;
using DrawboardPDFApp.Services;
using DrawboardPDFApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DrawboardPDFApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomeView : Page
    {
        public HomeViewModel ViewModel { get; set; }

        public HomeView()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is TabView tabView)
            {
                var tabViewService = new TabViewService(tabView);
                var pdfFileOpenPicker = App.Current.Services.GetRequiredService<IPdfFileOpenPicker>();
                var openedFilesHistoryKeeper = App.Current.Services.GetRequiredService<IOpenedFilesHistoryKeeper>();
                var pdfOpener = new PdfOpener(tabViewService, pdfFileOpenPicker, openedFilesHistoryKeeper);
                var sortingMethodsProvider = App.Current.Services.GetRequiredService<ISortingMethodsProvider>();
                var documentUploader = App.Current.Services.GetRequiredService<IDocumentsUploader>();
                var loginManager = App.Current.Services.GetRequiredService<ILoginManager>();
                ViewModel = new HomeViewModel(documentUploader, sortingMethodsProvider, pdfOpener, openedFilesHistoryKeeper, loginManager);
                base.OnNavigatedTo(e);
            }
            else
            {
                throw new ArgumentException("Parameter should be of type TabView");
            }
        }

        private void Button_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Hand, 0);
        }

        private void Button_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 1);
        }
    }
}
