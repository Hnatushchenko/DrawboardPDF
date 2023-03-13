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
using System.ComponentModel;
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
        private readonly ISortingMethodsProvider sortingMethodsProvider;
        private readonly IPdfOpener pdfOpener;
        private readonly IOpenedFilesHistoryKeeper openedFilesHistoryKeeper;
        private readonly ILoginManager loginManager;

        public HomeViewModel(IDocumentsUploader documentsUploader, ISortingMethodsProvider sortingMethodsProvider, IPdfOpener pdfOpener, IOpenedFilesHistoryKeeper openedFilesHistoryKeeper,
            ILoginManager loginManager)
        {
            SwitchToAllFilesCommand = new RelayCommand(SwitchToAllFiles);
            SwitchToCloudFilesCommand = new RelayCommand(SwitchToCloudFiles);
            LoginCommand = new AsyncRelayCommand(LoginAsync);
            LogoutCommand = new AsyncRelayCommand(LogoutAsync);
            OpenPdfFileIfAlreadySelectedCommand = new AsyncRelayCommand<ItemClickEventArgs>(OpenPdfFileIfAlreadySelectedAsync);
            SwitchToGridViewCommand = new RelayCommand(SwitchToGridView, () => !isGridView);
            SwitchToListViewCommand = new RelayCommand(SwitchToListView, () => !IsListView);
            UploadDocumentCommand = new AsyncRelayCommand(UploadNewDocumentAsync);
            OpenPdfFromDeviceCommand = new AsyncRelayCommand(pdfOpener.OpenNewFileAsync);

            this.documentsUploader = documentsUploader;
            this.sortingMethodsProvider = sortingMethodsProvider;
            this.pdfOpener = pdfOpener;
            this.openedFilesHistoryKeeper = openedFilesHistoryKeeper;
            this.loginManager = loginManager;

            loginManager.PropertyChanged += LoginManager_PropertyChanged;

            SortingMethods = sortingMethodsProvider.SortingMethods;
            SelectedSortingMethod = sortingMethodsProvider.SelectedMethod;
            IsGridView = true;
            SwitchToAllFiles();
        }

        private void LoginManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsUserLoggedIn))
            {
                OnPropertyChanged(nameof(IsUserLoggedIn));
            }
        }


        private bool isListView;
        public bool IsListView
        {
            get { return isListView; }
            set { SetProperty(ref isListView, value); }
        }

        public bool IsUserLoggedIn => loginManager.IsUserLoggedIn;

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
                if (SetProperty(ref selectedSortingMethod, value))
                {
                    sortingMethodsProvider.SelectedMethod = value;
                    SortPdfInfoItems();
                }
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

        private ObservableCollection<PdfFileInfo> pdfFiles;
        public ObservableCollection<PdfFileInfo> PdfFiles
        {
            get { return pdfFiles; }
            set { SetProperty(ref pdfFiles, value); }
        }

        public ICommand SwitchToCloudFilesCommand { get; }
        public ICommand SwitchToAllFilesCommand { get; }
        public IAsyncRelayCommand LoginCommand { get; }
        public IAsyncRelayCommand LogoutCommand { get; }
        public IAsyncRelayCommand UploadDocumentCommand { get; }
        public ICommand OpenPdfFileIfAlreadySelectedCommand { get; }
        public IRelayCommand SwitchToListViewCommand { get; }
        public IRelayCommand SwitchToGridViewCommand { get; }
        public IAsyncRelayCommand OpenPdfFromDeviceCommand { get; }
        public IEnumerable<PdfFileInfoSortingMethod> SortingMethods { get; set; }
        public ObservableCollection<PdfFileInfo> AllRecords => openedFilesHistoryKeeper.AllRecords;
        public ObservableCollection<PdfFileInfo> CloudRecords => openedFilesHistoryKeeper.CloudRecords;
        

        private void SwitchToCloudFiles()
        {
            PdfFiles = CloudRecords;
        }

        private void SwitchToAllFiles()
        {
            PdfFiles = AllRecords;
        }

        private void SortPdfInfoItems()
        {
            CloudRecords.Sort(SelectedSortingMethod.Comparison);
            AllRecords.Sort(SelectedSortingMethod.Comparison);
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
            try
            {
                await loginManager.LoginAsync();
            }
            catch (ServiceException serviceException)
                when (serviceException.InnerException is MsalClientException msalClientException &&
                msalClientException.ErrorCode == MsalError.AuthenticationCanceledError)
            {

            }
        }

        private async Task LogoutAsync()
        {
            await loginManager.LogoutAsync();
        }

        private async Task UploadNewDocumentAsync()
        {
            try
            {
                await documentsUploader.UploadNewDocumentAsync();
            }
            catch (ServiceException serviceException)
                when (serviceException.InnerException is MsalClientException msalClientException &&
                msalClientException.ErrorCode == MsalError.AuthenticationCanceledError)
            {

            }
        }
    }
}
