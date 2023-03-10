using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using Windows.Storage;

namespace DrawboardPDFApp.Services
{
    public class DocumentsUploader : IDocumentsUploader
    {
        private readonly IPdfFileOpenPicker pdfFileOpenPicker;
        private readonly IOpenedFilesHistoryKeeper openedFilesHistoryKeeper;
        private readonly ICloudStorage cloudStorage;

        public DocumentsUploader(IPdfFileOpenPicker pdfFileOpenPicker,
            IOpenedFilesHistoryKeeper openedFilesHistoryKeeper,
            ICloudStorage cloudStorage)
        {
            this.pdfFileOpenPicker = pdfFileOpenPicker;
            this.openedFilesHistoryKeeper = openedFilesHistoryKeeper;
            this.cloudStorage = cloudStorage;
        }

        public async Task UploadNewDocumentAsync()
        {
            var fileToUpload = await pdfFileOpenPicker.PickSingleFileAsync();
            await UploadFileAsync(fileToUpload);
        }

        public async Task UploadFileAsync(StorageFile file)
        {
            await cloudStorage.AddFileAsync(file);
            await openedFilesHistoryKeeper.RecordFileOpeningAsync(file, Enums.Location.Cloud);
        }
    }
}
