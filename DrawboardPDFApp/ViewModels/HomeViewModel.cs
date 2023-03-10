using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DrawboardPDFApp.Extensions;
using DrawboardPDFApp.Models;
using DrawboardPDFApp.Repository;
using DrawboardPDFApp.Services;
using DrawboardPDFApp.Views;
using Microsoft.Graph;
using Microsoft.Identity.Client;
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
        private readonly IDocumentsUploader documentsUploader;
        private readonly IPdfOpener pdfOpener;
        private readonly IOpenedFilesHistoryKeeper openedFilesHistoryKeeper;
        private readonly ILoginManager loginManager;

        public HomeViewModel(IDocumentsUploader documentsUploader, ISortingMethodsProvider sortingMethodsProvider, IPdfOpener pdfOpener, IOpenedFilesHistoryKeeper openedFilesHistoryKeeper,
            ILoginManager loginManager)
        {
            LoginCommand = new AsyncRelayCommand(LoginAsync);
            LogoutCommand = new AsyncRelayCommand(LogoutAsync);
            OpenPdfFileIfAlreadySelectedCommand = new AsyncRelayCommand<ItemClickEventArgs>(OpenPdfFileIfAlreadySelectedAsync);
            SwitchToGridViewCommand = new RelayCommand(SwitchToGridView, () => !isGridView);
            SwitchToListViewCommand = new RelayCommand(SwitchToListView, () => !IsListView);
            UploadDocumentCommand = new AsyncRelayCommand(documentsUploader.UploadNewDocumentAsync);
            OpenPdfFromDeviceCommand = new AsyncRelayCommand(pdfOpener.OpenNewFileAsync);
            this.documentsUploader = documentsUploader;
            this.pdfOpener = pdfOpener;
            this.openedFilesHistoryKeeper = openedFilesHistoryKeeper;
            this.loginManager = loginManager;
            SortingMethods = sortingMethodsProvider.SortingMethods;
            SelectedSortingMethod = SortingMethods.First();
            IsGridView = true;
            OpenedPdfFilesHistoryTask.PropertyChanged += OpenedPdfFilesHistoryTask_PropertyChanged;
        }

        private void OpenedPdfFilesHistoryTask_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(OpenedPdfFilesHistoryTask.Result))
            {
                SortPdfInfoItems();
            }
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

        private bool isUserLoggedIn;
        public bool IsUserLoggedIn
        {
            get { return isUserLoggedIn; }
            set { SetProperty(ref isUserLoggedIn, value); }
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

        public IAsyncRelayCommand LoginCommand { get; }
        public IAsyncRelayCommand LogoutCommand { get; }
        public IAsyncRelayCommand UploadDocumentCommand { get; }
        public ICommand OpenPdfFileIfAlreadySelectedCommand { get; }
        public IRelayCommand SwitchToListViewCommand { get; }
        public IRelayCommand SwitchToGridViewCommand { get; }
        public IAsyncRelayCommand OpenPdfFromDeviceCommand { get; }
        public IEnumerable<PdfFileInfoSortingMethod> SortingMethods { get; set; }
        public NotifyTaskCompletion<ObservableCollection<PdfFileInfo>> OpenedPdfFilesHistoryTask => openedFilesHistoryKeeper.Records;

        private void SortPdfInfoItems()
        {
            OpenedPdfFilesHistoryTask.Result?.Sort(SelectedSortingMethod.Comparison);
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

        private async Task LoginAsync()
        {
            await loginManager.LoginAsync();
            IsUserLoggedIn = true;
        }

        private async Task LogoutAsync()
        {
            await loginManager.LogoutAsync();
            IsUserLoggedIn = false;
        }
    }
}
