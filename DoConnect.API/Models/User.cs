namespace DoConnect.API.Models
{
    public class User
    {
        public int UserId { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Role { get; set; }

        // Navigation Properties
        public ICollection<Question> Questions { get; set; }

        public ICollection<Answer> Answers { get; set; }
    }
}