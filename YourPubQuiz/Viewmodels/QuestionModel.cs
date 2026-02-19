namespace YourPubQuiz.Viewmodels;

public class QuestionModel
{
    public string Id { get; set; } = "";
    public string Question { get; set; } = "";
    public List<string> PossibleAnswer { get; set; } = [];
}