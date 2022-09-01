using AutoMapper;
using AutoMapper.QueryableExtensions;
using Kysect.Shreks.Application.Dto.SubjectCourseAssociations;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Abstractions.Github.Queries.GetSubjectCourseGithubAssociations;

namespace Kysect.Shreks.Application.Handlers.Github;

public class GetSubjectCourseGithubAssociationsHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;

    public GetSubjectCourseGithubAssociationsHandler(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        List<GithubSubjectCourseAssociationDto> githubSubjectCourseAssociationDtos = await _context
            .SubjectCourseAssociations
            .OfType<GithubSubjectCourseAssociation>()
            .ProjectTo<GithubSubjectCourseAssociationDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return new Response(githubSubjectCourseAssociationDtos);
    }
}