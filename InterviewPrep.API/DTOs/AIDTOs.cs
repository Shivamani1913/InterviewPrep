using System.ComponentModel.DataAnnotations;

namespace InterviewPrep.API.DTOs
{
    public class ExplainTopicDto
    {
        [Required] public string Topic { get; set; } = string.Empty;
        public string Difficulty { get; set; } = "Medium";
    }

    public class MockInterviewDto
    {
        [Required] public string Topic { get; set; } = string.Empty;
        public string Difficulty { get; set; } = "Medium";
    }

    public class ResumeAnalyzeDto
    {
        [Required] public string ResumeText { get; set; } = string.Empty;
    }

    public class AIResponseDto
    {
        public string Result { get; set; } = string.Empty;
        public string Topic { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }
}
