using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Kysect.Shreks.ImageGenerator.Model;
using SkiaSharp;

namespace Kysect.Shreks.ImageGenerator.Services;

internal sealed class ImageBlobStorage : IDisposable
{
    private readonly IReadOnlyDictionary<string, MemoryStream> _blobs;

    internal ImageBlobStorage(IEnumerable<ImageFileModel> models)
    {
        _blobs = models.ToDictionary(model => model.Name, model =>
        {
            using FileStream fileStream = File.OpenRead(model.FullPath);
            var memoryStream = new MemoryStream();
            fileStream.CopyTo(memoryStream);
            return memoryStream;
        });
    }

    public SKBitmap GetBlob(string name)
    {
        if (!_blobs.TryGetValue(name, out MemoryStream stream))
        {
            throw new ArgumentException($"Blob {name} was not found");
        }

        stream.Seek(0, SeekOrigin.Begin);
        return SKBitmap.Decode(stream);
    }

    public void Dispose()
    {
        if (_blobs is null)
        {
            return;
        }

        foreach (MemoryStream memoryStream in _blobs.Values)
        {
            memoryStream.Dispose();
        }
    }
}