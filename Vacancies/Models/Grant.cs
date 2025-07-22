using System.ComponentModel.DataAnnotations;

namespace Vacancies.Models
{
    public class Grant
    {
        public Grant()
        {
            Categories = new HashSet<Category>();
        }

        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Country { get; set; } = string.Empty;

        public DateTime Deadline { get; set; }

        public string Requirements { get; set; } = string.Empty;

        public string FundingAmount { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation property for many-to-many relationship
        public virtual ICollection<Category> Categories { get; set; }
    }
}
