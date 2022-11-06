using Kysect.Shreks.WebApi.Sdk;

namespace Kysect.Shreks.WebUI.AdminPanel.Extensions;

public static class StudentQueryParameterExtensions
{
    public static string AsString(this StudentQueryParameter parameter)
    {
        return parameter switch
        {
            StudentQueryParameter._1 => "Name",
            StudentQueryParameter._2 => "Group",
            StudentQueryParameter._3 => "GitHub",
            _ => throw new ArgumentOutOfRangeException(nameof(parameter), parameter, null)
        };
    }
}