﻿using AutoMapper;
using Kysect.Shreks.Application.Dto.Users;
using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.Students.Queries.GetStudentById;

namespace Kysect.Shreks.Application.Handlers.Students;

internal class GetStudentByIdHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;

    public GetStudentByIdHandler(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        Student? student = await _context.Students
            .Where(s => s.User.Id == request.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (student == default)
            throw new EntityNotFoundException($"Student not found for user {request.UserId}");

        StudentDto? dto = _mapper.Map<StudentDto>(student);

        return new Response(dto);
    }
}