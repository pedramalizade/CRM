using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace CRM.Models
{

    public class User
    {
        public int Id { get; set; }

        [StringLength(60, MinimumLength = 2)]
        [Required(ErrorMessage = "لطفاً نام را وارد کنید")]
        public string Name { get; set; }

        [StringLength(60, MinimumLength = 2)]
        [Required(ErrorMessage = "لطفاً نام خانوادگی را وارد کنید")]
        public string Surname { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "لطفاً تاریخ تولد را وارد کنید")]
        public DateTime DateOfBirth { get; set; }

        [StringLength(60, MinimumLength = 3)]
        [Required(ErrorMessage = "لطفاً نام کاربری را وارد کنید")]
        public string Login { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "لطفاً رمز عبور را وارد کنید")]
        public string Password { get; set; }

        [Range(1, 3)]
        [Required(ErrorMessage = "لطفاً شناسه نقش را وارد کنید")]
        public int RoleId { get; set; }

        public int IsDeleted { get; set; }

    }
    //public class CustomDateRangeAttribute : RangeAttribute
    //{
    //    public CustomDateRangeAttribute() : base(typeof(DateTime), DateTime.Now.ToString(), DateTime.Now.AddYears(20).ToString())
    //    { }
    //}
    //public class DateAttribute : RangeAttribute
    //{
    //    public DateAttribute()
    //      : base(typeof(DateTime), DateTime.Now.AddYears(-20).ToShortDateString(), DateTime.Now.AddYears(2).ToShortDateString()) { }
    //}

}