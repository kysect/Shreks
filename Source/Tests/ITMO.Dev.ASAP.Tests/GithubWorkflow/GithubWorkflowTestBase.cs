// using ITMO.Dev.ASAP.Core.Submissions;
// using ITMO.Dev.ASAP.DataAccess.Context;
// using ITMO.Dev.ASAP.DataAccess.Extensions;
// using ITMO.Dev.ASAP.Mapping.Extensions;
// using ITMO.Dev.ASAP.Seeding.Extensions;
// using ITMO.Dev.ASAP.Tests.GithubWorkflow.Tools;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.DependencyInjection;
//
// namespace ITMO.Dev.ASAP.Tests.GithubWorkflow;
//
// public class GithubWorkflowTestBase : TestBase, IDisposable
// {
//     protected readonly DatabaseContext Context;
//     protected readonly IServiceProvider Provider;
//     protected readonly GithubApplicationTestContextGenerator TestContextGenerator;
//
//     protected GithubWorkflowTestBase()
//     {
//         var collection = new ServiceCollection();
//         var id = Guid.NewGuid();
//
//         collection.AddDatabaseContext(x =>
//         {
//             x.UseLazyLoadingProxies().UseSqlite($"Data Source={id}.db");
//         });
//
//         collection.AddEntityGenerators(x =>
//         {
//             x.ConfigureEntityGenerator<GithubSubmission>(xx => xx.Count = 500);
//         });
//         collection.AddDatabaseSeeders();
//         collection.AddMappingConfiguration();
//         collection.AddScoped<GithubApplicationTestContextGenerator>();
//
//         Provider = collection.BuildServiceProvider();
//
//         Context = Provider.GetRequiredService<DatabaseContext>();
//         Context.Database.EnsureCreated();
//
//         TestContextGenerator = Provider.GetRequiredService<GithubApplicationTestContextGenerator>();
//
//         Provider.UseDatabaseSeeders().GetAwaiter().GetResult();
//     }
//
//     public void Dispose()
//     {
//         Context.Database.EnsureDeleted();
//         Context.Dispose();
//     }
// }

