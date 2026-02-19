using YourPubQuiz.Models;

namespace YourPubQuiz.Singletons;

public class QuestionData
{
    public required List<Question> Questions { get; set; } = [];
}