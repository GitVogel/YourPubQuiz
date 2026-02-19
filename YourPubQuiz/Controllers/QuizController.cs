using System.Net;
using Microsoft.AspNetCore.Mvc;
using YourPubQuiz.Models;
using YourPubQuiz.OpenTdbApi;
using YourPubQuiz.Response;
using YourPubQuiz.Singletons;
using YourPubQuiz.Viewmodels;

namespace YourPubQuiz.Controllers;

[ApiController]
[Route("[controller]")]
public class QuizController : ControllerBase
{
    private readonly HttpClient _client;
    private readonly QuestionData _questionData;

    public QuizController(HttpClient client, QuestionData questionData)
    {
        _client = client;
        _questionData = questionData;
    }

    /// <summary>
    /// Gets questions from the OpenTDB API based on the provided query parameters (question amount, category, difficulty, and type).
    /// Questions are stored in the question data. The answers are shuffeled before storing.
    /// </summary>
    /// <param name="questionAmount"></param>
    /// <param name="category"></param>
    /// <param name="difficulty"></param>
    /// <param name="type"></param>
    /// <returns>List of QuestionModel</returns>
    /// <exception cref="Exception"></exception>
    [HttpGet]
    [Route("[action]")]
    public async Task<List<QuestionModel>> GetQuestions([FromQuery] int questionAmount, QuestionCategory? category,
        QuestionDifficulty? difficulty, QuestionType? type)
    {
        _questionData.Questions.Clear();

        IsValidQuestionAmount(questionAmount);
        var url = $"https://opentdb.com/api.php?amount={questionAmount}";

        if (category.HasValue)
        {
            await IsValidCategory((int)category);
            url += $"&category={(int)category}";
        }

        if (difficulty.HasValue)
        {
            IsValidDifficulty(difficulty);
            url += $"&difficulty={difficulty.Value.ToString().ToLower()}";
        }

        if (type.HasValue)
        {
            IsValidType(type);
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

            _questionData.Questions.Add(new Question
            {
                Id = question.Id,
                QuestionText = WebUtility.HtmlDecode(question.QuestionText),
                CorrectAnswer = WebUtility.HtmlDecode(question.CorrectAnswer),
                AllAnswers = allAnswers
            });
        }

        return _questionData.Questions.Select(q => new QuestionModel
        {
            Id = q.Id,
            Question = q.QuestionText,
            PossibleAnswer = q.AllAnswers
        }).ToList();
    }

    /// <summary>
    /// Gets the list of categories from the OpenTDB API and returns it as a list of CategoryModel.
    /// </summary>
    /// <returns>List of CategoryModel</returns>
    [HttpGet]
    [Route("[action]")]
    public async Task<List<CategoryModel>> GetCategories()
    {
        var response = await _client.GetFromJsonAsync<Categories>("https://opentdb.com/api_category.php");
        if (response is null)
        {
            return [];
        }

        return response.CategoryList.Select(c => new CategoryModel
        {
            Id = c.Id,
            Name = c.Name
        }).ToList();
    }

    /// <summary>
    /// Checks the provided answers against the correct answers in the question data and returns a list of QuestionResult.
    /// The QuestionResult includes the question ID, question text, whether the user's answer was correct, the user's answer, and the correct answer.
    /// </summary>
    /// <param name="answers"></param>
    /// <returns>List of QuestionResult</returns>
    [HttpPost("[action]")]
    public QuizResultModel CheckAnswers([FromBody] List<AnswerModel> answers)
    {
        var quizResult = new QuizResultModel
        {
            TotalQuestions = _questionData.Questions.Count,
            CorrectAnswers = 0,
            QuestionResults = []
        };
        
        foreach (var answer in answers)
        {
            var question = _questionData.Questions.FirstOrDefault(q => q.Id == answer.Id);
            if (question is null)
            {
                continue;
            }
            bool isCorrect = question.CorrectAnswer == answer.Answer;
            if (isCorrect)
            {
                quizResult.CorrectAnswers++;
            }
            
            quizResult.QuestionResults.Add(new QuestionResult
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
    internal static void IsValidQuestionAmount(int amount)
    {
        if (amount is <= 0 or > 50)
        {
            throw new ArgumentException("Invalid question amount. Must be between 1 and 50.");
        }
    }

    /// <summary>
    /// Checks if the provided category ID is valid by comparing it against the list of categories from the API.
    /// Others will throw an ArgumentException.
    /// </summary>
    /// <param name="categoryId"></param>
    /// <exception cref="ArgumentException"></exception>
    internal async Task IsValidCategory(int categoryId)
    {
        var categories = await GetCategories();
        
        var isValid = categories.Any(c => c.Id == categoryId);
        if (!isValid)
        {
            throw new ArgumentException("Invalid category ID.");
        }
    }

    /// <summary>
    /// Checks if the provided question difficulty is valid (Easy, Medium, or Hard).
    /// Others will throw an ArgumentException.
    /// </summary>
    /// <param name="difficulty"></param>
    /// <exception cref="ArgumentException"></exception>
    internal static void IsValidDifficulty(QuestionDifficulty? difficulty)
    {
        var isValid = difficulty is QuestionDifficulty.Easy or QuestionDifficulty.Medium or QuestionDifficulty.Hard;
        if (!isValid)
        {
            throw new ArgumentException("Invalid difficulty. Must be Easy, Medium, or Hard.");
        }
    }

    /// <summary>
    /// Checks if the provided question type is valid (Multiple or Boolean).
    /// Others will throw an ArgumentException.
    /// </summary>
    /// <param name="type"></param>
    /// <exception cref="ArgumentException"></exception>
    internal static void IsValidType(QuestionType? type)
    {
        var isValid = type is QuestionType.Multiple or QuestionType.Boolean;
        if (!isValid)
        {
            throw new ArgumentException("Invalid type. Must be Multiple or Boolean.");
        }
    }
}