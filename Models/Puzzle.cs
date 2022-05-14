public class Puzzle 
{
    public Guid id { get; set; }
    public Guid solution_id { get; set; }
    public Guid account_id { get; set; }
    public string? name { get; set; }
    public string? question { get; set; }
    public string? flag { get; set; }
    public bool? allow_anonymous { get; set; }
    public bool? is_published { get; set; }
    public DateTime? created_at { get; set; }
    public DateTime? last_modified { get; set; }

    public Puzzle()
    {
    }
}
