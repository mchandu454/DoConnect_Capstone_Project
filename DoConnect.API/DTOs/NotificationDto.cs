using System;

namespace DoConnect.API.DTOs
{
    public class NotificationDto
    {
        public string Id { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        // Expected values: "question" | "answer"
        public string Type { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public bool Read { get; set; } = false;
    }
}

