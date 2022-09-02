namespace Kysect.Shreks.DataAccess.Configuration;

public class PostgresConfiguration
{
    public string Host { get; init; }
    
    public int Port { get; init; }
    
    public string Database { get; init; }
    
    public string Username { get; init; }
    
    public string Password { get; init; }
    
    public string SslMode { get; init; }

    public string ToConnectionString()
    {
        return $"Host={Host};Port={Port};Database={Database};Username={Username};Password={Password};Ssl Mode={SslMode};";
    }
}