using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DrawboardPDFApp.Services;
using DrawboardPDFApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls;

namespace DrawboardPDFApp.ViewModels
{
    public class HomeViewModel : ObservableObject
    {
        private readonly INavigationService navigationService;

        public HomeViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
            OpenPdfFromDeviceCommand = new AsyncRelayCommand(OpenPdfFromDeviceAsync);
        }

        private int allFilesNumber;
        public int AllFilesNumber
        {
            get => allFilesNumber;
            set => SetProperty(ref allFilesNumber, value);
        }

        private int cloudFilesNumber;
        public int CloudFilesNumber
        {
            get => cloudFilesNumber;
            set => SetProperty(ref cloudFilesNumber, value);
        }

        public IAsyncRelayCommand OpenPdfFromDeviceCommand { get; }

        private async Task OpenPdfFromDeviceAsync()
        {
            var picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.FileTypeFilter.Add(".pdf");

            var file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                navigationService.Navigate(typeof(OpenedPdfView), file);
            }
        }
    }
}
