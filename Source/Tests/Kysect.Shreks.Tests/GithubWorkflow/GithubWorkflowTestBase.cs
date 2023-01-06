// using Kysect.Shreks.Core.Submissions;
// using Kysect.Shreks.DataAccess.Context;
// using Kysect.Shreks.DataAccess.Extensions;
// using Kysect.Shreks.Mapping.Extensions;
// using Kysect.Shreks.Seeding.Extensions;
// using Kysect.Shreks.Tests.GithubWorkflow.Tools;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.DependencyInjection;
//
// namespace Kysect.Shreks.Tests.GithubWorkflow;
//
// public class GithubWorkflowTestBase : TestBase, IDisposable
// {
//     protected readonly ShreksDatabaseContext Context;
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
//         Context = Provider.GetRequiredService<ShreksDatabaseContext>();
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

