namespace YourPubQuiz.Models;

public class QuizData
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required List<Question> Questions { get; set; } = [];
}