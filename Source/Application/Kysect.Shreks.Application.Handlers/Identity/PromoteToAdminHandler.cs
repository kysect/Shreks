using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Identity.Entities;
using Kysect.Shreks.Identity.Extensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using static Kysect.Shreks.Application.Contracts.Identity.Commands.PromoteToAdmin;

namespace Kysect.Shreks.Application.Handlers.Identity;

internal class PromoteToAdminHandler : IRequestHandler<Command>
{
    private readonly RoleManager<ShreksIdentityRole> _roleManager;
    private readonly UserManager<ShreksIdentityUser> _userManager;

    public PromoteToAdminHandler(
        UserManager<ShreksIdentityUser> userManager,
        RoleManager<ShreksIdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
    {
        ShreksIdentityUser? user = await _userManager.FindByNameAsync(request.Username);

        if (user is null)
            throw new EntityNotFoundException("User with such username does not exist");

        await _roleManager.CreateRoleIfNotExistsAsync(ShreksIdentityRole.AdminRoleName);
        if (await _userManager.IsInRoleAsync(user, ShreksIdentityRole.AdminRoleName))
            return Unit.Value;

        await _userManager.AddToRoleAsync(user, ShreksIdentityRole.AdminRoleName);
        return Unit.Value;
    }
}