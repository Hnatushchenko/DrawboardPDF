using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DrawboardPDFApp.Enums;
using DrawboardPDFApp.Models;
using DrawboardPDFApp.Repository;
using DrawboardPDFApp.Services;
using DrawboardPDFApp.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage.Pickers;

namespace DrawboardPDFApp.ViewModels
{
    public class MainPageViewModel : ObservableObject
    {
        private readonly ITabViewService tabViewService;
        private readonly IPdfFileOpenPicker pdfFileOpenPicker;
        private readonly IOpenedFilesHistoryKeeper openedFilesHistoryKeeper;
        private readonly ILoginManager loginManager;

        public MainPageViewModel(ITabViewService tabViewService,
            IPdfFileOpenPicker pdfFileOpenPicker,
            IOpenedFilesHistoryKeeper openedFilesHistoryKeeper,
            ILoginManager loginManager)
        {
            AddTabCommand = new AsyncRelayCommand(AddTabAsync);
            ExecuteStartupCommand = new AsyncRelayCommand(ExecuteStartupAsync);

            this.tabViewService = tabViewService;
            this.pdfFileOpenPicker = pdfFileOpenPicker;
            this.openedFilesHistoryKeeper = openedFilesHistoryKeeper;
            this.loginManager = loginManager;
        }

        public IAsyncRelayCommand ExecuteStartupCommand { get; }
        public ICommand AddTabCommand { get; }

        private async Task ExecuteStartupAsync()
        {
            await loginManager.LoginSilentlyIfPossibleAsync();
        }

        private async Task AddTabAsync()
        {
            var file = await pdfFileOpenPicker.PickSingleFileAsync();
            if (file is null)
            {
                return;
            }

            await openedFilesHistoryKeeper.RecordFileOpeningAsync(file, Location.Local);
            tabViewService.AddTab(file.DisplayName, typeof(OpenedPdfView), file);
        }
    }
}
