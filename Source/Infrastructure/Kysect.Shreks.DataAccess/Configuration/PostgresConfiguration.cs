namespace Kysect.Shreks.DataAccess.Configuration;

public class PostgresConfiguration
{
    public string Host { get; init; }

    public int Port { get; init; }

    public string Username { get; init; }

    public string Password { get; init; }

    public string SslMode { get; init; }

    public string ToConnectionString(string dbname)
    {
        return $"Host={Host};Port={Port};Database={dbname};Username={Username};Password={Password};Ssl Mode={SslMode};";
    }
}