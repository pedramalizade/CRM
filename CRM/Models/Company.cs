using System;
using System.ComponentModel.DataAnnotations;

namespace CRM.Models
{
    public class Company
    {
        public int Id { get; set; }

        [StringLength(60, MinimumLength = 2)]
        [Required(ErrorMessage = "لطفاً نام را وارد کنید")]
        public string Name { get; set; }

        [Required(ErrorMessage = "لطفاً کد اقتصادی (NIP) را وارد کنید")]
        [RegularExpression(@"^(\d{10})$", ErrorMessage = "کد اقتصادی باید دقیقاً ۱۰ رقم باشد")]
        public string NIP { get; set; }

        [Required(ErrorMessage = "لطفاً شناسه کسب‌وکار را وارد کنید")]
        public int BusinessId { get; set; }

        [StringLength(200, MinimumLength = 3)]
        [Required(ErrorMessage = "لطفاً آدرس را وارد کنید")]
        public string Address { get; set; }

        [StringLength(200, MinimumLength = 3)]
        [Required(ErrorMessage = "لطفاً شهر را وارد کنید")]
        public string City { get; set; }

        [Required(ErrorMessage = "لطفاً شناسه کاربر را وارد کنید")]
        public int UserId { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreationDate { get; set; }

        public int IsDeleted { get; set; }
    }
}
