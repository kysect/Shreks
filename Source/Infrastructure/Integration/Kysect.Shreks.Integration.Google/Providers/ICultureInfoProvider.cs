using System.Globalization;

namespace Kysect.Shreks.Integration.Google.Providers;

public interface ICultureInfoProvider
{
    CultureInfo GetCultureInfo();
}