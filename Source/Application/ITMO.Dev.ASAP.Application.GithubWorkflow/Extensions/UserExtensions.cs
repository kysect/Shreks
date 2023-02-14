using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Core.UserAssociations;
using ITMO.Dev.ASAP.Core.Users;
using Microsoft.EntityFrameworkCore;

namespace ITMO.Dev.ASAP.Application.GithubWorkflow.Extensions;

public static class UserExtensions
{
    public static async Task<User?> FindUserByGithubUsername(this DbSet<UserAssociation> users, string githubUsername)
    {
        return await users
            .OfType<GithubUserAssociation>()
            .Where(u => u.GithubUsername.ToLower() == githubUsername.ToLower())
            .Select(u => u.User)
            .SingleOrDefaultAsync();
    }

    public static async Task<User> GetUserByGithubUsername(this DbSet<UserAssociation> users, string githubUsername)
    {
        return await FindUserByGithubUsername(users, githubUsername) ??
               throw DomainInvalidOperationException.UserNotFoundByGithubUsername(githubUsername);
    }
}