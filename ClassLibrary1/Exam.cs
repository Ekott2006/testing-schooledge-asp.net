namespace ClassLibrary1;

public class Exam
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime ReleaseDate { get; set; }
    public TimeSpan Duration { get; set; } // Example: 1 hour

    // Navigation Property
    public List<Question> Questions { get; set; } = [];
}