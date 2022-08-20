using AutoMapper;
using Kysect.Shreks.Application.Abstractions.DataAccess;
using Kysect.Shreks.Application.Abstractions.DataAccess.Extensions;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Core.Exceptions;
using Kysect.Shreks.Core.ValueObject;
using MediatR;
using static Kysect.Shreks.Application.Abstractions.Submissions.Commands.UpdateSubmissionPoints;

namespace Kysect.Shreks.Application.Handlers.Submissions;

public class UpdateSubmissionPointsHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;

    public UpdateSubmissionPointsHandler(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        if (request.NewRating is null && request.ExtraPoints is null)
            throw new DomainInvalidOperationException($"Cannot update submission points, both {nameof(request.NewRating)} and {nameof(request.ExtraPoints)} are null.");

        var submission = await _context.Submissions.GetByIdAsync(request.SubmissionId, cancellationToken);

        if (request.NewRating is not null)
        {
            var fraction = new Fraction(request.NewRating.Value / 100);
            submission.Rating = fraction;
        } 

        if (request.ExtraPoints is not null)
            submission.ExtraPoints = new Points(request.ExtraPoints.Value);

        _context.Submissions.Update(submission);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = _mapper.Map<SubmissionDto>(submission);

        return new Response(dto);
    }
}