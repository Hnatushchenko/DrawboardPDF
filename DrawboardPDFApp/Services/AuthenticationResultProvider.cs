using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawboardPDFApp.Services
{
    public class AuthenticationResultProvider : IAuthenticationResultProvider
    {
        private readonly IPublicClientApplication publicClientApplication;
        private readonly IReadOnlyCollection<string> scopes;

        public AuthenticationResultProvider(IPublicClientApplication publicClientApplication)
        {
            this.publicClientApplication = publicClientApplication;
            scopes = new List<string>
            {
                "Files.ReadWrite",
            }.AsReadOnly();
        }

        public async Task<bool> CanAuthenticateSilentlyAsync()
        {
            var accounts = await publicClientApplication.GetAccountsAsync();
            try
            {
                await publicClientApplication.AcquireTokenSilent(scopes, accounts.FirstOrDefault()).ExecuteAsync();
                return true;
            }
            catch (MsalUiRequiredException)
            {
                return false;
            }
        }

        public async Task<AuthenticationResult> GetAuthenticationResultAsync()
        {
            var accounts = await publicClientApplication.GetAccountsAsync();
            AuthenticationResult authenticationResult;
            try
            {
                authenticationResult = await publicClientApplication.AcquireTokenSilent(scopes, accounts.FirstOrDefault()).ExecuteAsync();
            }
            catch (MsalUiRequiredException)
            {
                authenticationResult = await publicClientApplication.AcquireTokenInteractive(scopes).ExecuteAsync();
            }
            return authenticationResult;
        }
    }
}
