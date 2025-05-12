using System;
using System.ComponentModel.DataAnnotations;

namespace CRM.Models
{
    public class Contact
    {
        public int Id { get; set; }

        [StringLength(60, MinimumLength = 2)]
        [Required(ErrorMessage = "لطفاً نام را وارد کنید")]
        public string Name { get; set; }

        [StringLength(60, MinimumLength = 2)]
        [Required(ErrorMessage = "لطفاً نام خانوادگی را وارد کنید")]
        public string Surname { get; set; }

        [RegularExpression(@"^(\d{11})$",ErrorMessage = "لطفاً شماره تلفن معتبر وارد کنید")]
        [Required(ErrorMessage = "لطفاً شماره تلفن را وارد کنید")]
        public string Phone { get; set; }

        [RegularExpression(@"^(([^<>()[\]\\.,;:\s@""]+(\.[^<>()[\]\\.,;:\s@""]+)*)|("".+""))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$",
            ErrorMessage = "لطفاً ایمیل معتبر وارد کنید")]
        [Required(ErrorMessage = "لطفاً ایمیل را وارد کنید")]
        public string Email { get; set; }

        [StringLength(60, MinimumLength = 2)]
        [Required(ErrorMessage = "لطفاً سمت شغلی را وارد کنید")]
        public string Position { get; set; }

        [Required(ErrorMessage = "لطفاً شناسه شرکت را وارد کنید")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "لطفاً شناسه کاربر را وارد کنید")]
        public int UserId { get; set; }

        public int IsDeleted { get; set; }
    }
}
