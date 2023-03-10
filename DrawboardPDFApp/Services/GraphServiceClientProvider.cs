using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DrawboardPDFApp.Services
{
    public class GraphServiceClientProvider : IGraphServiceClientProvider
    {
        public GraphServiceClientProvider(IAuthenticationResultProvider authenticationResultProvider)
        {
            GraphServiceClient = new GraphServiceClient(new DelegateAuthenticationProvider(async (requestMessage) =>
            {
                var authenticationResult = await authenticationResultProvider.GetAuthenticationResultAsync();
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authenticationResult.AccessToken);
            }));
        }

        public GraphServiceClient GraphServiceClient { get; }
    }
}
