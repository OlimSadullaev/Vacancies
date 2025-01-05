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
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Country { get; set; }

        public DateTime Deadline { get; set; }

        public string Requirements { get; set; }

        public string FundingAmount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation property for many-to-many relationship
        public virtual ICollection<Category> Categories { get; set; }
    }
}
