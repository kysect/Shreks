using NSwag;
using NSwag.CodeGeneration.CSharp;

string filePath = "ShreksApiClient.cs";

var httpClient = new HttpClient();

var spec = await httpClient.GetStringAsync("https://localhost:7188/swagger/v1/swagger.json");

var document = await OpenApiDocument.FromJsonAsync(spec);

var settings = new CSharpClientGeneratorSettings
{
    ClassName = "ShreksApiClient",
    CSharpGeneratorSettings =
    {
        Namespace = "Shreks.ApiClient"
    }
};


var generator = new CSharpClientGenerator(document, settings);
var code = generator.GenerateFile();

File.WriteAllText(filePath, code);