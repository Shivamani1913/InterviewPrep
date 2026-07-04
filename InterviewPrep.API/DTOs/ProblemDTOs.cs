using System.ComponentModel.DataAnnotations;

namespace InterviewPrep.API.DTOs
{
    public class CreateProblemDto
    {
        [Required]
        [MaxLength(300)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Platform { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^(Easy|Medium|Hard)$", ErrorMessage = "Difficulty must be Easy, Medium, or Hard")]
        public string Difficulty { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Topic { get; set; } = string.Empty;

        public DateOnly? SolvedDate { get; set; }

        [MaxLength(2000)]
        public string? Notes { get; set; }

        [MaxLength(500)]
        public string? ProblemUrl { get; set; }
    }

    public class UpdateProblemDto
    {
        [MaxLength(300)]
        public string? Title { get; set; }

        [MaxLength(100)]
        public string? Platform { get; set; }

        [RegularExpression("^(Easy|Medium|Hard)$")]
        public string? Difficulty { get; set; }

        [MaxLength(100)]
        public string? Topic { get; set; }

        public DateOnly? SolvedDate { get; set; }

        [MaxLength(2000)]
        public string? Notes { get; set; }

        [MaxLength(500)]
        public string? ProblemUrl { get; set; }
    }

    public class ProblemResponseDto
    {
        public Guid ProblemId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Platform { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty;
        public string Topic { get; set; } = string.Empty;
        public DateOnly SolvedDate { get; set; }
        public string? Notes { get; set; }
        public string? ProblemUrl { get; set; }
        public Guid UserId { get; set; }
    }

    public class PagedResponseDto<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public bool HasNextPage => Page * PageSize < TotalCount;
        public bool HasPreviousPage => Page > 1;
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }

    public class ProblemFilterDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? Search { get; set; }
        public string? Difficulty { get; set; }
        public string? Topic { get; set; }
        public string? Platform { get; set; }
    }
}
