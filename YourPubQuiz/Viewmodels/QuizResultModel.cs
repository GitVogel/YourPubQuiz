namespace YourPubQuiz.Viewmodels;

public class QuizResultModel
{
    public int TotalQuestions { get; set; }
    public int CorrectAnswers { get; set; }
    public List<QuestionResult> QuestionResults { get; set; } = [];
}

public class QuestionResult
{
    public string QuestionId { get; set; }
    public string QuestionText { get; set; }
    public bool IsCorrect { get; set; }
    public string UserAnswer { get; set; }
    public string CorrectAnswer { get; set; }
}