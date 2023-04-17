using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Yottabyte.Services.Contracts;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;

namespace Yottabyte.Services.Implementations;

internal class CustomVisionService : ICustomVisionService
{
    private readonly IConfiguration configuration;
    private CustomVisionPredictionClient predictionClient;
    
    public CustomVisionService(IConfiguration configuration)
    {
        this.configuration = configuration;

        predictionClient = new(new ApiKeyServiceClientCredentials(this.configuration["AzureCustomVision:PredictionKey"]))
        {
            Endpoint = this.configuration["AzureCustomVision:PredictionEndpoint"]
        };
    }
    
    public async Task<bool> IsBeachPollutedAsync(IFormFile file)
    {
        var result = await this.predictionClient.ClassifyImageAsync(
                Guid.Parse(this.configuration["AzureCustomVision:PredictionModeId"]),
                this.configuration["AzureCustomVision:PredictionModelPublishedName"],
                file.OpenReadStream());

        var unclearProb = 0d;
        var clearProb = 0d;

        foreach (var prediction in result.Predictions)
        {
            if (prediction.TagName == "Clear")
            {
                clearProb = prediction.Probability;
            }
            else if (prediction.TagName == "Unclear")
            {
                unclearProb = prediction.Probability;
            }
        }

        return unclearProb > clearProb;
    }
}
