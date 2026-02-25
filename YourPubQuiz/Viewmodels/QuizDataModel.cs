namespace YourPubQuiz.Viewmodels;

public class QuizDataModel
{
    public string Id { get; set; } = "";
    public List<QuestionModel> Questions { get; set; } = [];
}

public class QuestionModel
{
    public string Id { get; set; } = "";
    public string Question { get; set; } = "";
    public List<string> PossibleAnswer { get; set; } = [];
}