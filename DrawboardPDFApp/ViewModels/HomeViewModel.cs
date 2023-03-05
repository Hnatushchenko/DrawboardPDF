using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DrawboardPDFApp.Extensions;
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
using Windows.ApplicationModel.Resources;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls;

namespace DrawboardPDFApp.ViewModels
{
    public class HomeViewModel : ObservableObject
    {
        private readonly IOpenedFilesHistoryKeeper openedFilesHistoryKeeper;

        public HomeViewModel(ISortingMethodsProvider sortingMethodsProvider, IPdfOpener pdfOpener, IOpenedFilesHistoryKeeper openedFilesHistoryKeeper)
        {
            OpenPdfFromDeviceCommand = new AsyncRelayCommand(pdfOpener.OpenNewFileAsync);
            this.openedFilesHistoryKeeper = openedFilesHistoryKeeper;
            SortingMethods = sortingMethodsProvider.SortingMethods;
            SelectedSortingMethod = SortingMethods.First();
        }

        private int cloudFilesNumber;
        public int CloudFilesNumber
        {
            get => cloudFilesNumber;
            set => SetProperty(ref cloudFilesNumber, value);
        } 

        private PdfFileInfoSortingMethod selectedSortingMethod;
        public PdfFileInfoSortingMethod SelectedSortingMethod
        {
            get => selectedSortingMethod;
            set
            {
                SetProperty(ref selectedSortingMethod, value);
                SortPdfInfoItems();
            }
        }

        public IAsyncRelayCommand OpenPdfFromDeviceCommand { get; }
        public IEnumerable<PdfFileInfoSortingMethod> SortingMethods { get; set; }
        public NotifyTaskCompletion<ObservableCollection<PdfFileInfo>> OpenedPdfFilesHistoryTask => openedFilesHistoryKeeper.Records;

        private void SortPdfInfoItems()
        {
            OpenedPdfFilesHistoryTask.Result.Sort(SelectedSortingMethod.Comparison);
        }
    }
}
