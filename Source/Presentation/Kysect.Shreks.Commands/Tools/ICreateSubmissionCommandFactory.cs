using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Commands.Tools;

public interface ICreateSubmissionCommandFactory
{
    Task<IRequest<SubmissionDto>> CreateCommandAsync();
}