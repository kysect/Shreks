namespace Kysect.Shreks.DataAccess.Configuration;

public class PostgresConfiguration
{
    public string Host { get; init; } = string.Empty;

    public int Port { get; init; }

    public string Username { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;

    public string SslMode { get; init; } = string.Empty;

    public string ToConnectionString(string dbname)
    {
        return $"Host={Host};Port={Port};Database={dbname};Username={Username};Password={Password};Ssl Mode={SslMode};";
    }
}