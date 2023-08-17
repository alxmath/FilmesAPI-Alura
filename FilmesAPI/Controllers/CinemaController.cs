using AutoMapper;
using FilmesAPI.Data;
using FilmesAPI.Data.Dtos;
using FilmesAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FilmesAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class CinemaController : ControllerBase
{
    private readonly FilmeContext _context;
    private readonly IMapper _mapper;

    public CinemaController(FilmeContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpPost]
    public IActionResult AdicionarCinema([FromBody] CreateCinemaDto cinemaDto)
    {
        Cinema cinema = _mapper.Map<Cinema>(cinemaDto);
        _context.Cinemas.Add(cinema);
        _context.SaveChanges();
        return CreatedAtAction(nameof(RecuperarCinemasPorId), new { cinema.Id }, cinema);
    }

    [HttpGet]
    public IEnumerable<ReadCinemaDto> RecuperarCinemas([FromQuery] int? enderecoId = null)
    {
        if (enderecoId == null)
        {
            return _mapper.Map<IEnumerable<ReadCinemaDto>>(_context.Cinemas.ToList());
        }

        var result = _context.Cinemas.FromSqlRaw($"SELECT Id, Nome, EnderecoId FROM Cinemas WHERE EnderecoId = {enderecoId}");
        return _mapper.Map<IEnumerable<ReadCinemaDto>>(result.ToList());
    }

    [HttpGet("{id}")]
    public IActionResult RecuperarCinemasPorId(int id)
    {
        Cinema cinema = _context.Cinemas.Find(id);

        if (cinema != null)
        {
            ReadCinemaDto cinemaDto = _mapper.Map<ReadCinemaDto>(cinema);
            return Ok(cinemaDto);
        }

        return NotFound();
    }

    [HttpPut("{id}")]
    public IActionResult AtualizarCinema(int id, [FromBody] UpdateCinemaDto cinemaDto)
    {
        Cinema cinema = _context.Cinemas.Find(id);

        if (cinema == null) { return NotFound(); }

        _mapper.Map(cinemaDto, cinema);
        _context.SaveChanges();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeletarCinema(int id)
    {
        var cinema = _context.Cinemas.Find(id);

        if (cinema == null)
        {
            return NotFound();
        }

        _context.Remove(cinema);
        _context.SaveChanges();
        return NoContent();
    }
}
