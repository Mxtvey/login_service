using Microsoft.AspNetCore.Mvc;
using LoginService.Models;

namespace LoginService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private static readonly List<User> Users = new();
    private static int _nextId = 1;

    [HttpPost("login")]
    public ActionResult<UserResponse> Login([FromBody] LoginRequest request)
    {
        var user = Users.FirstOrDefault(u => 
            u.Username == request.Username && u.Password == request.Password);

        if (user == null)
        {
            return Unauthorized("Неверный логин или пароль");
        }

        var response = new UserResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email
        };

        return Ok(response);
    }

    [HttpPost]
    public ActionResult<UserResponse> CreateUser([FromBody] CreateUserRequest request)
    {
        var existingUser = Users.FirstOrDefault(u => u.Username == request.Username);
        if (existingUser != null)
        {
            return BadRequest("Пользователь с таким логином уже существует");
        }

        var newUser = new User
        {
            Id = _nextId++,
            Username = request.Username,
            Email = request.Email,
            Password = request.Password
        };

        Users.Add(newUser);

        var response = new UserResponse
        {
            Id = newUser.Id,
            Username = newUser.Username,
            Email = newUser.Email
        };

        return CreatedAtAction(nameof(GetUserById), new { id = newUser.Id }, response);
    }

    [HttpGet]
    public ActionResult<List<UserResponse>> GetAllUsers()
    {
        var response = Users.Select(user => new UserResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email
        }).ToList();

        return Ok(response);
    }

    [HttpGet("{id}")]
    public ActionResult<UserResponse> GetUserById(int id)
    {
        var user = Users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            return NotFound("Пользователь не найден");
        }

        var response = new UserResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email
        };

        return Ok(response);
    }

    [HttpPut("{id}")]
    public ActionResult<UserResponse> UpdateUser(int id, [FromBody] UpdateUserRequest request)
    {
        var user = Users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            return NotFound("Пользователь не найден");
        }

        user.Username = request.Username;
        user.Email = request.Email;
        user.Password = request.Password;

        var response = new UserResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email
        };

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteUser(int id)
    {
        var user = Users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            return NotFound("Пользователь не найден");
        }

        Users.Remove(user);
        return NoContent();
    }
}
