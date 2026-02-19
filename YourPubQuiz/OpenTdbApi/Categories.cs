using System.Text.Json.Serialization;

namespace YourPubQuiz.OpenTdbApi;

public class Categories
{
    [JsonPropertyName("trivia_categories")]
    public List<Category> CategoryList { get; set; } = [];
}

public class Category
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
}