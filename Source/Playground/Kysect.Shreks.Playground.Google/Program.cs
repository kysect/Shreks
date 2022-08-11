using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Core.Formatters;
using Kysect.Shreks.Integration.Google;
using Kysect.Shreks.Integration.Google.Extensions;
using Kysect.Shreks.Integration.Google.Providers;
using Microsoft.Extensions.DependencyInjection;

var credential = await GoogleCredential.FromFileAsync("client_secrets.json", default);

var initializer = new BaseClientService.Initializer
{
    HttpClientInitializer = credential
};

var sheetsService = new SheetsService(initializer);

const string spreadSheetId = "";

var spreadsheetIdProvider = new ConstSpreadsheetIdProvider(spreadSheetId);

IServiceProvider services = new ServiceCollection()
    .AddSheetServices()
    .AddSingleton(sheetsService)
    .AddSingleton<ISpreadsheetIdProvider>(spreadsheetIdProvider)
    .AddSingleton<IGoogleTableAccessor, GoogleTableAccessor>()
    .AddSingleton<IUserFullNameFormatter, UserFullNameFormatter>()
    .AddSingleton<ICultureInfoProvider, RuCultureInfoProvider>()
    .BuildServiceProvider();

var googleTableAccessor = services.GetRequiredService<IGoogleTableAccessor>();