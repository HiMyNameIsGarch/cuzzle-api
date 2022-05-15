using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cuzzle_api.Controllers;

[ApiController]
[Route("api/puzzle")]
public class PuzzleController : ControllerBase
{
    private readonly ILogger<PuzzleController> _logger;

    private readonly PuzzleService ps;

    public PuzzleController(ILogger<PuzzleController> logger)
    {
        ps = new PuzzleService();
        _logger = logger;
    }

    [Authorize] // test purpose
    [HttpGet]
    public IActionResult GetAll()
    {
        var puzzles = ps.GetPuzzleList();

        return Ok(puzzles);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(Guid id)
    {
        var puzzle = ps.GetPuzzle(id);
        if(puzzle.Id == Guid.Empty) return BadRequest("We could not get puzzle");

        return Ok(puzzle);
    }

    [HttpPost]
    public IActionResult Add([FromBody] PuzzleVM puzzleToAdd)
    {
        puzzleToAdd.Id = Guid.Empty;
        puzzleToAdd.SolutionId = Guid.Empty;

        var puzzle = ps.AddPuzzle(puzzleToAdd);
        if(puzzle.Id == Guid.Empty) return BadRequest("We could not create puzzle");

        return Ok(puzzle);
    }

    [HttpPut]
    public IActionResult Update([FromBody] PuzzleVM puzzleToUpdate)
    {
        var updated = ps.UpdatePuzzle(puzzleToUpdate);
        if(!updated) return BadRequest("We could not update puzzle");

        return Ok();
    }

    [HttpDelete]
    public IActionResult Delete(Guid id)
    {
        var deleted = ps.DeletePuzzle(id);
        if(!deleted) return BadRequest("We could not delete puzzle!");

        return Ok();
    }
}
