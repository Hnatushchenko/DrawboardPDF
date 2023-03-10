using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DrawboardPDFApp.Services
{
    public class CustomAuthenticationProvider : IAuthenticationProvider
    {
        private readonly IAuthenticationResultProvider authenticationResultProvider;

        public CustomAuthenticationProvider(IAuthenticationResultProvider authenticationResultProvider)
        {
            this.authenticationResultProvider = authenticationResultProvider;
        }

        public async Task AuthenticateRequestAsync(HttpRequestMessage request)
        {
            var authenticationResult = await authenticationResultProvider.GetAuthenticationResultAsync();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authenticationResult.AccessToken);
        }
    }
}
