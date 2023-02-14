using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Core.UserAssociations;
using ITMO.Dev.ASAP.Core.Users;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.DataAccess.Abstractions.Extensions;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Users.Commands.UpdateUserUniversityId;

namespace ITMO.Dev.ASAP.Application.Handlers.Users;

internal class UpdateUserUniversityIdHandler : IRequestHandler<Command>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IDatabaseContext _context;

    public UpdateUserUniversityIdHandler(IDatabaseContext context, IAuthorizationService authorizationService)
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
            association = new IsuUserAssociation(Guid.NewGuid(), user, request.UniversityId);
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