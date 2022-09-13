using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.UserAssociations;
using Kysect.Shreks.Core.Users;
using Microsoft.EntityFrameworkCore;

namespace Kysect.Shreks.Application.GithubWorkflow.Extensions;

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
        return await FindUserByGithubUsername(users, githubUsername)
               ?? throw new UserNotFoundByGithubUsernameException(githubUsername);
    }
}