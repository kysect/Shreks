using System.Globalization;

namespace Kysect.Shreks.Integration.Google.Providers;

public class RuCultureInfoProvider : ICultureInfoProvider
{
    private static readonly CultureInfo RuCultureInfo = new CultureInfo("ru-RU");

    public CultureInfo GetCultureInfo()
    {
        return RuCultureInfo;
    }
}