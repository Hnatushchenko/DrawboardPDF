using Microsoft.Graph;

namespace DrawboardPDFApp.Services
{
    public interface IGraphServiceClientProvider
    {
        GraphServiceClient GraphServiceClient { get; }
    }
}