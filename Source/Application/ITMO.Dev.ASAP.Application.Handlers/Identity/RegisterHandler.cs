using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Identity.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using static ITMO.Dev.ASAP.Application.Contracts.Identity.Commands.Register;

namespace ITMO.Dev.ASAP.Application.Handlers.Identity;

internal class RegisterHandler : IRequestHandler<Command>
{
    private readonly UserManager<AsapIdentityUser> _userManager;

    public RegisterHandler(UserManager<AsapIdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
    {
        AsapIdentityUser? existingUser = await _userManager.FindByNameAsync(request.Username);

        if (existingUser is not null)
            throw new RegistrationFailedException("User with given name already exists");

        var user = new AsapIdentityUser { UserName = request.Username, SecurityStamp = Guid.NewGuid().ToString() };

        IdentityResult? result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            throw new RegistrationFailedException(string.Join(' ', result.Errors.Select(r => r.Description)));

        return Unit.Value;
    }
}