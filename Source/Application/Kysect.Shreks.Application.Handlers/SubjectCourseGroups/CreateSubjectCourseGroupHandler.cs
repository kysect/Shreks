using AutoMapper;
using Kysect.Shreks.Application.Dto.SubjectCourses;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.SubjectCourseGroups.Commands.CreateSubjectCourseGroup;

namespace Kysect.Shreks.Application.Handlers.SubjectCourseGroups;

internal class CreateSubjectCourseGroupHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;

    public CreateSubjectCourseGroupHandler(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        SubjectCourse subjectCourse = await _context.SubjectCourses
            .Include(x => x.Assignments)
            .ThenInclude(x => x.GroupAssignments)
            .GetByIdAsync(request.SubjectCourseId, cancellationToken);

        StudentGroup studentGroup = await _context.StudentGroups
            .GetByIdAsync(request.StudentGroupId, cancellationToken);

        SubjectCourseGroup subjectCourseGroup = subjectCourse.AddGroup(studentGroup);

        foreach (Assignment assignment in subjectCourse.Assignments)
        {
            assignment.AddGroup(studentGroup, DateOnly.FromDateTime(DateTime.UnixEpoch));
        }

        await _context.SubjectCourseGroups.AddAsync(subjectCourseGroup, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        SubjectCourseGroupDto? dto = _mapper.Map<SubjectCourseGroupDto>(subjectCourseGroup);

        return new Response(dto);
    }
}