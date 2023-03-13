using CommunityToolkit.Mvvm.ComponentModel;
using DrawboardPDFApp.Models;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace DrawboardPDFApp.Services
{
    public class LoginManager : ObservableObject, ILoginManager
    {
        private readonly IOpenedFilesHistoryKeeper openedFilesHistoryKeeper;
        private readonly IAuthenticationResultProvider authenticationResultProvider;
        private readonly IPublicClientApplication publicClientApplication;

        public LoginManager(IOpenedFilesHistoryKeeper openedFilesHistoryKeeper,
            IAuthenticationResultProvider authenticationResultProvider,
            IPublicClientApplication publicClientApplication)
        {
            this.openedFilesHistoryKeeper = openedFilesHistoryKeeper;
            this.authenticationResultProvider = authenticationResultProvider;
            this.publicClientApplication = publicClientApplication;
        }

        private bool isUserLoggedIn;
        public bool IsUserLoggedIn
        {
            get => isUserLoggedIn;
            set => SetProperty(ref isUserLoggedIn, value);
        }

        public async Task LoginSilentlyIfPossibleAsync()
        {
            if (await authenticationResultProvider.CanAuthenticateSilentlyAsync())
            {
                await LoginAsync();
            }
        }

        public async Task LoginAsync()
        {
            if (!IsUserLoggedIn)
            {
                await openedFilesHistoryKeeper.DownloadRecordsFromCloudAsync();
                IsUserLoggedIn = true;
            }
        }

        public async Task LogoutAsync()
        {
            if (IsUserLoggedIn)
            {
                await ClearTokenCacheAsync();
                openedFilesHistoryKeeper.ClearCloudRecords();
                IsUserLoggedIn = false; 
            }
        }

        private async Task ClearTokenCacheAsync()
        {
            var accounts = await publicClientApplication.GetAccountsAsync();

            foreach (var account in accounts)
            {
                await publicClientApplication.RemoveAsync(account);
            }
        }
    } 
}
