namespace Vacancies.DTOs
{
    public class GrantDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Country { get; set; }
        public DateTime Deadline { get; set; }
        public string Requirements { get; set; }
        public string FundingAmount { get; set; }
        public List<CategoryDTO> Categories { get; set; }
    }

    public class CreateGrantDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Country { get; set; }
        public DateTime Deadline { get; set; }
        public string Requirements { get; set; }
        public string FundingAmount { get; set; }
        public List<Guid> CategoryIds { get; set; }
    }
}
