using System.ComponentModel.DataAnnotations;

namespace Vacancies.DTOs
{
    public class BaseGrantDTO
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;

        [DataType(DataType.DateTime)]
        public DateTime Deadline { get; set; }

        public string Requirements { get; set; } = string.Empty;

        public string FundingAmount { get; set; } = string.Empty;
    }

    public class GrantDTO : BaseGrantDTO
    {
        public Guid Id { get; set; }
        public List<CategoryDTO> Categories { get; set; } = new List<CategoryDTO>();
    }

    public class CreateGrantDTO : BaseGrantDTO
    {
        [Required(ErrorMessage = "At least one category is required")]
        public List<Guid> CategoryIds { get; set; } = new();
    }

    // Custom validation attribute for future dates
    public class FutureDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is DateTime date)
            {
                return date > DateTime.UtcNow;
            }
            return false;
        }
    }
}
