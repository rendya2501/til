using Microsoft.EntityFrameworkCore;
using TekitouCRUD.Server.Interfaces;
using TekitouCRUD.Server.Models;
using TekitouCRUD.Server.Services;
using TekitouCRUD.Shared.Entities;

namespace TekitouCRUD.Server.Services;

public class UserManager : IUser
{
    private readonly DataContext _dbContext;
    public UserManager(DataContext dbContext)
    {
        _dbContext = dbContext;
    }

    public User GetUserData(int id)
    {
        return _dbContext.Users.Find(id) ?? throw new Exception("NUll");
    }


    public List<User> GetUserDetails()
    {
        return _dbContext.Users.ToList();
    }


    public void UpdateUserDetails(User user)
    {
        _dbContext.Entry(user).State = EntityState.Modified;
        _dbContext.SaveChanges();
    }


    public void AddUser(User user)
    {
        _dbContext.Users.Add(user);
        _dbContext.SaveChanges();
    }


    public void DeleteUser(int id)
    {
        User? user = _dbContext.Users.Find(id);
        if (user != null)
        {
            _dbContext.Users.Remove(user);
            _dbContext.SaveChanges();
        }
    }
}
