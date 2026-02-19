using System.Net;
using System.Net.Http.Json;
using Moq;
using Moq.Protected;
using YourPubQuiz.Controllers;
using YourPubQuiz.Models;
using YourPubQuiz.Singletons;
using YourPubQuiz.Viewmodels;

namespace YourPubQuiz.Test.Controllers;

public class QuizControllerTests
{
    private QuizController _quizController;
    private QuestionData _questionData;
    private Mock<HttpMessageHandler> _httpMessageHandlerMock;

    [SetUp]
    public void Setup()
    {
        _questionData = new QuestionData
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
        
        _quizController = new QuizController(httpClient, _questionData);
    }

    [Test] 
    public void Test_IsValidQuestionAmount_Valid()
    {
        var validAmount = 10;
        var validAmountIsOne = 1;
        var validAmountIsFifty = 50;
        
        Assert.DoesNotThrow(() => QuizController.IsValidQuestionAmount(validAmount));
        Assert.DoesNotThrow(() => QuizController.IsValidQuestionAmount(validAmountIsOne));
        Assert.DoesNotThrow(() => QuizController.IsValidQuestionAmount(validAmountIsFifty));
    }
    
    [Test] 
    public void Test_IsValidQuestionAmount_Invalid()
    {
        var invalidAmountIsZero = 0;
        var invalidAmountIsNegative = -5;
        var invalidAmountIsTooHigh = 51;
        
        var isZeroException = Assert.Throws<ArgumentException>(() => QuizController.IsValidQuestionAmount(invalidAmountIsZero));
        var isNegativeException = Assert.Throws<ArgumentException>(() => QuizController.IsValidQuestionAmount(invalidAmountIsNegative));
        var isHighException = Assert.Throws<ArgumentException>(() => QuizController.IsValidQuestionAmount(invalidAmountIsTooHigh));
        
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
        
        Assert.DoesNotThrowAsync(async () => await _quizController.IsValidCategory((int)validCategoryGeneralKnowledge));
        Assert.DoesNotThrowAsync(async () => await _quizController.IsValidCategory((int)validCategorMythology));
        Assert.DoesNotThrowAsync(async () => await _quizController.IsValidCategory((int)validCategoryCartoons));
    }
    
    [Test] 
    public void Test_IsValidCategory_Invalid()
    {
        var invalidCategoryNegative = -1;
        var invalidCategoryTooHigh = 999;
        
        var isNegativeException = Assert.ThrowsAsync<ArgumentException>(async () => await _quizController.IsValidCategory(invalidCategoryNegative));
        var isHighException = Assert.ThrowsAsync<ArgumentException>(async () => await _quizController.IsValidCategory(invalidCategoryTooHigh));
        
        Assert.That(isNegativeException.Message, Is.EqualTo("Invalid category ID."));
        Assert.That(isHighException.Message, Is.EqualTo("Invalid category ID."));
    }
    
    [Test] 
    public void Test_IsValidDifficulty_Valid()
    {
        var validDifficultyEasy = QuestionDifficulty.Easy;
        var validDifficultyMedium = QuestionDifficulty.Medium;
        var validDifficultyHard = QuestionDifficulty.Hard;
        
        Assert.DoesNotThrow(() => QuizController.IsValidDifficulty(validDifficultyEasy));
        Assert.DoesNotThrow(() => QuizController.IsValidDifficulty(validDifficultyMedium));
        Assert.DoesNotThrow(() => QuizController.IsValidDifficulty(validDifficultyHard));
    }
    
    [Test] 
    public void Test_IsValidDifficulty_Invalid()
    {
        QuestionDifficulty? invalidDifficultyNull = null;
        var invalidDifficultyInvalidValue = (QuestionDifficulty)999;
        
        var isNullException = Assert.Throws<ArgumentException>(() => QuizController.IsValidDifficulty(invalidDifficultyNull));
        var isInvalidValueException = Assert.Throws<ArgumentException>(() => QuizController.IsValidDifficulty(invalidDifficultyInvalidValue));
        
        Assert.That(isNullException.Message, Is.EqualTo("Invalid difficulty. Must be Easy, Medium, or Hard."));
        Assert.That(isInvalidValueException.Message, Is.EqualTo("Invalid difficulty. Must be Easy, Medium, or Hard."));
    }
    
    [Test] 
    public void Test_IsValidType_Valid()
    {
        var validTypeMultiple = QuestionType.Multiple;
        var validTypeBoolean = QuestionType.Boolean;
        
        Assert.DoesNotThrow(() => QuizController.IsValidType(validTypeMultiple));
        Assert.DoesNotThrow(() => QuizController.IsValidType(validTypeBoolean));
    }
    
    [Test] 
    public void Test_IsValidType_Invalid()
    {
        QuestionType? invalidTypeNull = null;
        var invalidTypeInvalidValue = (QuestionType)999;
        
        var isNullException = Assert.Throws<ArgumentException>(() => QuizController.IsValidType(invalidTypeNull));
        var isInvalidValueException = Assert.Throws<ArgumentException>(() => QuizController.IsValidType(invalidTypeInvalidValue));
        
        Assert.That(isNullException.Message, Is.EqualTo("Invalid type. Must be Multiple or Boolean."));
        Assert.That(isInvalidValueException.Message, Is.EqualTo("Invalid type. Must be Multiple or Boolean."));
    }
    
    [Test]
    public void Test_CheckAnswers_AllCorrect()
    {
        var userAnswers = new List<AnswerModel>
        {
            new() { Id = "111", Answer = "CorrectAnswer1" },
            new () { Id = "222", Answer = "False" }
        };

        var result = _quizController.CheckAnswers(userAnswers);

        Assert.That(result.TotalQuestions, Is.EqualTo(2));
        Assert.That(result.CorrectAnswers, Is.EqualTo(2));
        Assert.That(result.QuestionResults.Count, Is.EqualTo(2));
        Assert.That(result.QuestionResults.All(r => r.IsCorrect), Is.True);
    }
    
    [Test]
    public void Test_CheckAnswers_SomeCorrect()
    {
        var userAnswers = new List<AnswerModel>
        {
            new() { Id = "111", Answer = "WrongAnswer1" },
            new () { Id = "222", Answer = "False" }
        };

        var result = _quizController.CheckAnswers(userAnswers);

        Assert.That(result.TotalQuestions, Is.EqualTo(2));
        Assert.That(result.CorrectAnswers, Is.EqualTo(1));
        Assert.That(result.QuestionResults.Count, Is.EqualTo(2));
        Assert.That(result.QuestionResults.Any(r => r.IsCorrect), Is.True);
    }

    [Test]
    public void Test_CheckAnswers_NoneCorrect()
    {
        var userAnswers = new List<AnswerModel>
        {
            new() { Id = "111", Answer = "WrongAnswer3" },
            new () { Id = "222", Answer = "True" }
        };

        var result = _quizController.CheckAnswers(userAnswers);

        Assert.That(result.TotalQuestions, Is.EqualTo(2));
        Assert.That(result.CorrectAnswers, Is.EqualTo(0));
        Assert.That(result.QuestionResults.Count, Is.EqualTo(2));
        Assert.That(result.QuestionResults.All(r => r.IsCorrect), Is.False);
    }
}