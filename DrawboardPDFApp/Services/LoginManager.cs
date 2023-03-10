using DrawboardPDFApp.Models;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace DrawboardPDFApp.Services
{
    public class LoginManager : ILoginManager
    {
        private readonly IOpenedFilesHistoryKeeper openedFilesHistoryKeeper;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IAuthenticationResultProvider authenticationResultProvider;
        private readonly IPublicClientApplication publicClientApplication;

        public LoginManager(IOpenedFilesHistoryKeeper openedFilesHistoryKeeper,
            IHttpClientFactory httpClientFactory,
            IAuthenticationResultProvider authenticationResultProvider,
            IPublicClientApplication publicClientApplication)
        {
            this.openedFilesHistoryKeeper = openedFilesHistoryKeeper;
            this.httpClientFactory = httpClientFactory;
            this.authenticationResultProvider = authenticationResultProvider;
            this.publicClientApplication = publicClientApplication;
        }

        public async Task LoginAsync()
        {
            await openedFilesHistoryKeeper.DownloadRecordsFromCloudAsync();
        }

        public async Task LogoutAsync()
        {
            var authResult = await authenticationResultProvider.GetAuthenticationResultAsync();
            string accessToken = authResult.AccessToken;
            HttpClient httpClient = httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage response = await httpClient.GetAsync("https://login.microsoftonline.com/common/oauth2/v2.0/logout");

            var accounts = await publicClientApplication.GetAccountsAsync();

            foreach (var account in accounts)
            {
                await publicClientApplication.RemoveAsync(account);
            }
        }
    }
}
