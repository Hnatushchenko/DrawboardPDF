using DrawboardPDFApp.Models;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace DrawboardPDFApp.Services
{
    public class LoginManager : ILoginManager
    {
        private readonly IOpenedFilesHistoryKeeper openedFilesHistoryKeeper;

        public LoginManager(IOpenedFilesHistoryKeeper openedFilesHistoryKeeper)
        {
            this.openedFilesHistoryKeeper = openedFilesHistoryKeeper;
        }

        public async Task LoginAsync()
        {
            await openedFilesHistoryKeeper.DownloadRecordsFromCloudAsync();
        }
    }
}
