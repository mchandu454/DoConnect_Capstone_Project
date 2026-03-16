using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoConnect.API.DTOs;
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

        public AnswerService(IAnswerRepository answerRepo, IQuestionRepository questionRepo)
        {
            _answerRepo = answerRepo;
            _questionRepo = questionRepo;
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