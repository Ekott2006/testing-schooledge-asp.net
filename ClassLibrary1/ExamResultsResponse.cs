namespace ClassLibrary1;

public class ExamResultsResponse : StudentExam
{
    public int StudentExamId { get; set; }
    public string ExamTitle { get; set; }
    public int Percentage { get; set; }
}