﻿using Kysect.Shreks.Application.Dto.Users;
using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Github.Queries;

public class GerUserCourseRole
{
    public record Query(Guid SubjectCourseId, Guid UserId) : IRequest<Response>;
    public record Response(UserCourseRole Role);    
}