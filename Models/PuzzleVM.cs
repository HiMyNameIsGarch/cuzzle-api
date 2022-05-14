public class PuzzleVM
{
    public Guid id { get; set; }
    public Guid solution_id { get; set; }
    public Guid account_id { get; set; }
    public string name { get; set; }
    public string question { get; set; }
    public string flag { get; set; }
    public bool allow_anonymous { get; set; }
    public bool is_published { get; set; }

    public PuzzleVM(string flag, string name, string question)
    {
        this.flag = flag;
        this.name = name;
        this.question = question;
    }
}
