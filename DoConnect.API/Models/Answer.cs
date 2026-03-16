namespace DoConnect.API.Models
{
    public class Answer
    {
        public int AnswerId { get; set; }

        public int QuestionId { get; set; }

        public int UserId { get; set; }

        public string AnswerText { get; set; }

        public string Status { get; set; }

        public DateTime CreatedDate { get; set; }

        // Navigation Properties
        public Question Question { get; set; }

        public string? ImagePath { get; set; }

        public User User { get; set; }

        public ICollection<Image> Images { get; set; }
    }
}