using Microsoft.AspNetCore.Mvc;
using cuzzle_api.Models;

namespace cuzzle_api.Controllers;

[ApiController]
[Route("[controller]")]
public class PuzzleController : ControllerBase
{
    private readonly ILogger<PuzzleController> _logger;

    private readonly CuzzleEntity db = new CuzzleEntity();

    public PuzzleController(ILogger<PuzzleController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetPuzzle")]
    public Puzzle Get()
    {
        _logger.Log(LogLevel.Information, "trying to get data from db");
        var data = db.GetData("select * from puzzle limit 1;");
        _logger.Log(LogLevel.Information, "You data is: {0}", data);
        return data;
    }
}
