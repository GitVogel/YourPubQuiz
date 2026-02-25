using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Moq.Protected;
using YourPubQuiz.Models;
using YourPubQuiz.Services;
using YourPubQuiz.Viewmodels;

namespace YourPubQuiz.Test.Services;

public class OpenTdbServiceTests
{
    private OpenTdbService _openTdbService;
    private QuizData _quizData;
    private Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private Mock<IMemoryCache> _memoryCacheMock;

    [SetUp]
    public void Setup()
    {
        _quizData = new QuizData
        {
            Questions =
            [
                new Question()
                {
                    Id = "111",
                    QuestionText = "Queston 1",
                    CorrectAnswer = "CorrectAnswer1",
                    AllAnswers = new List<string> { "WrongAnswer1", "CorrectAnswer1", "WrongAnswer2", "WrongAnswer3" }
                },

                new Question()
                {
                    Id = "222",
                    QuestionText = "Question 2",
                    CorrectAnswer = "False",
                    AllAnswers = new List<string> { "True", "False" }
                }
            ]
        };
        
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        
        _memoryCacheMock = new Mock<IMemoryCache>();
        
        var mockCategories = new
        {
            trivia_categories = new[]
            {
                new { id = 9, name = "General Knowledge" },
                new { id = 20, name = "Mythology" },
                new { id = 32, name = "Entertainment: Cartoon & Animations" }
            }
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .Returns(() => Task.FromResult(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(mockCategories)
            }));
        
        _openTdbService = new OpenTdbService(httpClient, _memoryCacheMock.Object);
    }

    [Test] 
    public void Test_IsValidQuestionAmount_Valid()
    {
        var validAmount = 10;
        var validAmountIsOne = 1;
        var validAmountIsFifty = 50;
        
        Assert.DoesNotThrow(() => OpenTdbService.IsValidQuestionAmount(validAmount));
        Assert.DoesNotThrow(() => OpenTdbService.IsValidQuestionAmount(validAmountIsOne));
        Assert.DoesNotThrow(() => OpenTdbService.IsValidQuestionAmount(validAmountIsFifty));
    }
    
    [Test] 
    public void Test_IsValidQuestionAmount_Invalid()
    {
        var invalidAmountIsZero = 0;
        var invalidAmountIsNegative = -5;
        var invalidAmountIsTooHigh = 51;
        
        var isZeroException = Assert.Throws<ArgumentException>(() => OpenTdbService.IsValidQuestionAmount(invalidAmountIsZero));
        var isNegativeException = Assert.Throws<ArgumentException>(() => OpenTdbService.IsValidQuestionAmount(invalidAmountIsNegative));
        var isHighException = Assert.Throws<ArgumentException>(() => OpenTdbService.IsValidQuestionAmount(invalidAmountIsTooHigh));
        
        Assert.That(isZeroException.Message, Is.EqualTo("Invalid question amount. Must be between 1 and 50."));
        Assert.That(isNegativeException.Message, Is.EqualTo("Invalid question amount. Must be between 1 and 50."));
        Assert.That(isHighException.Message, Is.EqualTo("Invalid question amount. Must be between 1 and 50."));
    }
    
    [Test] 
    public async Task Test_IsValidCategory_Valid()
    {
        var validCategoryGeneralKnowledge = QuestionCategory.GeneralKnowledge;
        var validCategorMythology = QuestionCategory.Mythology;
        var validCategoryCartoons = QuestionCategory.EntertainmentCartoonAnimations;
        
        Assert.DoesNotThrowAsync(async () => await _openTdbService.IsValidCategory((int)validCategoryGeneralKnowledge));
        Assert.DoesNotThrowAsync(async () => await _openTdbService.IsValidCategory((int)validCategorMythology));
        Assert.DoesNotThrowAsync(async () => await _openTdbService.IsValidCategory((int)validCategoryCartoons));
    }
    
    [Test] 
    public void Test_IsValidCategory_Invalid()
    {
        var invalidCategoryNegative = -1;
        var invalidCategoryTooHigh = 999;
        
        var isNegativeException = Assert.ThrowsAsync<ArgumentException>(async () => await _openTdbService.IsValidCategory(invalidCategoryNegative));
        var isHighException = Assert.ThrowsAsync<ArgumentException>(async () => await _openTdbService.IsValidCategory(invalidCategoryTooHigh));
        
        Assert.That(isNegativeException.Message, Is.EqualTo("Invalid category ID."));
        Assert.That(isHighException.Message, Is.EqualTo("Invalid category ID."));
    }
    
    [Test] 
    public void Test_IsValidDifficulty_Valid()
    {
        var validDifficultyEasy = QuestionDifficulty.Easy;
        var validDifficultyMedium = QuestionDifficulty.Medium;
        var validDifficultyHard = QuestionDifficulty.Hard;
        
        Assert.DoesNotThrow(() => OpenTdbService.IsValidDifficulty(validDifficultyEasy));
        Assert.DoesNotThrow(() => OpenTdbService.IsValidDifficulty(validDifficultyMedium));
        Assert.DoesNotThrow(() => OpenTdbService.IsValidDifficulty(validDifficultyHard));
    }
    
    [Test] 
    public void Test_IsValidDifficulty_Invalid()
    {
        QuestionDifficulty? invalidDifficultyNull = null;
        var invalidDifficultyInvalidValue = (QuestionDifficulty)999;
        
        var isNullException = Assert.Throws<ArgumentException>(() => OpenTdbService.IsValidDifficulty(invalidDifficultyNull));
        var isInvalidValueException = Assert.Throws<ArgumentException>(() => OpenTdbService.IsValidDifficulty(invalidDifficultyInvalidValue));
        
        Assert.That(isNullException.Message, Is.EqualTo("Invalid difficulty. Must be Easy, Medium, or Hard."));
        Assert.That(isInvalidValueException.Message, Is.EqualTo("Invalid difficulty. Must be Easy, Medium, or Hard."));
    }
    
    [Test] 
    public void Test_IsValidType_Valid()
    {
        var validTypeMultiple = QuestionType.Multiple;
        var validTypeBoolean = QuestionType.Boolean;
        
        Assert.DoesNotThrow(() => OpenTdbService.IsValidType(validTypeMultiple));
        Assert.DoesNotThrow(() => OpenTdbService.IsValidType(validTypeBoolean));
    }
    
    [Test] 
    public void Test_IsValidType_Invalid()
    {
        QuestionType? invalidTypeNull = null;
        var invalidTypeInvalidValue = (QuestionType)999;
        
        var isNullException = Assert.Throws<ArgumentException>(() => OpenTdbService.IsValidType(invalidTypeNull));
        var isInvalidValueException = Assert.Throws<ArgumentException>(() => OpenTdbService.IsValidType(invalidTypeInvalidValue));
        
        Assert.That(isNullException.Message, Is.EqualTo("Invalid type. Must be Multiple or Boolean."));
        Assert.That(isInvalidValueException.Message, Is.EqualTo("Invalid type. Must be Multiple or Boolean."));
    }
    
    [Test]
    public void Test_CheckAnswers_AllCorrect()
    {
        var quizId = "test-quiz-id";
        var userAnswers = new QuizAnswerModel
        {
            Id = quizId,
            Answers = new List<AnswerModel>
            {
                new() { Id = "111", Answer = "CorrectAnswer1" },
                new() { Id = "222", Answer = "False" }
            }
        };

        object questions = _quizData.Questions;
        _memoryCacheMock.Setup(m => m.TryGetValue(quizId, out questions)).Returns(true);

        var result = _openTdbService.CheckAnswers(userAnswers);

        Assert.That(result.TotalQuestions, Is.EqualTo(2));
        Assert.That(result.CorrectAnswers, Is.EqualTo(2));
        Assert.That(result.QuestionResults.Count, Is.EqualTo(2));
        Assert.That(result.QuestionResults.All(r => r.IsCorrect), Is.True);
    }
    
    [Test]
    public void Test_CheckAnswers_SomeCorrect()
    {
        var quizId = "test-quiz-id";
        var userAnswers = new QuizAnswerModel
        {
            Id = quizId,
            Answers = new List<AnswerModel>
            {
                new() { Id = "111", Answer = "WrongAnswer1" },
                new() { Id = "222", Answer = "False" }
            }
        };

        object questions = _quizData.Questions;
        _memoryCacheMock.Setup(m => m.TryGetValue(quizId, out questions)).Returns(true);

        var result = _openTdbService.CheckAnswers(userAnswers);

        Assert.That(result.TotalQuestions, Is.EqualTo(2));
        Assert.That(result.CorrectAnswers, Is.EqualTo(1));
        Assert.That(result.QuestionResults.Count, Is.EqualTo(2));
        Assert.That(result.QuestionResults.Any(r => r.IsCorrect), Is.True);
    }

    [Test]
    public void Test_CheckAnswers_NoneCorrect()
    {
        var quizId = "test-quiz-id";
        var userAnswers = new QuizAnswerModel
        {
            Id = quizId,
            Answers = new List<AnswerModel>
            {
                new() { Id = "111", Answer = "WrongAnswer3" },
                new() { Id = "222", Answer = "True" }
            }
        };

        object questions = _quizData.Questions;
        _memoryCacheMock.Setup(m => m.TryGetValue(quizId, out questions)).Returns(true);

        var result = _openTdbService.CheckAnswers(userAnswers);

        Assert.That(result.TotalQuestions, Is.EqualTo(2));
        Assert.That(result.CorrectAnswers, Is.EqualTo(0));
        Assert.That(result.QuestionResults.Count, Is.EqualTo(2));
        Assert.That(result.QuestionResults.All(r => r.IsCorrect), Is.False);
    }

    [Test]
    public void Test_CheckAnswers_SessionExpired()
    {
        var quizId = "expired-id";
        var userAnswers = new QuizAnswerModel { Id = quizId };
        
        object? questions = null;
        _memoryCacheMock.Setup(m => m.TryGetValue(quizId, out questions)).Returns(false);

        var exception = Assert.Throws<Exception>(() => _openTdbService.CheckAnswers(userAnswers));
        Assert.That(exception.Message, Is.EqualTo("Quiz session expired or not found."));
    }
}