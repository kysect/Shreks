using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Kysect.Shreks.Application.Abstractions.ImageGenerator;
using Kysect.Shreks.ImageGenerator.Extensions;
using Kysect.Shreks.ImageGenerator.Model;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.ImageGenerator.Services;

internal sealed class ImageGeneratorService : IImageGeneratorService, IDisposable
{
    private const string ConfigFilePath = "config.json";
    private readonly IReadOnlyDictionary<string, Rectangle> _callouts;
    private readonly ImageBlobStorage _blobStorage;
    private readonly ILogger<ImageGeneratorService> _logger;

    public ImageGeneratorService(string basePath, ConfigurationParser configurationParser, ILogger<ImageGeneratorService> logger)
    {
        if (string.IsNullOrWhiteSpace(basePath))
        {
            throw new ArgumentException("Base path for image generator service cannot be empty", nameof(basePath));
        }

        _logger = logger;

        if (!Directory.Exists(basePath))
        {
            throw new ArgumentException($"Base path '{basePath}' does not exist on file system", nameof(basePath));
        }

        string configFileFullPath = Path.GetFullPath(ConfigFilePath, basePath);
        if (!File.Exists(configFileFullPath))
        {
            throw new FileNotFoundException("Configuration file for image generator was not found", configFileFullPath);
        }

        using StreamReader file = File.OpenText(configFileFullPath);
        ImageFileModel[] imageModels = configurationParser
            .Parse(file)
            .MapToImageFileModel(basePath)
            .Where(FileExists)
            .ToArray();

        _callouts = imageModels.ToDictionary(model => model.Name, model => model.Callout);
        _blobStorage = new ImageBlobStorage(imageModels);
    }

    private bool FileExists(ImageFileModel cfg)
    {
        bool exists = File.Exists(cfg.Name);
        if (!exists)
        {
            _logger.LogWarning("File '{fileName}' does not exist and was excluded from configuration", cfg.Name);
        }

        return exists;
    }

    public void Dispose()
    {
        _blobStorage?.Dispose();
    }
}