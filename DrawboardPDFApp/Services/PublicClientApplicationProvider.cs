using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawboardPDFApp.Services
{
    public class PublicClientApplicationProvider : IPublicClientApplicationProvider
    {
        public PublicClientApplicationProvider(IConfiguration configuration)
        {
            string applicationId = configuration["ApplicationId"];
            string redirectUri = configuration["RedirectUri"];

            PublicClientApplication = PublicClientApplicationBuilder.Create(applicationId)
                .WithRedirectUri(redirectUri)
                .Build();
        }

        public IPublicClientApplication PublicClientApplication { get; }
    }
}
