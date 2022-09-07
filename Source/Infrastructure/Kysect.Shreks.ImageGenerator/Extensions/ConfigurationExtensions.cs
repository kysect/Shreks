using System.Collections.Generic;
using System.IO;
using System.Linq;
using Kysect.Shreks.ImageGenerator.Model;

using Config = Kysect.Shreks.ImageGenerator.Configuration.Configuration;

namespace Kysect.Shreks.ImageGenerator.Extensions;

internal static class ConfigurationExtensions
{
    internal static IEnumerable<ImageFileModel> MapToImageFileModel(
        this Config configuration,
        string basePath)
    {
        return configuration
            .Images
            .Select(cfg => new ImageFileModel
            {
                Name = Path.GetFileNameWithoutExtension(cfg.FileName),
                FullPath = Path.GetFullPath(cfg.FileName, basePath),
                Callout = cfg.Callout
            });
    }
}