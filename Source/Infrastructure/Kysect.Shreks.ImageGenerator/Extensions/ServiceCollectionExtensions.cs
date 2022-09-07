using Kysect.Shreks.Application.Abstractions.ImageGenerator;
using Kysect.Shreks.ImageGenerator.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.ImageGenerator.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddImageGenerationService(this IServiceCollection serviceCollection, string basePath)
    {
        return serviceCollection
            .AddSingleton<IImageGeneratorService>(sp =>
            {
                var configurationParserLogger = sp.GetRequiredService<ILogger<ConfigurationParser>>();
                var configurationParser = new ConfigurationParser(configurationParserLogger);

                var imageGeneratorLogger = sp.GetRequiredService<ILogger<ImageGeneratorService>>();
                return new ImageGeneratorService(basePath, configurationParser, imageGeneratorLogger);
            });
    }
}