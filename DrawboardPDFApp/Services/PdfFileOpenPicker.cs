using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace DrawboardPDFApp.Services
{
    public class PdfFileOpenPicker : IPdfFileOpenPicker
    {
        private readonly FileOpenPicker fileOpenPicker;

        public PdfFileOpenPicker()
        {
            fileOpenPicker = new FileOpenPicker();
            fileOpenPicker.ViewMode = PickerViewMode.Thumbnail;
            fileOpenPicker.FileTypeFilter.Add(".pdf");
        }

        public async Task<StorageFile> PickSingleFileAsync()
        {
            var storageFile = await fileOpenPicker.PickSingleFileAsync();
            return storageFile;
        }
    }
}
