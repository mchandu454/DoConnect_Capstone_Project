using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using DoConnect.API.DTOs;
using DoConnect.API.Hubs;
using DoConnect.API.Models;
using DoConnect.API.Repositories;

namespace DoConnect.API.Services
{
    public interface IQuestionService
    {
        Task<QuestionResponseDto> CreateAsync(CreateQuestionDto dto, int userId, string? imagePath);
        Task<QuestionDetailDto?> GetByIdAsync(int id);
        Task<(List<QuestionResponseDto> Items, int Total)> GetAllApprovedAsync(int page, int pageSize);
        Task<(List<QuestionResponseDto> Items, int Total)> SearchAsync(string query, int page, int pageSize);
        Task<List<QuestionResponseDto>> GetPendingAsync();
        Task<List<QuestionResponseDto>> GetHistoryAsync();
        Task<List<QuestionResponseDto>> GetByUserIdAsync(int userId);
        Task<QuestionResponseDto?> UpdateOwnAsync(int questionId, int userId, UpdateQuestionDto dto, string? imagePath);
        Task<bool> DeleteOwnAsync(int questionId, int userId);
    }

    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _questionRepo;
        private readonly IHubContext<NotificationsHub> _hubContext;

        public QuestionService(IQuestionRepository questionRepo, IHubContext<NotificationsHub> hubContext)
        {
            _questionRepo = questionRepo;
            _hubContext = hubContext;
        }

        public async Task<QuestionResponseDto> CreateAsync(CreateQuestionDto dto, int userId, string? imagePath)
        {
            var question = new Question
            {
                UserId = userId,
                Title = dto.Title,
                Description = dto.Description,
                ImagePath = imagePath,
                Status = "Pending"
            };

            if (!string.IsNullOrWhiteSpace(imagePath))
            {
                question.Images.Add(new Image
                {
                    ImagePath = imagePath
                });
            }

            await _questionRepo.CreateAsync(question);

            var created = await _questionRepo.GetByIdAsync(question.QuestionId);

            if (created != null)
            {
                var notification = new NotificationDto
                {
                    Id = $"question-{created.QuestionId}",
                    Message = $"New question posted: {created.Title} by {created.User?.Username}",
                    Type = "question",
                    Timestamp = created.CreatedDate,
                    Read = false
                };

                try
                {
                    NotificationsHub.AddRecent(notification);
                    await _hubContext.Clients.Group(NotificationsHub.AdminGroup)
                        .SendAsync("ReceiveNotification", notification);

                    Console.WriteLine($"[SignalR] Sent question notification. Id={notification.Id}, Group={NotificationsHub.AdminGroup}");
                    Console.WriteLine($"[SignalR] ConnectedAdminsCount={NotificationsHub.ConnectedAdmins.Count}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[SignalR] Failed to send question notification. Error: {ex.Message}");
                }
            }

            return MapToDto(created);
        }

        public async Task<QuestionDetailDto?> GetByIdAsync(int id)
        {
            var q = await _questionRepo.GetByIdAsync(id);
            if (q == null) return null;

            var dto = new QuestionDetailDto
            {
                QuestionId = q.QuestionId,
                UserId = q.UserId,
                Username = q.User?.Username ?? "",
                Title = q.Title,
                Description = q.Description,
                ImagePath = q.ImagePath,
                Status = q.Status,
                CreatedDate = q.CreatedDate,
                AnswerCount = q.Answers?.Count ?? 0,
                Answers = q.Answers.Select(a => new AnswerResponseDto
                {
                    AnswerId = a.AnswerId,
                    QuestionId = a.QuestionId,
                    UserId = a.UserId,
                    Username = a.User?.Username ?? "",
                    AnswerText = a.AnswerText,
                    ImagePath = a.ImagePath,
                    Status = a.Status,
                    CreatedDate = a.CreatedDate
                }).ToList()
            };

            return dto;
        }

        public async Task<(List<QuestionResponseDto> Items, int Total)> GetAllApprovedAsync(int page, int pageSize)
        {
            var items = await _questionRepo.GetAllApprovedAsync(page, pageSize);
            var total = await _questionRepo.CountApprovedAsync();

            return (items.Select(MapToDto).ToList(), total);
        }

        public async Task<(List<QuestionResponseDto> Items, int Total)> SearchAsync(string query, int page, int pageSize)
        {
            var items = await _questionRepo.SearchAsync(query, page, pageSize);
            var total = await _questionRepo.CountSearchAsync(query);

            return (items.Select(MapToDto).ToList(), total);
        }

        public async Task<List<QuestionResponseDto>> GetPendingAsync()
        {
            var items = await _questionRepo.GetPendingAsync();
            return items.Select(MapToDto).ToList();
        }

        public async Task<List<QuestionResponseDto>> GetHistoryAsync()
        {
            var items = await _questionRepo.GetHistoryAsync();
            return items.Select(MapToDto).ToList();
        }

        public async Task<List<QuestionResponseDto>> GetByUserIdAsync(int userId)
        {
            var items = await _questionRepo.GetByUserIdAsync(userId);
            return items.Select(MapToDto).ToList();
        }

        public async Task<QuestionResponseDto?> UpdateOwnAsync(int questionId, int userId, UpdateQuestionDto dto, string? imagePath)
        {
            var q = await _questionRepo.GetByIdAsync(questionId);
            if (q == null || q.UserId != userId) return null;
            var toUpdate = new Question
            {
                QuestionId = questionId,
                UserId = q.UserId,
                Title = dto.Title,
                Description = dto.Description,
                ImagePath = imagePath ?? q.ImagePath,
                Status = q.Status,
                CreatedDate = q.CreatedDate
            };
            await _questionRepo.UpdateAsync(toUpdate);
            var updated = await _questionRepo.GetByIdAsync(questionId);
            return updated != null ? MapToDto(updated) : null;
        }

        public async Task<bool> DeleteOwnAsync(int questionId, int userId)
        {
            var q = await _questionRepo.GetByIdAsync(questionId);
            if (q == null || q.UserId != userId) return false;
            var toDelete = new Question
            {
                QuestionId = questionId,
                UserId = q.UserId,
                Title = q.Title,
                Description = q.Description,
                ImagePath = q.ImagePath,
                Status = "Deleted",
                CreatedDate = q.CreatedDate
            };
            await _questionRepo.DeleteAsync(toDelete);
            return true;
        }

        private static QuestionResponseDto MapToDto(Question q) => new()
        {
            QuestionId = q.QuestionId,
            UserId = q.UserId,
            Username = q.User?.Username ?? "",
            Title = q.Title,
            Description = q.Description,
            ImagePath = q.ImagePath,
            Status = q.Status,
            CreatedDate = q.CreatedDate,
            AnswerCount = q.Answers?.Count ?? 0
        };
    }
}