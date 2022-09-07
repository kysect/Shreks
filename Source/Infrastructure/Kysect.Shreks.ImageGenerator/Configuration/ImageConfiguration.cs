using System;
using System.Drawing;

namespace Kysect.Shreks.ImageGenerator.Configuration;

[Serializable]
public sealed class ImageConfiguration
{
    public Rectangle Callout { get; init; }

    public string FileName { get; init; }
}