public class Puzzle 
{
    public Guid Id { get; set; }
    public Guid SolutionId { get; set; }
    public Guid AccountId { get; set; }
    public string? Name { get; set; }
    public string? Question { get; set; }
    public string? Flag { get; set; }
    public bool AllowAnonymous { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastModified { get; set; }

    public Puzzle()
    {
    }
}
