using System.ComponentModel.DataAnnotations;

namespace Vacancies.Models
{
    public class Category
    {
        public Category()
        {
            Id = Guid.NewGuid();
            Grants = new HashSet<Grant>();
        }

        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        public virtual ICollection<Grant> Grants { get; set; }
    }
}
