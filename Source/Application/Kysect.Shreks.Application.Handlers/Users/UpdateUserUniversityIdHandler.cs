using Kysect.Shreks.Application.Abstractions.Identity;
using Kysect.Shreks.Core.UserAssociations;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using MediatR;
using static Kysect.Shreks.Application.Contracts.Users.Commands.UpdateUserUniversityId;

namespace Kysect.Shreks.Application.Handlers.Users;

internal class UpdateUserUniversityIdHandler : IRequestHandler<Command>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IShreksDatabaseContext _context;

    public UpdateUserUniversityIdHandler(IShreksDatabaseContext context, IAuthorizationService authorizationService)
    {
        _context = context;
        _authorizationService = authorizationService;
    }

    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
    {
        await _authorizationService.AuthorizeAdminAsync(request.CallerUsername, cancellationToken);

        User user = await _context.Users.GetByIdAsync(request.UserId, cancellationToken);
        IsuUserAssociation? association = user.FindAssociation<IsuUserAssociation>();

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

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return Unit.Value;
    }
}