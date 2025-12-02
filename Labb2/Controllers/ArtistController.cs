using Labb2.DTOS;
using Labb2.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Labb2.Controllers;

[ApiController]
[Route("Artist")]
public class ArtistController : ControllerBase {
    private readonly RepoContext _repoContext;
    private readonly ILogger<ArtistController> _logger;
    public ArtistController (ILogger<ArtistController> logger, RepoContext repoContext) {
        _logger = logger;
        _repoContext = repoContext;
    }
    [HttpGet("all")]
    public IActionResult GetAllArtists () {
        var artists = _repoContext.Artists
            .Include(a => a.Albums)
            .Include(a => a.Songs)
            .ToList();

        var result = new GetAllDto();
        foreach (var item in artists) {
            result.GetArtistDtos.Add(new GetArtistDto() {
                Artist = new ArtistDTO(item.Id, item.Name),
                AlbumIds = item.Albums.Select(x => x.Id).ToList(),
                SongIds = item.Songs.Select(x => x.Id).ToList()
            });
        }
        return Ok(result);
    }

    public class GetAllDto {
        public List<GetArtistDto> GetArtistDtos { get; set; } = new List<GetArtistDto>();
    }

    public class GetArtistDto {
        public ArtistDTO Artist { get; set; }
        public List<int> AlbumIds { get; set; } = new();
        public List<int> SongIds { get; set; } = new();
    }


    [HttpGet("get")]
    public IActionResult GetArtistByName (string name) {
        var artist = _repoContext.Artists
            .Where(a => a.Name == name)
            .Include(a => a.Albums)
            .Include(a => a.Songs)
            .FirstOrDefault();
        if (artist == null) return NotFound($"Artist with name '{name}' not found.");

        var result = new GetArtistDto() {
            Artist = new ArtistDTO(artist.Id, artist.Name),
            AlbumIds = artist.Albums.Select(x => x.Id).ToList(),
            SongIds = artist.Songs.Select(x => x.Id).ToList()
        };

        return Ok(result);
    }

    [HttpPost("add")]
    public IActionResult AddArtist (string name) {
        var existingArtist = _repoContext.Artists
            .Where(a => a.Name == name)
            .FirstOrDefault();

        if (existingArtist != null) return Conflict($"Artist with name '{name}' already exists.");

        var newArtist = new Artist {
            Name = name
        };
        _repoContext.Artists.Add(newArtist);
        _repoContext.SaveChanges();

        var result = new AddDto();
        result.IsAdded = true;
        return Ok(result);
    }

    public class AddDto {
        public bool IsAdded { get; set; }
        public string Message { get; set; } = string.Empty;

    }

    [HttpDelete("delete")]
    public IActionResult DeleteArtist (string name) {
        var artist = _repoContext.Artists
            .Where(a => a.Name == name)
            .FirstOrDefault();

        var result = new DeleteDto();
        if (artist == null) {
            result.IsDeleted = true;
            result.Message = $"Artist with name '{name}' not found.";
            return Ok(result);
        }

        _repoContext.Artists.Remove(artist);
        _repoContext.SaveChanges();
        result.IsDeleted = true;
        return Ok(result);
    }

    public class DeleteDto {
        public bool IsDeleted { get; set; }
        public string Message { get; set; } = string.Empty;

    }
}
