using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
        private readonly IApplicationContext applicationContext;

        public MainPageViewModel(ITabViewService tabViewService, IPdfFileOpenPicker pdfFileOpenPicker, IApplicationContext applicationContext)
        {
            AddTabCommand = new AsyncRelayCommand(AddTab);

            this.tabViewService = tabViewService;
            this.pdfFileOpenPicker = pdfFileOpenPicker;
            this.applicationContext = applicationContext;
        }

        public ICommand AddTabCommand { get; }

        private async Task AddTab()
        {
            var file = await pdfFileOpenPicker.PickSingleFileAsync();

            if (file != null)
            {
                tabViewService.AddTab(file.DisplayName, typeof(OpenedPdfView), file);
            }
        }
    }
}
