using DrawboardPDFApp.Enums;
using DrawboardPDFApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;

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
            if (file != null)
            {
                await OpenExistingFileAsync(file, Location.Local);
            }
        }

        public async Task OpenExistingFileAsync(StorageFile file, Location location)
        {
            await openedFilesHistoryKeeper.RecordFileOpeningAsync(file, location);
            tabViewService.AddTabOrSelectIfIsOpened(file.DisplayName, typeof(OpenedPdfView), file);
        }

        public async Task OpenExistingFileAsync(string fileToken)
        {
            var file = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(fileToken);
            await OpenExistingFileAsync(file, Location.Local);
        }
    }
}
