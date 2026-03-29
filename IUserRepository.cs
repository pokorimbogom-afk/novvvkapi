using System.Threading.Tasks;

public interface IUserRepository
{
    Task<bool> UserExists(string username);
    Task CreateUser(string username, string passwordHash);
    Task<UserData> GetUserByUsername(string username);
}

public class UserData
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string Role { get; set; }
    public string Hwid { get; set; }
    public string Subscription { get; set; }
}
