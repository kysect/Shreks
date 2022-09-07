using System.Drawing;

namespace Kysect.Shreks.ImageGenerator.Model;

public class ImageFileModel
{
    public string FullPath { get; init; }

    public string Name { get; init; }

    public Rectangle Callout { get; init; }
}