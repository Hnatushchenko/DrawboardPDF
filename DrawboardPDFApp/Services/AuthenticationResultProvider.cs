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

        public AuthenticationResultProvider(IPublicClientApplication publicClientApplication)
        {
            this.publicClientApplication = publicClientApplication;
        }

        public async Task<AuthenticationResult> GetAuthenticationResultAsync()
        {
            var scopes = new[] { "Files.ReadWrite" };
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
