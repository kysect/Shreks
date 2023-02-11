using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Identity.Entities;
using ITMO.Dev.ASAP.Identity.Extensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using static ITMO.Dev.ASAP.Application.Contracts.Identity.Commands.PromoteToAdmin;

namespace ITMO.Dev.ASAP.Application.Handlers.Identity;

internal class PromoteToAdminHandler : IRequestHandler<Command>
{
    private readonly RoleManager<AsapIdentityRole> _roleManager;
    private readonly UserManager<AsapIdentityUser> _userManager;

    public PromoteToAdminHandler(
        UserManager<AsapIdentityUser> userManager,
        RoleManager<AsapIdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
    {
        AsapIdentityUser? user = await _userManager.FindByNameAsync(request.Username);

        if (user is null)
            throw new EntityNotFoundException("User with such username does not exist");

        await _roleManager.CreateRoleIfNotExistsAsync(AsapIdentityRole.AdminRoleName);
        if (await _userManager.IsInRoleAsync(user, AsapIdentityRole.AdminRoleName))
            return Unit.Value;

        await _userManager.AddToRoleAsync(user, AsapIdentityRole.AdminRoleName);
        return Unit.Value;
    }
}