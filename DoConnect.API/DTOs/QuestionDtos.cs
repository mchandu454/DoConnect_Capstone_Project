using System;
using System.ComponentModel.DataAnnotations;

namespace DoConnect.API.DTOs
{
    public class CreateQuestionDto
    {
        [Required, MaxLength(300)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;
    }

    public class UpdateQuestionDto
    {
        [Required, MaxLength(300)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;
    }

    public class QuestionResponseDto
    {
        public int QuestionId { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public int AnswerCount { get; set; }
    }

    public class QuestionDetailDto : QuestionResponseDto
    {
        public List<AnswerResponseDto> Answers { get; set; } = new();
    }
}
