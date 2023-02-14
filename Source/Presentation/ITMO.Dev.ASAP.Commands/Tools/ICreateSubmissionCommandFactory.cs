using ITMO.Dev.ASAP.Application.Dto.Study;
using MediatR;

namespace ITMO.Dev.ASAP.Commands.Tools;

public interface ICreateSubmissionCommandFactory
{
    Task<IRequest<SubmissionDto>> CreateCommandAsync();
}