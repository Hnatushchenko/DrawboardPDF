using DrawboardPDFApp.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;

namespace DrawboardPDFApp.Services
{
    public class OneDriveStorage : ICloudStorage
    {
        private readonly GraphServiceClient graphServiceClient;
        private readonly string cloudFolderName;
        private readonly IDriveItemLocalSaver driveItemLocalSaver;

        public OneDriveStorage(GraphServiceClient graphServiceClient,
            IConfiguration configuration,
            IDriveItemLocalSaver driveItemLocalSaver)
        {
            this.graphServiceClient = graphServiceClient;
            cloudFolderName = configuration["CloudFolderName"];
            this.driveItemLocalSaver = driveItemLocalSaver;
        }

        public async Task<IEnumerable<StorageFile>> GetAllPdfFilesAsync()
        {
            await CreateCloudFolderIfAbsentAsync();
            
            var cloudFolderId = await GetCloudFolderIdAsync();

            var cloudFolderChilder = await graphServiceClient.Me.Drive.Items[cloudFolderId].Children.Request().GetAsync();

            var driveFiles = cloudFolderChilder.Where(c => c.File != null).ToList();

            var donwloadedPdfFiles = new List<StorageFile>(driveFiles.Count);
            foreach (var driveItem in driveFiles)
            {
                var storageFile = await driveItemLocalSaver.SaveToLocalStorageAsync(driveItem);
                donwloadedPdfFiles.Add(storageFile);
            }
            return donwloadedPdfFiles;
        }

        private async Task CreateCloudFolderAsync()
        {
            var folder = new DriveItem
            {
                Name = cloudFolderName,
                Folder = new Folder(),
                AdditionalData = new Dictionary<string, object>()
                {
                    {
                        "@microsoft.graph.conflictBehavior", "fail"
                    }
                }
            };

            await graphServiceClient.Me.Drive.Root.Children.Request()
                .AddAsync(folder);
        }

        private async Task CreateCloudFolderIfAbsentAsync()
        {
            if (!await ExistsCloudFolderAsync())
            {
                await CreateCloudFolderAsync();
            }
        }

        private async Task<bool> ExistsCloudFolderAsync()
        {
            var driveItems = await graphServiceClient.Me.Drive.Root.Children.Request().GetAsync();
            bool exists = driveItems.Any(item => item.Name == cloudFolderName);
            return exists;
        }

        private async Task<string> GetCloudFolderIdAsync()
        {
            var driveItems = await graphServiceClient.Me.Drive.Root.Children.Request().GetAsync();
            var cloudFolder = driveItems.Where(item => item.Name == cloudFolderName).First();
            return cloudFolder.Id;
        }

        public async Task AddFileAsync(StorageFile file)
        {
            using (var stream = await file.OpenStreamForReadAsync())
            {
                var uploadResult = await graphServiceClient.Me.Drive.Root
                    .ItemWithPath($"/{cloudFolderName}/{file.Name}").Content
                    .Request()
                    .PutAsync<DriveItem>(stream);
            }
        }

        public async Task RemoveFileAsync(string fileName)
        {
            await graphServiceClient.Me.Drive.Root.ItemWithPath($"/{cloudFolderName}/{fileName}")
                .Request()
                .DeleteAsync();
        }
    }
}
