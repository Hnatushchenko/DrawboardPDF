using DrawboardPDFApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace DrawboardPDFApp.Services
{
    public class PdfOpener : IPdfOpener
    {
        private readonly ITabViewService tabViewService;
        private readonly IPdfFileOpenPicker pdfFileOpenPicker;
        private readonly IOpenedFilesHistoryKeeper openedFilesHistoryKeeper;

        public PdfOpener(ITabViewService tabViewService, IPdfFileOpenPicker pdfFileOpenPicker, IOpenedFilesHistoryKeeper openedFilesHistoryKeeper)
        {
            this.tabViewService = tabViewService;
            this.pdfFileOpenPicker = pdfFileOpenPicker;
            this.openedFilesHistoryKeeper = openedFilesHistoryKeeper;
        }

        public async Task OpenNewFileAsync()
        {
            var file = await pdfFileOpenPicker.PickSingleFileAsync();
            if (file is null)
            {
                return;
            }

            await OpenExistingFileAsync(file);
        }

        public async Task OpenExistingFileAsync(StorageFile file)
        {
            await openedFilesHistoryKeeper.RecordFileOpeningAsync(file);
            tabViewService.AddTab(file.DisplayName, typeof(OpenedPdfView), file);
        }
    }
}
