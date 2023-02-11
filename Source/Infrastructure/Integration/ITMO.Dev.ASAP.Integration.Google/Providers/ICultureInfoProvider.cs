using System.Globalization;

namespace ITMO.Dev.ASAP.Integration.Google.Providers;

public interface ICultureInfoProvider
{
    CultureInfo GetCultureInfo();
}