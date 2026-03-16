using DoConnect.API.DTOs;
using DoConnect.API.Models;
using DoConnect.API.Repositories;
using DoConnect.API.Services;
using Moq;

namespace DoConnect.API.Tests;

public class QuestionServiceTests
{
    [Fact]
    public async Task CreateAsync_WithImagePath_AddsImageAndReturnsDto()
    {
        var repo = new Mock<IQuestionRepository>(MockBehavior.Strict);

        repo.Setup(r => r.CreateAsync(It.IsAny<Question>()))
            .ReturnsAsync((Question q) =>
            {
                q.QuestionId = 123;
                return q;
            });

        repo.Setup(r => r.GetByIdAsync(123))
            .ReturnsAsync(new Question
            {
                QuestionId = 123,
                UserId = 77,
                Title = "t",
                Description = "d",
                ImagePath = "/uploads/x.png",
                Status = "Pending",
                User = null,
                Answers = new List<Answer>(),
                Images = new List<Image> { new() { ImagePath = "/uploads/x.png" } }
            });

        var svc = new QuestionService(repo.Object);

        var dto = await svc.CreateAsync(
            new CreateQuestionDto { Title = "t", Description = "d" },
            userId: 77,
            imagePath: "/uploads/x.png");

        Assert.Equal(123, dto.QuestionId);
        Assert.Equal(77, dto.UserId);
        Assert.Equal("t", dto.Title);
        Assert.Equal("d", dto.Description);
        Assert.Equal("/uploads/x.png", dto.ImagePath);
        Assert.Equal("Pending", dto.Status);
        Assert.Equal(0, dto.AnswerCount);

        repo.Verify(r => r.CreateAsync(It.Is<Question>(q =>
            q.UserId == 77 &&
            q.Title == "t" &&
            q.Description == "d" &&
            q.ImagePath == "/uploads/x.png" &&
            q.Status == "Pending" &&
            q.Images.Count == 1 &&
            q.Images.First().ImagePath == "/uploads/x.png"
        )), Times.Once);

        repo.Verify(r => r.GetByIdAsync(123), Times.Once);
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdateOwnAsync_WhenNotOwner_ReturnsNull()
    {
        var repo = new Mock<IQuestionRepository>(MockBehavior.Strict);
        repo.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(new Question
        {
            QuestionId = 10,
            UserId = 999,
            Title = "old",
            Description = "old",
            Status = "Approved",
            CreatedDate = DateTime.UtcNow
        });

        var svc = new QuestionService(repo.Object);

        var updated = await svc.UpdateOwnAsync(
            questionId: 10,
            userId: 111,
            dto: new UpdateQuestionDto { Title = "new", Description = "new" },
            imagePath: null);

        Assert.Null(updated);
        repo.Verify(r => r.GetByIdAsync(10), Times.Once);
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task DeleteOwnAsync_WhenNotOwner_ReturnsFalse()
    {
        var repo = new Mock<IQuestionRepository>(MockBehavior.Strict);
        repo.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(new Question
        {
            QuestionId = 10,
            UserId = 999,
            Title = "old",
            Description = "old",
            Status = "Approved",
            CreatedDate = DateTime.UtcNow
        });

        var svc = new QuestionService(repo.Object);

        var ok = await svc.DeleteOwnAsync(questionId: 10, userId: 111);

        Assert.False(ok);
        repo.Verify(r => r.GetByIdAsync(10), Times.Once);
        repo.VerifyNoOtherCalls();
    }
}
