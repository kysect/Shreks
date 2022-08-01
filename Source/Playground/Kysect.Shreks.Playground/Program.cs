using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .WriteTo.Console()
    .WriteTo.File("ShreksPlayground.log")
    .CreateLogger();

Console.WriteLine("Hello, World!");
