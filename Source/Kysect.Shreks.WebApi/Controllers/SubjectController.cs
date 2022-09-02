﻿using Kysect.Shreks.Application.Abstractions.Study.Commands;
using Kysect.Shreks.Application.Abstractions.Study.Queries;
using Kysect.Shreks.Application.Dto.Study;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Kysect.Shreks.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SubjectController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<SubjectDto>> Create(string name)
        {
            CreateSubject.Response response = await _mediator.Send(new CreateSubject.Command(name));
            return Ok(response.Subject);
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyCollection<SubjectDto>>> Get()
        {
            GetSubjects.Response response = await _mediator.Send(new GetSubjects.Query());
            return Ok(response.Subjects);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SubjectDto>> GetById(Guid id)
        {
            GetSubjectById.Response response = await _mediator.Send(new GetSubjectById.Query(id));
            return Ok(response.Subject);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<SubjectDto>> Update(Guid id, string name)
        {
            UpdateSubject.Response response = await _mediator.Send(new UpdateSubject.Command(id, name));
            return Ok(response.Subject);
        }
    }
}
