﻿using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.Study.Queries;

internal static class GetSubjects
{
    public record Query : IRequest<Response>;

    public record Response(IReadOnlyCollection<SubjectDto> Subjects);
}