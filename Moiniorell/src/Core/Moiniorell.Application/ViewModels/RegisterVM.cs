using Moiniorell.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moiniorell.Application.ViewModels
{
    public record RegisterVM
    {
        [Required(ErrorMessage = "Username is required")]
        [MinLength(3, ErrorMessage = "Username must be at least 3 characters long")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; init; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
        public string Password { get; init; }
        [Required(ErrorMessage = "Confirming Password is required")]

        [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; init; }

        [Required(ErrorMessage = "Name is required")]
        [MinLength(3, ErrorMessage = "Name must be at least 3 characters long")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Name can only contain alphabetic characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Surname is required")]
        [MinLength(3, ErrorMessage = "Surname must be at least 3 characters long")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Name can only contain alphabetic characters")]
        public string Surname { get; set; }
        [Required(ErrorMessage = "Gender is required")]
        public Gender Gender { get; init; }

        [Required(ErrorMessage = "Birthdate is required")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format")]

        public DateTime BirthDate { get; init; }
    }


}
