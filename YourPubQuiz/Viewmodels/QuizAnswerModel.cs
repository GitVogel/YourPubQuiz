using System.Text.Json.Serialization;

namespace YourPubQuiz.Viewmodels;

public class QuizAnswerModel
{
    public string Id { get; set; } = "";
    public List<AnswerModel> Answers { get; set; } = [];
}

public class AnswerModel
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";
    
    [JsonPropertyName("answer")]
    public string Answer { get; set; } = "";
}