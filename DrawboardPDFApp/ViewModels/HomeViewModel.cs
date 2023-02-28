﻿using CommunityToolkit.Mvvm.ComponentModel;
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
        private readonly ITabViewService tabViewService;
        private readonly IPdfFileOpenPicker pdfFileOpenPicker;

        public HomeViewModel(ITabViewService tabViewService, IPdfFileOpenPicker pdfFileOpenPicker)
        {
            this.tabViewService = tabViewService;
            this.pdfFileOpenPicker = pdfFileOpenPicker;
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
            var file = await pdfFileOpenPicker.PickSingleFileAsync();

            if (file != null)
            {
                tabViewService.AddTab(file.DisplayName, typeof(OpenedPdfView), file);
            }
        }
    }
}
