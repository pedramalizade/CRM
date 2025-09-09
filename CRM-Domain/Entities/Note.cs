namespace CRM.Domain.Entities
{
    public class Note
    {
        public int Id { get; set; }

        [StringLength(500)]
        [Required(ErrorMessage = "لطفاً محتوای یادداشت را وارد کنید")]
        public string Content { get; set; }

        [Required(ErrorMessage = "لطفاً شناسه شرکت را وارد کنید")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "لطفاً شناسه کاربر را وارد کنید")]
        public int UserId { get; set; }

        public int IsDeleted { get; set; }
    }
}
