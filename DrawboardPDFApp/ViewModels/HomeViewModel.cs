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
        private readonly ITabViewService tabViewService;
        private readonly IPdfFileOpenPicker pdfFileOpenPicker;
        private readonly IOpenedFilesHistoryKeeper openedFilesHistoryKeeper;

        public HomeViewModel(ITabViewService tabViewService, IPdfFileOpenPicker pdfFileOpenPicker, IOpenedFilesHistoryKeeper openedFilesHistoryKeeper)
        {
            this.tabViewService = tabViewService;
            this.pdfFileOpenPicker = pdfFileOpenPicker;
            this.openedFilesHistoryKeeper = openedFilesHistoryKeeper;
            OpenPdfFromDeviceCommand = new AsyncRelayCommand(OpenPdfFromDeviceAsync);
            OpenedPdfFilesHistoryTask = new NotifyTaskCompletion<ObservableCollection<PdfFileInfo>>(LoadPdfFilesHistoryAsObservable());
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

        private async Task<ObservableCollection<PdfFileInfo>> LoadPdfFilesHistoryAsObservable()
        {
            var openedPdfFilesHistory = await openedFilesHistoryKeeper.GetAllPdfFilesAsync();
            var observableOpenedPdfFilesHistory = new ObservableCollection<PdfFileInfo>(openedPdfFilesHistory);
            return observableOpenedPdfFilesHistory;
        }

        private async Task OpenPdfFromDeviceAsync()
        {
            var file = await pdfFileOpenPicker.PickSingleFileAsync();
            if (file is null)
            {
                return;
            }
            
            tabViewService.AddTab(file.DisplayName, typeof(OpenedPdfView), file);
            if (await openedFilesHistoryKeeper.RecordExistsAsync(file))
            {
                await openedFilesHistoryKeeper.UpdateAsync(file);
            }
            else
            {
                var fileInfo = await openedFilesHistoryKeeper.AddRecordAsync(file);
                OpenedPdfFilesHistoryTask.Result.Add(fileInfo);
                AllFilesNumber = OpenedPdfFilesHistoryTask.Result.Count;
            }
        }

        public NotifyTaskCompletion<ObservableCollection<PdfFileInfo>> OpenedPdfFilesHistoryTask { get; private set; }
    }
}
