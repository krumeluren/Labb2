using Labb2.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Labb2.Controllers;

[ApiController]
[Route("Playlist")]
public class PlaylistController : ControllerBase {
    private readonly RepoContext _repoContext;
    private readonly ILogger<UserController> _logger;

    public PlaylistController (ILogger<UserController> logger, RepoContext repoContext) {
        _logger = logger;
        _repoContext = repoContext;
    }

    [HttpPost("create")]
    public IActionResult CreatePlaylist (string username, string playlistName) {
        var user = _repoContext.Users
            .Where(u => u.Username == username)
            .FirstOrDefault();
        if (user == null) return NotFound($"User with name '{username}' not found.");

        var newPlaylist = new Playlist {
            Name = playlistName,
            UserId = user.Id
        };
        _repoContext.Playlists.Add(newPlaylist);
        _repoContext.SaveChanges();
        return Ok(newPlaylist);
    }

    [HttpPost("delete")]
    public IActionResult DeletePlaylist (string username, string playlistName) {
        var user = _repoContext.Users
            .Where(u => u.Username == username)
            .FirstOrDefault();

        if (user == null) return NotFound($"User with name '{username}' not found.");

        var playlist = _repoContext.Playlists
            .Where(p => p.Name == playlistName && p.UserId == user.Id)
            .FirstOrDefault();
        if (playlist == null) return NotFound($"Playlist with name '{playlistName}' not found for user '{username}'.");

        _repoContext.Playlists.Remove(playlist);
        _repoContext.SaveChanges();
        return Ok($"Playlist '{playlistName}' deleted successfully.");
    }

    [HttpPost("add")]
    public IActionResult AddSongToPlaylist (string username, string playlistName, int songId) {
        var user = _repoContext.Users
            .Where(u => u.Username == username)
            .FirstOrDefault();
        if (user == null) return NotFound($"User with name '{username}' not found.");


        var playlist = _repoContext.Playlists
            .Where(p => p.Name == playlistName && p.UserId == user.Id)
            .Include(p => p.Songs)
            .FirstOrDefault();
        if (playlist == null) return NotFound($"Playlist with name '{playlistName}' not found for user '{username}'.");

        var song = _repoContext.Songs
            .Where(s => s.Id == songId)
            .FirstOrDefault();
        if (song == null) return NotFound($"Song with ID '{songId}' not found.");

        playlist.Songs.Add(song);
        _repoContext.SaveChanges();

        return Ok($"Song with ID '{songId}' added to playlist '{playlistName}'.");
    }
    [HttpPost("remove")]
    public IActionResult RemoveSongFromPlaylist (string username, string playlistName, int songId) {
        var user = _repoContext.Users
            .Where(u => u.Username == username)
            .FirstOrDefault();
        if (user == null) return NotFound($"User with name '{username}' not found.");
        var playlist = _repoContext.Playlists
            .Where(p => p.Name == playlistName && p.UserId == user.Id)
            .Include(p => p.Songs)
            .FirstOrDefault();
        if (playlist == null) return NotFound($"Playlist with name '{playlistName}' not found for user '{username}'.");
        var song = playlist.Songs
            .Where(s => s.Id == songId)
            .FirstOrDefault();
        if (song == null) return NotFound($"Song with ID '{songId}' not found in playlist '{playlistName}'.");
        playlist.Songs.Remove(song);
        _repoContext.SaveChanges();
        return Ok($"Song with ID '{songId}' removed from playlist '{playlistName}'.");
    }
}
