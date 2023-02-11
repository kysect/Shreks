using ITMO.Dev.ASAP.Core.Users;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.DataAccess.Abstractions.Extensions;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Students.Commands.UpdateUserName;

namespace ITMO.Dev.ASAP.Application.Handlers.Users;

internal class UpdateUserNameHandler : IRequestHandler<Command, Response>
{
    private readonly IDatabaseContext _context;

    public UpdateUserNameHandler(IDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        User user = await _context.Users.GetByIdAsync(request.UserId, cancellationToken);

        user.FirstName = request.FirstName;
        user.MiddleName = request.MiddleName;
        user.LastName = request.LastName;

        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);

        return new Response();
    }
}