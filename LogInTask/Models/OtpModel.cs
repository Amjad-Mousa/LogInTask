using System.ComponentModel.DataAnnotations;

namespace LogInTask.Models
{
    public class OtpModel
    {
        [Required(ErrorMessage = "OTP code is required")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP must be 6 digits")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "OTP must be numeric")]
        public string OtpCode { get; set; } = string.Empty;
    }
}