using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cuzzle_api.Controllers;

[ApiController]
[Route("api/puzzle")]
public class PuzzleController : ControllerBase
{
    private readonly ILogger<PuzzleController> _logger;
    private readonly IUserService _userService;
    private readonly IPuzzleService _ps;

    public PuzzleController(ILogger<PuzzleController> logger, IUserService user, IPuzzleService ps)
    {
        _ps = ps;
        _logger = logger;
        _userService = user;
    }

    [Authorize] // test purpose
    [HttpGet]
    public IActionResult GetAll()
    {
        var puzzles = _ps.GetPuzzleList();

        return Ok(puzzles);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(Guid id)
    {
        var puzzle = _ps.GetPuzzle(id);
        if(puzzle.Id == Guid.Empty) return BadRequest("We could not get puzzle");

        return Ok(puzzle);
    }

    [HttpPost]
    public IActionResult Add([FromBody] PuzzleVM puzzleToAdd)
    {
        puzzleToAdd.Id = Guid.Empty;
        puzzleToAdd.SolutionId = Guid.Empty;

        var puzzle = _ps.AddPuzzle(puzzleToAdd);
        if(puzzle.Id == Guid.Empty) return BadRequest("We could not create puzzle");

        return Ok(puzzle);
    }

    [HttpPut]
    public IActionResult Update([FromBody] PuzzleVM puzzleToUpdate)
    {
        var updated = _ps.UpdatePuzzle(puzzleToUpdate);
        if(!updated) return BadRequest("We could not update puzzle");

        return Ok();
    }

    [HttpDelete]
    public IActionResult Delete(Guid id)
    {
        var deleted = _ps.DeletePuzzle(id);
        if(!deleted) return BadRequest("We could not delete puzzle!");

        return Ok();
    }
}
