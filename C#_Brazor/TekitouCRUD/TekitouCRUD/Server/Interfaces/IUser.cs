using TekitouCRUD.Shared.Entities;

namespace TekitouCRUD.Server.Interfaces;

public interface IUser
{
    public List<User> GetUserDetails();
    public void AddUser(User user);
    public void UpdateUserDetails(User user);
    public User GetUserData(int id);
    public void DeleteUser(int id);
}
