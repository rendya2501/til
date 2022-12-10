using Microsoft.AspNetCore.Mvc;
using TekitouCRUD.Server.Interfaces;
using TekitouCRUD.Shared.Entities;

namespace TekitouCRUD.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUser _IUser;
    public UserController(IUser iUser)
    {
        _IUser = iUser;
    }

    [HttpGet]
    public async Task<List<User>> GetAll()
    {
        return await Task.FromResult(_IUser.GetUserDetails());
    }

    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        User user = _IUser.GetUserData(id);
        if (user != null)
        {
            return Ok(user);
        }
        return NotFound();
    }

    [HttpPost]
    public void Post(User user)
    {
        _IUser.AddUser(user);
    }

    [HttpPut]
    public void Put(User user)
    {
        _IUser.UpdateUserDetails(user);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _IUser.DeleteUser(id);
        return Ok();
    }
}
