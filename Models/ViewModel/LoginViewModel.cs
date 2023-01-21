using System.ComponentModel.DataAnnotations;

namespace SobhanJuice.Models.ViewModel
{
    public class LoginViewModel
    {
   
        [Display(Name ="شماره موبایل")]
        [Required(ErrorMessage = "لطفا شماره همراه خود را وارد کنید")]
        [MinLength(11, ErrorMessage = "موبایل نامعتبر")]
        public string number { get; set; }
    }
}
