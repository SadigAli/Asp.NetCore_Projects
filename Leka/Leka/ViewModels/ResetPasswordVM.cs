using System.ComponentModel.DataAnnotations;

namespace Leka.ViewModels
{
    public class ResetPasswordVM
    {
        [Required, StringLength(50)]
        public string Fullname { get; set; }
        [Required, StringLength(50)]
        public string Username { get; set; }
        [Required, StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }
        [Required, StringLength(50)]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }
        [StringLength(50)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [StringLength(50)]
        [DataType(DataType.Password),Compare(nameof(NewPassword))]
        public string ConfirmNewPassword { get; set; }    }
}