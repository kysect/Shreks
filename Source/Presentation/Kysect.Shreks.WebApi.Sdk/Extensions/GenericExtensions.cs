using Newtonsoft.Json;
using System.Text;

namespace Kysect.Shreks.WebApi.Sdk.Extensions;

public static class GenericExtensions
{
    public static HttpContent ToContent<T>(this T obj, JsonSerializerSettings settings)
    {
        string serialized = JsonConvert.SerializeObject(obj, settings);
        return new StringContent(serialized, Encoding.UTF8, "application/json");
    }
}