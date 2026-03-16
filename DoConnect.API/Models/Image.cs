namespace DoConnect.API.Models
{
    public class Image
    {
        public int ImageId { get; set; }

        public string ImagePath { get; set; }

        public int? QuestionId { get; set; }

        public int? AnswerId { get; set; }

        // Navigation Properties
        public Question Question { get; set; }

        public Answer Answer { get; set; }
    }
}