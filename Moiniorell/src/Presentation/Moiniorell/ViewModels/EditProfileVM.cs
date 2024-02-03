using Moiniorell.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Moiniorell.ViewModels
{
    public class EditProfileVM
    {
        [Display(Name = "Profile Picture")]
        public IFormFile? ProfilePictureFile { get; set; }

        public string ProfilePicture { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "User Name is required.")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; init; }
        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }
        public string? ConfirmPassword { get; set; }


        [Required(ErrorMessage = "Gender is required.")]
        public Gender Gender { get; set; }

        [Required(ErrorMessage = "Date Of Birth is required.")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public string? Address { get; set; }
    }
}
