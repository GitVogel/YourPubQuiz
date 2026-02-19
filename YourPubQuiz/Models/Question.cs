using System.Text.Json.Serialization;

namespace YourPubQuiz.Models;

public class Question
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [JsonPropertyName("type")]
    public string Type { get; set; } = "";
    
    [JsonPropertyName("difficulty")]
    public string Difficulty { get; set; } = "";
    
    [JsonPropertyName("category")]
    public string Category { get; set; } = "";
    
    [JsonPropertyName("question")]
    public string QuestionText { get; set; } = "";
    
    [JsonPropertyName("correct_answer")]
    public string CorrectAnswer { get; set; } = "";
    
    [JsonPropertyName("incorrect_answers")]
    public List<string> IncorrectAnswers { get; set; } = [];
    
    public List<string> AllAnswers { get; set; } = [];
}