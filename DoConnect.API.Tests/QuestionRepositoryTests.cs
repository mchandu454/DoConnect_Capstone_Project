using DoConnect.API.Data;
using DoConnect.API.Models;
using DoConnect.API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DoConnect.API.Tests;

public class QuestionRepositoryTests
{
    private static ApplicationDbContext CreateDb()
    {
        var opts = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(opts);
    }

    [Fact]
    public async Task GetByIdAsync_IncludesUserImagesAnswersAndAnswerUser()
    {
        await using var db = CreateDb();

        var asker = new User { UserId = 10, Username = "asker", Password = "p", Role = "User" };
        var answerer = new User { UserId = 11, Username = "answerer", Password = "p", Role = "User" };
        db.Users.AddRange(asker, answerer);

        var q = new Question
        {
            QuestionId = 100,
            UserId = 10,
            Title = "T",
            Description = "D",
            Status = "Approved",
            CreatedDate = DateTime.UtcNow
        };
        db.Questions.Add(q);

        db.Images.Add(new Image { ImageId = 1, QuestionId = 100, ImagePath = "/uploads/q.png" });
        db.Answers.Add(new Answer
        {
            AnswerId = 200,
            QuestionId = 100,
            UserId = 11,
            AnswerText = "A",
            Status = "Approved",
            CreatedDate = DateTime.UtcNow
        });

        await db.SaveChangesAsync();

        var repo = new QuestionRepository(db);
        var loaded = await repo.GetByIdAsync(100);

        Assert.NotNull(loaded);
        Assert.NotNull(loaded!.User);
        Assert.Equal("asker", loaded.User!.Username);

        Assert.Single(loaded.Images);
        Assert.Equal("/uploads/q.png", loaded.Images.First().ImagePath);

        Assert.Single(loaded.Answers);
        Assert.NotNull(loaded.Answers.First().User);
        Assert.Equal("answerer", loaded.Answers.First().User!.Username);
    }

    [Fact]
    public async Task SearchAsync_OnlyReturnsApprovedQuestions()
    {
        await using var db = CreateDb();

        db.Users.Add(new User { UserId = 10, Username = "u", Password = "p", Role = "User" });

        db.Questions.AddRange(
            new Question
            {
                QuestionId = 1,
                UserId = 10,
                Title = "How to unit test?",
                Description = "x",
                Status = "Approved",
                CreatedDate = DateTime.UtcNow
            },
            new Question
            {
                QuestionId = 2,
                UserId = 10,
                Title = "How to unit test?",
                Description = "x",
                Status = "Pending",
                CreatedDate = DateTime.UtcNow
            }
        );

        await db.SaveChangesAsync();

        var repo = new QuestionRepository(db);
        var results = await repo.SearchAsync("UNIT", page: 1, pageSize: 10);

        Assert.Single(results);
        Assert.Equal(1, results[0].QuestionId);
        Assert.Equal("Approved", results[0].Status);
    }
}

