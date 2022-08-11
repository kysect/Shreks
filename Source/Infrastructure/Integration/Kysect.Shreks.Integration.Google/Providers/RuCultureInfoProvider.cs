using System.Globalization;

namespace Kysect.Shreks.Integration.Google.Providers;

public class RuCultureInfoProvider : ICultureInfoProvider
{
    private static readonly CultureInfo RuCultureInfo = new("ru-RU");

    public CultureInfo CultureInfo => RuCultureInfo;
}