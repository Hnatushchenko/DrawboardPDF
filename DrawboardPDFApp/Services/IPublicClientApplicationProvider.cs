using Microsoft.Identity.Client;

namespace DrawboardPDFApp.Services
{
    public interface IPublicClientApplicationProvider
    {
        IPublicClientApplication PublicClientApplication { get; }
    }
}