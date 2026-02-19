using System.Text.Json.Serialization;
using YourPubQuiz.Models;

namespace YourPubQuiz.Response;

public class OpenTdbResponse
{
    [JsonPropertyName("response_code")]
    public int ResponseCode { get; set; }
    
    [JsonPropertyName("results")]
    public List<Question> Questions { get; set; } = [];
}