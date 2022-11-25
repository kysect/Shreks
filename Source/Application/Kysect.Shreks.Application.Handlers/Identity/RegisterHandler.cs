using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Identity.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using static Kysect.Shreks.Application.Contracts.Identity.Commands.Register;

namespace Kysect.Shreks.Application.Handlers.Identity;

public class RegisterHandler : IRequestHandler<Command>
{
    private readonly UserManager<ShreksIdentityUser> _userManager;

    public RegisterHandler(UserManager<ShreksIdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
    {
        var existingUser = await _userManager.FindByNameAsync(request.Username);

        if (existingUser is not null)
            throw new RegistrationFailedException("User with given name already exists");

        var user = new ShreksIdentityUser
        {
            UserName = request.Username,
            SecurityStamp = Guid.NewGuid().ToString(),
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            throw new RegistrationFailedException(string.Join(' ', result.Errors.Select(r => r.Description)));
        
        return Unit.Value;
    }
}