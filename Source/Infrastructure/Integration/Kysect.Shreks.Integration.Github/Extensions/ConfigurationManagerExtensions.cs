using Kysect.Shreks.Integration.Github.Helpers;
using Microsoft.Extensions.Configuration;

namespace Kysect.Shreks.Integration.Github.Extensions;

public static class ConfigurationManagerExtensions
{
    public static ShreksConfiguration GetShreksConfiguration(this ConfigurationManager configuration)
    {
        var shreksConfiguration = configuration.GetSection(nameof(ShreksConfiguration)).Get<ShreksConfiguration>();
        shreksConfiguration.AppendSecret(configuration["GithubAppSecret"]).Verify();
        return shreksConfiguration;
    }
}