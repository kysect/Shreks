﻿using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Students;

public static class UpdateUserName
{
    public record Command(Guid UserId, string FirstName, string MiddleName, string LastName) : IRequest<Response>;

    public record Response;
}