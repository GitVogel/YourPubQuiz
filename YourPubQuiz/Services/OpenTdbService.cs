using System.Net;
using Microsoft.Extensions.Caching.Memory;
using YourPubQuiz.Models;
using YourPubQuiz.OpenTdbApi;
using YourPubQuiz.Response;
using YourPubQuiz.Viewmodels;

namespace YourPubQuiz.Services;

public class OpenTdbService
{
    private readonly HttpClient _client;
    private readonly IMemoryCache _cache;

    public OpenTdbService(HttpClient client, IMemoryCache cache)
    {
        _client = client;
        _cache = cache;
    }

    /// <summary>
    /// Gets questions from the OpenTDB API based on the provided query parameters (question amount, category, difficulty, and type).
    /// Questions are stored in the question data. The answers are shuffled before storing.
    /// </summary>
    /// <param name="questionAmount"></param>
    /// <param name="category"></param>
    /// <param name="difficulty"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<QuizData> GetQuestions(int questionAmount, QuestionCategory? category,
        QuestionDifficulty? difficulty, QuestionType? type)
    {
        var quizData = new QuizData
        {
            Questions = []
        };

        IsValidQuestionAmount(questionAmount);
        var url = $"https://opentdb.com/api.php?amount={questionAmount}";

        if (category.HasValue && await IsValidCategory((int)category))
        {
            url += $"&category={(int)category}";
        }

        if (difficulty.HasValue && IsValidDifficulty(difficulty))
        {
            url += $"&difficulty={difficulty.Value.ToString().ToLower()}";
        }

        if (type.HasValue && IsValidType(type))
        {
            url += $"&type={type.Value.ToString().ToLower()}";
        }

        var response = await _client.GetFromJsonAsync<OpenTdbResponse>(url);
        if (response?.Questions is null)
        {
            throw new Exception("No questions found");
        }

        var rnd = new Random();
        foreach (var question in response.Questions)
        {
            var allAnswers = question.IncorrectAnswers
                .Append(question.CorrectAnswer)
                .Select(a => WebUtility.HtmlDecode(a))
                .OrderBy(_ => rnd.Next())
                .ToList();

            quizData.Questions.Add(new Question
            {
                Id = question.Id,
                QuestionText = WebUtility.HtmlDecode(question.QuestionText),
                CorrectAnswer = WebUtility.HtmlDecode(question.CorrectAnswer),
                AllAnswers = allAnswers
            });
        }
        
        _cache.Set(quizData.Id, quizData.Questions, TimeSpan.FromHours(1));
        
        return quizData;
    }
    
    /// <summary>
    /// Gets the list of categories from the OpenTDB API and returns it as a list of Category objects.
    /// </summary>
    /// <returns></returns>
    public async Task<List<Category>> GetCategories()
    {
        var response = await _client.GetFromJsonAsync<Categories>("https://opentdb.com/api_category.php");
        if (response is null)
        {
            return [];
        }

        return response.CategoryList.ToList();
    }

    /// <summary>
    /// Checks the provided answers against the correct answers in the question data.
    /// Returns a QuizResult object that includes the total number of questions, the number of correct answers, and a list of QuestionResultDetails for each question.
    /// </summary>
    /// <param name="quizAnswers"></param>
    /// <returns></returns>
    public QuizResult CheckAnswers(QuizAnswerModel quizAnswers)
    {
        if (!_cache.TryGetValue(quizAnswers.Id, out List<Question>? questions))
        {
            throw new Exception("Quiz session expired or not found.");
        }

        var quizResult = new QuizResult
        {
            TotalQuestions = questions!.Count,
            CorrectAnswers = 0,
            QuestionResults = []
        };
        
        foreach (var answer in quizAnswers.Answers)
        {
            var question = questions.FirstOrDefault(q => q.Id == answer.Id);
            if (question is null)
            {
                continue;
            }
            bool isCorrect = question.CorrectAnswer == answer.Answer;
            if (isCorrect)
            {
                quizResult.CorrectAnswers++;
            }
            
            quizResult.QuestionResults.Add(new QuestionResultDetails
            {
                QuestionId = question.Id,
                QuestionText = question.QuestionText,
                IsCorrect = isCorrect,
                UserAnswer = answer.Answer,
                CorrectAnswer = question.CorrectAnswer
            });
        }

        return quizResult;
    }
    
    /// <summary>
    /// Checks if the provided question amount is valid (between 1 and 50).
    /// Others will throw an ArgumentException.
    /// </summary>
    /// <param name="amount"></param>
    /// <exception cref="ArgumentException"></exception>
    internal static bool IsValidQuestionAmount(int amount)
    {
        return amount is <= 0 or > 50 ? throw new ArgumentException("Invalid question amount. Must be between 1 and 50.") : true;
    }

    /// <summary>
    /// Checks if the provided category ID is valid by comparing it against the list of categories from the API.
    /// Others will throw an ArgumentException.
    /// </summary>
    /// <param name="categoryId"></param>
    /// <exception cref="ArgumentException"></exception>
    internal async Task<bool> IsValidCategory(int categoryId)
    {
        var categories = await GetCategories();
        var isValid = categories.Any(c => c.Id == categoryId);
        
        return !isValid ? throw new ArgumentException("Invalid category ID.") : true;
    }

    /// <summary>
    /// Checks if the provided question difficulty is valid (Easy, Medium, or Hard).
    /// Others will throw an ArgumentException.
    /// </summary>
    /// <param name="difficulty"></param>
    /// <exception cref="ArgumentException"></exception>
    internal static bool IsValidDifficulty(QuestionDifficulty? difficulty)
    {
        var isValid = difficulty is QuestionDifficulty.Easy or QuestionDifficulty.Medium or QuestionDifficulty.Hard;
        return !isValid ? throw new ArgumentException("Invalid difficulty. Must be Easy, Medium, or Hard.") : true;
    }

    /// <summary>
    /// Checks if the provided question type is valid (Multiple or Boolean).
    /// Others will throw an ArgumentException.
    /// </summary>
    /// <param name="type"></param>
    /// <exception cref="ArgumentException"></exception>
    internal static bool IsValidType(QuestionType? type)
    {
        var isValid = type is QuestionType.Multiple or QuestionType.Boolean;
        return !isValid ? throw new ArgumentException("Invalid type. Must be Multiple or Boolean.") : true;
    }
}