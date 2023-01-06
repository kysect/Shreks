using Kysect.Shreks.Application.Contracts.Identity.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Kysect.Shreks.WebApi.Filters;

public class AuthenticationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        IMediator mediator = context.HttpContext.RequestServices.GetRequiredService<IMediator>();
        string? token = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (token is null)
        {
            await next.Invoke();
            return;
        }

        try
        {
            var query = new GetIdentityUserByToken.Query(token);
            GetIdentityUserByToken.Response response = await mediator.Send(query, context.HttpContext.RequestAborted);

            context.HttpContext.Items["user"] = response.User;
        }
        catch
        {
            // ignored
        }

        await next.Invoke();
    }
}