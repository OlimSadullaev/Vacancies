using System.ComponentModel.DataAnnotations;

namespace Vacancies.DTOs
{
    public class BaseGrantDTO
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(5000, ErrorMessage = "Description cannot exceed 5000 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Country is required")]
        [StringLength(100, ErrorMessage = "Country cannot exceed 100 characters")]
        public string Country { get; set; }

        [Required(ErrorMessage = "Deadline is required")]
        [DataType(DataType.DateTime)]
        [FutureDate(ErrorMessage = "Deadline must be in the future")]
        public DateTime Deadline { get; set; }

        [StringLength(2000, ErrorMessage = "Requirements cannot exceed 2000 characters")]
        public string Requirements { get; set; }

        [StringLength(100, ErrorMessage = "Funding amount cannot exceed 100 characters")]
        public string FundingAmount { get; set; }
    }

    public class GrantDTO : BaseGrantDTO
    {
        public int Id { get; set; }
        public List<CategoryDTO> Categories { get; set; } = new List<CategoryDTO>();
    }

    public class CreateGrantDTO : BaseGrantDTO
    {
        public List<Guid> CategoryIds { get; set; } = new List<Guid>();
    }

    // Custom validation attribute for future dates
    public class FutureDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is DateTime date)
            {
                return date > DateTime.UtcNow;
            }
            return false;
        }
    }
}
