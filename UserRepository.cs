using System;
using System.Threading.Tasks;
using Npgsql;

public class UserRepository : IUserRepository
{
    private readonly string _connectionString;

    public UserRepository()
    {
        var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
        
        if (!string.IsNullOrEmpty(databaseUrl) && databaseUrl.StartsWith("postgresql://"))
        {
            // Parse Render.com PostgreSQL URL format
            var uri = new Uri(databaseUrl);
            var port = uri.Port > 0 ? uri.Port : 5432;
            _connectionString = $"Host={uri.Host};Port={port};Database={uri.AbsolutePath.TrimStart('/')};Username={uri.UserInfo.Split(':')[0]};Password={uri.UserInfo.Split(':')[1]};SSL Mode=Require;Trust Server Certificate=true";
        }
        else
        {
            _connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                ?? databaseUrl
                ?? "Host=localhost;Database=pastadb;Username=postgres;Password=yourpassword";
        }
    }

    public async Task<bool> UserExists(string username)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        
        await using var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM users WHERE username = @username", conn);
        cmd.Parameters.AddWithValue("username", username);
        
        var count = (long)await cmd.ExecuteScalarAsync();
        return count > 0;
    }

    public async Task CreateUser(string username, string passwordHash)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        
        await using var cmd = new NpgsqlCommand("INSERT INTO users (username, password_hash, role) VALUES (@username, @password_hash, 'User')", conn);
        cmd.Parameters.AddWithValue("username", username);
        cmd.Parameters.AddWithValue("password_hash", passwordHash);
        
        await cmd.ExecuteNonQueryAsync();
    }
}
