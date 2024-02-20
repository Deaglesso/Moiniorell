using Microsoft.AspNetCore.Http;
using Moiniorell.Application.ViewModels.ValidateAttributes;
using Moiniorell.Domain.Enums;
using System.ComponentModel.DataAnnotations;


namespace Moiniorell.Application.ViewModels
{
    public class EditProfileVM
    {
        [Display(Name = "Profile Picture")]
        public IFormFile? ProfilePictureFile { get; set; }

        public string? ProfilePicture { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        [MinLength(3, ErrorMessage = "Name must be at least 3 characters long")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Name can only contain alphabetic characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        [MinLength(3, ErrorMessage = "Name must be at least 3 characters long")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Name can only contain alphabetic characters")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [MinLength(3, ErrorMessage = "Username must be at least 3 characters long")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; init; }
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]

        public string? CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]

        public string? NewPassword { get; set; }
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
        [Compare(nameof(NewPassword))]

        public string? ConfirmPassword { get; set; }


        [Required(ErrorMessage = "Gender is required.")]
        public Gender Gender { get; set; }

        [Required(ErrorMessage = "Date Of Birth is required.")]
        [DataType(DataType.Date)]
        [MinimumAge(18, ErrorMessage = "Must be at least 18 years old.")]
        public DateTime BirthDate { get; set; }

        
        public string? Address { get; set; }
        public string? Biography { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? PhoneNumber { get; set; }
        public bool IsPrivate { get; set; }
        public bool Availability { get; set; }

    }
}
