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
        
        string role = "User";
        if (username.ToLower() == "admin" || username.ToLower() == "owner")
        {
            role = "Owner";
        }
        
        await using var cmd = new NpgsqlCommand("INSERT INTO users (username, password_hash, role) VALUES (@username, @password_hash, @role)", conn);
        cmd.Parameters.AddWithValue("username", username);
        cmd.Parameters.AddWithValue("password_hash", passwordHash);
        cmd.Parameters.AddWithValue("role", role);
        
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<bool> UpdateUserRole(string username, string role)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        
        await using var cmd = new NpgsqlCommand("UPDATE users SET role = @role WHERE username = @username", conn);
        cmd.Parameters.AddWithValue("username", username);
        cmd.Parameters.AddWithValue("role", role);
        
        var rowsAffected = await cmd.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }
}
