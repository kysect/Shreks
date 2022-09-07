using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using Config = Kysect.Shreks.ImageGenerator.Configuration.Configuration;

namespace Kysect.Shreks.ImageGenerator.Services;

internal class ConfigurationParser
{
    private readonly JsonSerializer _jsonSerializer;
    private readonly ILogger<ConfigurationParser> _logger;

    internal ConfigurationParser(ILogger<ConfigurationParser> logger)
    {
        _logger = logger;

        var jsonSerializerSettings = new JsonSerializerSettings()
        {
        };
        _jsonSerializer = JsonSerializer.Create(jsonSerializerSettings);
    }

    internal Config Parse(StreamReader configurationFileStream)
    {
        try
        {
            object deserializedObject = _jsonSerializer.Deserialize(configurationFileStream, typeof(Config));
            if (deserializedObject is Config configuration)
            {
                return configuration;
            }

            throw new FormatException("Image generator configuration file is malformed");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to read image generator configuration");
        }

        return Config.NullObject;
    }
}