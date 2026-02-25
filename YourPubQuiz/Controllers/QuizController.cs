using Microsoft.AspNetCore.Mvc;
using YourPubQuiz.Models;
using YourPubQuiz.Services;
using YourPubQuiz.Viewmodels;

namespace YourPubQuiz.Controllers;

[ApiController]
[Route("[controller]")]
public class QuizController : ControllerBase
{
    private readonly OpenTdbService _openTdbService;

    public QuizController(OpenTdbService openTdbService)
    {
        _openTdbService = openTdbService;
    }

    /// <summary>
    /// Gets A list of questions from the OpenTdbService based on the provided question settings.
    /// </summary>
    /// <param name="questionAmount"></param>
    /// <param name="category"></param>
    /// <param name="difficulty"></param>
    /// <param name="type"></param>
    /// <returns>List of QuestionModel</returns>
    /// <exception cref="Exception"></exception>
    [HttpGet]
    [Route("[action]")]
    public async Task<QuizDataModel> GetQuestions([FromQuery] int questionAmount, QuestionCategory? category,
        QuestionDifficulty? difficulty, QuestionType? type)
    {
        var quizData = await _openTdbService.GetQuestions(questionAmount, category, difficulty, type);

        return new QuizDataModel
        {
            Id = quizData.Id,
            Questions = quizData.Questions.Select(q => new QuestionModel
            {
                Id = q.Id,
                Question = q.QuestionText,
                PossibleAnswer = q.AllAnswers
            }).ToList()
        };
    }

    /// <summary>
    /// Gets the list of categories from the OpenTdbService and returns it as a list of CategoryModel.
    /// </summary>
    /// <returns>List of CategoryModel</returns>
    [HttpGet]
    [Route("[action]")]
    public async Task<List<CategoryModel>> GetCategories()
    {
        var categories = await _openTdbService.GetCategories();

        return categories.Select(c => new CategoryModel
        {
            Id = c.Id,
            Name = c.Name
        }).ToList();
    }

    /// <summary>
    /// Checks the provided answers against the correct answers stored in the OpenTdbService and returns a QuizResultModel.
    /// </summary>
    /// <param name="quizAnswers"></param>
    /// <returns>List of QuestionResult</returns>
    [HttpPost("[action]")]
    public QuizResultModel CheckAnswers([FromBody] QuizAnswerModel quizAnswers)
    {
        var result = _openTdbService.CheckAnswers(quizAnswers);

        return new QuizResultModel
        {
            TotalQuestions = result.TotalQuestions,
            CorrectAnswers = result.CorrectAnswers,
            QuestionResults = result.QuestionResults.Select(r => new QuestionResult
            {
                QuestionId = r.QuestionId,
                QuestionText = r.QuestionText,
                IsCorrect = r.IsCorrect,
                UserAnswer = r.UserAnswer,
                CorrectAnswer = r.CorrectAnswer
            }).ToList()
        };
    }
}