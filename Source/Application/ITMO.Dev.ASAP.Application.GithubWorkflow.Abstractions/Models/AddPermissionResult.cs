namespace ITMO.Dev.ASAP.Application.GithubWorkflow.Abstractions.Models;

public enum AddPermissionResult
{
    Invited,
    ReInvited,
    Pending,
    AlreadyCollaborator,
}