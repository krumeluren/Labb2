using Labb2.DTOS;
using Labb2.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Labb2.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase {
    private readonly RepoContext _repoContext;
    private readonly ILogger<UserController> _logger;

    public UserController (ILogger<UserController> logger, RepoContext repoContext) {
        _logger = logger;
        _repoContext = repoContext;
    }


    [HttpPost("register")]
    public IActionResult Register (string username) {
        var existingUser = _repoContext.Users
            .Where(u => u.Username == username)
            .FirstOrDefault();
        if (existingUser != null) {
            return Conflict($"User with name '{username}' already exists.");
        }
        var newUser = new User {
            Username = username
        };

        _repoContext.Users.Add(newUser);
        _repoContext.SaveChanges();

        var response = new UserDTO(newUser.Username, newUser.Id);
        return Ok(response);
    }

    [HttpGet("get/playlists")]
    public IActionResult GetPlaylists (string username) {
        var user = _repoContext.Users
            .Where(u => u.Username == username)
            .Include(u => u.Playlists)
            .ThenInclude(p => p.Songs)
            .FirstOrDefault();

        if (user == null) {
            return NotFound($"User with name '{username}' not found.");
        }
        var response = new PlaylistsDTO();
        var playlists = new List<PlaylistDTO>();
        foreach (var item in user.Playlists) {
            playlists.Add(new PlaylistDTO(item.Id, item.Name) {
                Songs = item.Songs.Select(s => new SongDTO(s.Id, s.Title, s.Duration)).ToList()
            });
        }
        response.Playlists = playlists;
        return Ok(response);
    }

    public class PlaylistsDTO {
        public List<PlaylistDTO> Playlists { get; set; }

    }
}