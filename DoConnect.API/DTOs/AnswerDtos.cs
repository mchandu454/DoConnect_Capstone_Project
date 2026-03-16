using System;
using System.ComponentModel.DataAnnotations;

namespace DoConnect.API.DTOs
{
    public class CreateAnswerDto
    {
        [Required]
        public int QuestionId { get; set; }

        [Required]
        public string AnswerText { get; set; } = string.Empty;
    }

    public class UpdateAnswerDto
    {
        [Required]
        public string AnswerText { get; set; } = string.Empty;
    }

    public class AnswerResponseDto
    {
        public int AnswerId { get; set; }
        public int QuestionId { get; set; }
        public string? QuestionTitle { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string AnswerText { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }
}
