using System;
using System.Threading.Tasks;
using Npgsql;

public class UserRepository : IUserRepository
{
    private readonly string _connectionString;

    public UserRepository()
    {
        _connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") 
            ?? "Host=localhost;Database=pastadb;Username=postgres;Password=yourpassword";
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
        
        await using var cmd = new NpgsqlCommand("INSERT INTO users (username, password_hash) VALUES (@username, @password_hash)", conn);
        cmd.Parameters.AddWithValue("username", username);
        cmd.Parameters.AddWithValue("password_hash", passwordHash);
        
        await cmd.ExecuteNonQueryAsync();
    }
}
