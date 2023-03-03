using DrawboardPDFApp.Repository;
using DrawboardPDFApp.Services;
using DrawboardPDFApp.ViewModels;
using DrawboardPDFApp.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DrawboardPDFApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        public MainPageViewModel ViewModel { get; set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var tabViewService = new TabViewService(TabView);
            var pdfFileOpenService = App.Current.Services.GetRequiredService<IPdfFileOpenPicker>();
            var openedFilesHistoryKeeper = App.Current.Services.GetRequiredService<IOpenedFilesHistoryKeeper>();
            ViewModel = new MainPageViewModel(tabViewService, pdfFileOpenService, openedFilesHistoryKeeper);
            base.OnNavigatedTo(e);
        }

        private void TabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            sender.TabItems.Remove(args.Tab);
        }

        private void HomePageFrame_Loaded(object sender, RoutedEventArgs e)
        {
            HomePageFrame.Navigate(typeof(HomeView), TabView);
        }
    }
}
