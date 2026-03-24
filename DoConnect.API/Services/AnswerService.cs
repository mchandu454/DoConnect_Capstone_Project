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
    public interface IAnswerService
    {
        Task<AnswerResponseDto?> CreateAsync(CreateAnswerDto dto, int userId, string? imagePath);
        Task<List<AnswerResponseDto>> GetByQuestionIdAsync(int questionId);
        Task<List<AnswerResponseDto>> GetPendingAsync();
        Task<List<AnswerResponseDto>> GetHistoryAsync();
        Task<List<AnswerResponseDto>> GetByUserIdAsync(int userId);
        Task<AnswerResponseDto?> UpdateOwnAsync(int answerId, int userId, UpdateAnswerDto dto, string? imagePath);
        Task<bool> DeleteOwnAsync(int answerId, int userId);
    }

    public class AnswerService : IAnswerService
    {
        private readonly IAnswerRepository _answerRepo;
        private readonly IQuestionRepository _questionRepo;
        private readonly IHubContext<NotificationsHub> _hubContext;

        public AnswerService(
            IAnswerRepository answerRepo,
            IQuestionRepository questionRepo,
            IHubContext<NotificationsHub> hubContext)
        {
            _answerRepo = answerRepo;
            _questionRepo = questionRepo;
            _hubContext = hubContext;
        }

        public async Task<AnswerResponseDto?> CreateAsync(CreateAnswerDto dto, int userId, string? imagePath)
        {
            var question = await _questionRepo.GetByIdAsync(dto.QuestionId);

            if (question == null || question.Status != "Approved")
                return null;

            var answer = new Answer
            {
                QuestionId = dto.QuestionId,
                UserId = userId,
                AnswerText = dto.AnswerText,
                ImagePath = imagePath,
                Status = "Pending",
                CreatedDate = DateTime.UtcNow,
                Images = new List<Image>()
            };

            if (!string.IsNullOrWhiteSpace(imagePath))
            {
                answer.Images.Add(new Image
                {
                    ImagePath = imagePath
                });
            }

            await _answerRepo.CreateAsync(answer);

            var created = await _answerRepo.GetByIdAsync(answer.AnswerId);

            if (created != null)
            {
                var notification = new NotificationDto
                {
                    Id = $"answer-{created.AnswerId}",
                    Message = $"New answer posted on '{created.Question?.Title}' by {created.User?.Username}",
                    Type = "answer",
                    Timestamp = created.CreatedDate,
                    Read = false
                };

                try
                {
                    NotificationsHub.AddRecent(notification);
                    await _hubContext.Clients.Group(NotificationsHub.AdminGroup)
                        .SendAsync("ReceiveNotification", notification);

                    Console.WriteLine($"[SignalR] Sent answer notification. Id={notification.Id}, Group={NotificationsHub.AdminGroup}");
                    Console.WriteLine($"[SignalR] ConnectedAdminsCount={NotificationsHub.ConnectedAdmins.Count}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[SignalR] Failed to send answer notification. Error: {ex.Message}");
                }
            }

            return MapToDto(created!);
        }

        public async Task<List<AnswerResponseDto>> GetByQuestionIdAsync(int questionId)
        {
            var answers = await _answerRepo.GetByQuestionIdAsync(questionId);

            return answers.Select(MapToDto).ToList();
        }

        public async Task<List<AnswerResponseDto>> GetPendingAsync()
        {
            var answers = await _answerRepo.GetPendingAsync();

            return answers.Select(MapToDto).ToList();
        }

        public async Task<List<AnswerResponseDto>> GetHistoryAsync()
        {
            var answers = await _answerRepo.GetHistoryAsync();

            return answers.Select(MapToDto).ToList();
        }

        public async Task<List<AnswerResponseDto>> GetByUserIdAsync(int userId)
        {
            var answers = await _answerRepo.GetByUserIdAsync(userId);
            return answers.Select(MapToDto).ToList();
        }

        public async Task<AnswerResponseDto?> UpdateOwnAsync(int answerId, int userId, UpdateAnswerDto dto, string? imagePath)
        {
            var a = await _answerRepo.GetByIdAsync(answerId);
            if (a == null || a.UserId != userId) return null;
            var toUpdate = new Answer
            {
                AnswerId = answerId,
                QuestionId = a.QuestionId,
                UserId = a.UserId,
                AnswerText = dto.AnswerText,
                ImagePath = imagePath ?? a.ImagePath,
                Status = a.Status,
                CreatedDate = a.CreatedDate
            };
            await _answerRepo.UpdateAsync(toUpdate);
            var updated = await _answerRepo.GetByIdAsync(answerId);
            return updated != null ? MapToDto(updated) : null;
        }

        public async Task<bool> DeleteOwnAsync(int answerId, int userId)
        {
            var a = await _answerRepo.GetByIdAsync(answerId);
            if (a == null || a.UserId != userId) return false;
            var toDelete = new Answer
            {
                AnswerId = answerId,
                QuestionId = a.QuestionId,
                UserId = a.UserId,
                AnswerText = a.AnswerText,
                ImagePath = a.ImagePath,
                Status = "Deleted",
                CreatedDate = a.CreatedDate
            };
            await _answerRepo.DeleteAsync(toDelete);
            return true;
        }

        private static AnswerResponseDto MapToDto(Answer a) => new()
        {
            AnswerId = a.AnswerId,
            QuestionId = a.QuestionId,
            QuestionTitle = a.Question?.Title,
            UserId = a.UserId,
            Username = a.User?.Username ?? "",
            AnswerText = a.AnswerText,
            ImagePath = a.ImagePath,
            Status = a.Status,
            CreatedDate = a.CreatedDate
        };
    }
}