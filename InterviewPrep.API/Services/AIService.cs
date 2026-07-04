using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace InterviewPrep.API.Services
{
    public interface IAIService
    {
        Task<string> ExplainTopicAsync(string topic, string difficulty);
        Task<string> GenerateMockInterviewAsync(string topic, string difficulty);
        Task<string> AnalyzeResumeAsync(string resumeText);
    }

    public class AIService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public AIService(IConfiguration config, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _apiKey = config["Groq:ApiKey"]
                ?? throw new InvalidOperationException("Groq API key not configured");
        }

        private async Task<string> CallGroqAsync(string prompt)
        {
            var url = "https://api.groq.com/openai/v1/chat/completions";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

            var requestBody = new
            {
                model = "llama-3.3-70b-versatile",
                messages = new[]
                {
                    new { role = "user", content = prompt }
                },
                max_tokens = 1500,
                temperature = 0.7
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<JsonElement>();

            var text = result
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return text ?? "No response generated.";
        }

        public async Task<string> ExplainTopicAsync(string topic, string difficulty)
        {
            var prompt = $@"
You are an expert computer science teacher helping a student prepare for coding interviews.

Explain the following topic clearly and concisely:
Topic: {topic}
Difficulty Level: {difficulty}

Please provide:
1. Simple definition (2-3 sentences)
2. Key concepts to remember
3. Time and space complexity (if applicable)
4. A simple example with step-by-step explanation
5. Common interview questions on this topic
6. Tips and tricks

Keep the explanation beginner-friendly but thorough.
Format the response clearly with sections.
";
            return await CallGroqAsync(prompt);
        }

        public async Task<string> GenerateMockInterviewAsync(string topic, string difficulty)
        {
            var prompt = $@"
You are a technical interviewer at a top tech company.

Generate a mock interview question set for:
Topic: {topic}
Difficulty: {difficulty}

Provide:
1. Main Problem Statement (clearly described)
2. Example Input and Output (2-3 examples)
3. Constraints
4. Follow-up questions (2-3)
5. Hints (without giving away the solution)
6. Expected approach/algorithm name

Make it realistic like an actual coding interview question.
Do NOT provide the full solution code.
";
            return await CallGroqAsync(prompt);
        }

        public async Task<string> AnalyzeResumeAsync(string resumeText)
        {
            var prompt = $@"
You are an expert technical recruiter and career advisor specializing in software engineering roles.

Analyze the following resume text and provide detailed feedback:

RESUME:
{resumeText}

Please provide:
1. DETECTED SKILLS
   - Programming Languages
   - Frameworks and Libraries
   - Tools and Technologies
   - Databases

2. STRENGTHS
   - What stands out positively

3. MISSING SKILLS (for software engineering roles)
   - Important skills not mentioned
   - Technologies in high demand

4. SUGGESTIONS
   - How to improve the resume
   - What projects to add
   - What skills to learn next

5. OVERALL RATING (out of 10) with justification

Be specific and actionable in your feedback.
";
            return await CallGroqAsync(prompt);
        }
    }
}
