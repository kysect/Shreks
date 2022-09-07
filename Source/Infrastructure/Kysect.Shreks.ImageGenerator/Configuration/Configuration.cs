using System;
using System.Collections.Generic;

namespace Kysect.Shreks.ImageGenerator.Configuration;

[Serializable]
public sealed class Configuration
{
    public IEnumerable<ImageConfiguration> Images { get; init; }

    public static Configuration NullObject => new()
    {
        Images = Array.Empty<ImageConfiguration>()
    };
}