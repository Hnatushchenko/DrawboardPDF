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
        private readonly ILoginManager loginManager;

        public DocumentsUploader(IPdfFileOpenPicker pdfFileOpenPicker,
            IOpenedFilesHistoryKeeper openedFilesHistoryKeeper,
            ICloudStorage cloudStorage,
            ILoginManager loginManager)
        {
            this.pdfFileOpenPicker = pdfFileOpenPicker;
            this.openedFilesHistoryKeeper = openedFilesHistoryKeeper;
            this.cloudStorage = cloudStorage;
            this.loginManager = loginManager;
        }

        public async Task UploadNewDocumentAsync()
        {
            var fileToUpload = await pdfFileOpenPicker.PickSingleFileAsync();
            if (fileToUpload != null)
            {
                await UploadFileAsync(fileToUpload); 
            }
        }

        public async Task UploadFileAsync(StorageFile file)
        {
            await loginManager.LoginAsync();
            await cloudStorage.AddFileAsync(file);
            await openedFilesHistoryKeeper.AddCloudRecordIfNotExistAsync(file);
        }
    }
}
