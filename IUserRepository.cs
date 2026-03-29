using System.Threading.Tasks;

public interface IUserRepository
{
    Task<bool> UserExists(string username);
    Task CreateUser(string username, string passwordHash);
    Task<bool> UpdateUserRole(string username, string role);
}
