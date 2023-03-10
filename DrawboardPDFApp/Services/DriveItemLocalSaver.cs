using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace DrawboardPDFApp.Services
{
    public class DriveItemLocalSaver : IDriveItemLocalSaver
    {
        private readonly GraphServiceClient graphServiceClient;

        public DriveItemLocalSaver(IGraphServiceClientProvider graphServiceClientProvider)
        {
            graphServiceClient = graphServiceClientProvider.GraphServiceClient;
        }

        public async Task<StorageFile> SaveToLocalStorageAsync(DriveItem driveItem)
        {
            var driveItemStream = await graphServiceClient.Me.Drive.Items[driveItem.Id].Content.Request().GetAsync();

            var localFolder = ApplicationData.Current.LocalFolder;
            var localFile = await localFolder.CreateFileAsync(driveItem.Name, CreationCollisionOption.ReplaceExisting);

            using (var pdfStream = await localFile.OpenStreamForWriteAsync())
            {
                await driveItemStream.CopyToAsync(pdfStream);
            }
            return localFile;
        }
    }
}
