using NJsonSchema.CodeGeneration;
using NSwag;
using NSwag.CodeGeneration.CSharp;
using NSwag.CodeGeneration.OperationNameGenerators;

const string filePath = "ShreksApiClient.cs";

using var httpClient = new HttpClient();

string spec = await httpClient.GetStringAsync("https://localhost:7188/swagger/v1/swagger.json");

OpenApiDocument document = await OpenApiDocument.FromJsonAsync(spec);

var settings = new CSharpClientGeneratorSettings
{
    ClassName = "{controller}Client",
    CSharpGeneratorSettings =
    {
        Namespace = "Kysect.Shreks.WebApi.Sdk",
        TimeSpanType = "System.TimeSpan",
        DateType = "System.DateOnly",
        DateTimeType = "System.DateTime",
        EnumNameGenerator = new DefaultEnumNameGenerator(),
    },
    GenerateClientInterfaces = true,
    ClientClassAccessModifier = "public",
    OperationNameGenerator = new MultipleClientsFromFirstTagAndPathSegmentsOperationNameGenerator(),
};


var generator = new CSharpClientGenerator(document, settings);
string code = generator.GenerateFile();

File.WriteAllText(filePath, code);