using System.ComponentModel.DataAnnotations;

namespace CRM.Domain.Entities
{
    public class LoginModel
    {
        [Required(ErrorMessage = "لطفا نام خود را وارد کنید")]
        [Display(Name = "ورود")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "لطفا رمز عبور خود را وارد کنید")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
