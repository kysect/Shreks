using Kysect.Shreks.Application.Abstractions.Identity;
using Kysect.Shreks.Core.UserAssociations;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using MediatR;
using static Kysect.Shreks.Application.Abstractions.Users.Commands.UpdateUserUniversityId;

namespace Kysect.Shreks.Application.Handlers.Users;

public class UpdateUserUniversityIdHandler : IRequestHandler<Command>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IAuthorizationService _authorizationService;

    public UpdateUserUniversityIdHandler(IShreksDatabaseContext context, IAuthorizationService authorizationService)
    {
        _context = context;
        _authorizationService = authorizationService;
    }

    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
    {
        await _authorizationService.AuthorizeAdminAsync(request.CallerUsername, cancellationToken);

        var user = await _context.Users.GetByIdAsync(request.UserId, cancellationToken);
        var association = user.FindAssociation<IsuUserAssociation>();

        if (association is null)
        {
            association = new IsuUserAssociation(user, request.UniversityId);
            _context.UserAssociations.Add(association);
        }
        else
        {
            association.UniversityId = request.UniversityId;
            _context.UserAssociations.Update(association);
        }

        await _context.SaveChangesAsync(cancellationToken);
        
        return Unit.Value;
    }
}