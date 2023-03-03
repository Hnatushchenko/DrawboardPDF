using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DrawboardPDFApp.Models;
using DrawboardPDFApp.Repository;
using DrawboardPDFApp.Services;
using DrawboardPDFApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private readonly IPdfOpener pdfOpener;
        private readonly IOpenedFilesHistoryKeeper openedFilesHistoryKeeper;

        public HomeViewModel(IPdfOpener pdfOpener, IOpenedFilesHistoryKeeper openedFilesHistoryKeeper)
        {
            
            OpenPdfFromDeviceCommand = new AsyncRelayCommand(pdfOpener.OpenNewFileAsync);
            this.pdfOpener = pdfOpener;
            this.openedFilesHistoryKeeper = openedFilesHistoryKeeper;
        }

        private int cloudFilesNumber;
        public int CloudFilesNumber
        {
            get => cloudFilesNumber;
            set => SetProperty(ref cloudFilesNumber, value);
        }

        public IAsyncRelayCommand OpenPdfFromDeviceCommand { get; }

        public NotifyTaskCompletion<ObservableCollection<PdfFileInfo>> OpenedPdfFilesHistoryTask => openedFilesHistoryKeeper.Records;
    }
}
