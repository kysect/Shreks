using Kysect.Shreks.Application.Contracts.Identity.Commands;
using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.WebApi.Abstractions.Models;
using MediatR;

namespace Kysect.Shreks.WebApi.Helpers;

internal static class SeedingHelper
{
    internal static async Task SeedAdmins(IServiceProvider provider, IConfiguration configuration)
    {
        IMediator mediatr = provider.GetRequiredService<IMediator>();
        ILogger<Program> logger = provider.GetRequiredService<ILogger<Program>>();
        IConfigurationSection adminsSection = configuration.GetSection("Identity:DefaultAdmins");
        AdminModel[] admins = adminsSection.Get<AdminModel[]>() ?? Array.Empty<AdminModel>();

        foreach (AdminModel admin in admins)
        {
            try
            {
                var registerCommand = new Register.Command(admin.Username, admin.Password);
                await mediatr.Send(registerCommand);

                var promoteCommand = new PromoteToAdmin.Command(admin.Username);
                await mediatr.Send(promoteCommand);
            }
            catch (RegistrationFailedException e)
            {
                logger.LogWarning(e, "Failed registration of {AdminUsername}", admin.Username);
            }
        }
    }
}