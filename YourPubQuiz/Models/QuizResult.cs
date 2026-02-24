namespace YourPubQuiz.Models;

public class QuizResult
{
    public int TotalQuestions { get; set; }
    public int CorrectAnswers { get; set; }
    public List<QuestionResultDetails> QuestionResults { get; set; } = [];
}

public class QuestionResultDetails
{
    public string QuestionId { get; set; }
    public string QuestionText { get; set; }
    public bool IsCorrect { get; set; }
    public string UserAnswer { get; set; }
    public string CorrectAnswer { get; set; }
}