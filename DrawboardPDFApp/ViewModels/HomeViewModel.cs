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
        private readonly IPdfOpener pdfOpener;
        private readonly IOpenedFilesHistoryKeeper openedFilesHistoryKeeper;

        public HomeViewModel(ISortingMethodsProvider sortingMethodsProvider, IPdfOpener pdfOpener, IOpenedFilesHistoryKeeper openedFilesHistoryKeeper)
        {
            OpenPdfFileIfAlreadySelectedCommand = new AsyncRelayCommand<ItemClickEventArgs>(OpenPdfFileIfAlreadySelectedAsync);
            SwitchToGridViewCommand = new RelayCommand(SwitchToGridView, () => !isGridView);
            SwitchToListViewCommand = new RelayCommand(SwitchToListView, () => !IsListView);
            OpenPdfFromDeviceCommand = new AsyncRelayCommand(pdfOpener.OpenNewFileAsync);
            this.pdfOpener = pdfOpener;
            this.openedFilesHistoryKeeper = openedFilesHistoryKeeper;
            SortingMethods = sortingMethodsProvider.SortingMethods;
            SelectedSortingMethod = SortingMethods.First();
            IsGridView = true; 
        }

        private int cloudFilesNumber;
        public int CloudFilesNumber
        {
            get => cloudFilesNumber;
            set => SetProperty(ref cloudFilesNumber, value);
        }

        private bool isListView;
        public bool IsListView
        {
            get { return isListView; }
            set { SetProperty(ref isListView, value); }
        }

        private bool isGridView;
        public bool IsGridView
        {
            get { return isGridView; }
            set { SetProperty(ref isGridView, value); }
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

        private PdfFileInfo selectedPdfFile;
        public PdfFileInfo SelectedPdfFile
        {
            get => selectedPdfFile;
            set
            {
                SetProperty(ref selectedPdfFile, value);
            }
        }

        public ICommand OpenPdfFileIfAlreadySelectedCommand { get; }
        public IRelayCommand SwitchToListViewCommand { get; }
        public IRelayCommand SwitchToGridViewCommand { get; }
        public IAsyncRelayCommand OpenPdfFromDeviceCommand { get; }
        public IEnumerable<PdfFileInfoSortingMethod> SortingMethods { get; set; }
        public NotifyTaskCompletion<ObservableCollection<PdfFileInfo>> OpenedPdfFilesHistoryTask => openedFilesHistoryKeeper.Records;

        private void SortPdfInfoItems()
        {
            OpenedPdfFilesHistoryTask.Result.Sort(SelectedSortingMethod.Comparison);
        }

        private void SwitchToListView()
        {
            IsListView = true;
            IsGridView = false;
            SwitchToGridViewCommand.NotifyCanExecuteChanged();
            SwitchToListViewCommand.NotifyCanExecuteChanged();
        }

        private void SwitchToGridView()
        {
            IsListView = false;
            IsGridView = true;
            SwitchToGridViewCommand.NotifyCanExecuteChanged();
            SwitchToListViewCommand.NotifyCanExecuteChanged();
        }

        private PdfFileInfo previouslyClickedPdfFile;

        private async Task OpenPdfFileIfAlreadySelectedAsync(ItemClickEventArgs e)
        {
            if (e.ClickedItem is PdfFileInfo pdfFile)
            {
                if (previouslyClickedPdfFile == pdfFile)
                {
                    await pdfOpener.OpenExistingFileAsync(pdfFile.FileToken);
                }
                previouslyClickedPdfFile = pdfFile;
            }
        }
    }
}
