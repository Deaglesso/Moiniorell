using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moiniorell.Application.ViewModels.ValidateAttributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class MinimumAgeAttribute : ValidationAttribute
    {
        private readonly int _minimumAge;

        public MinimumAgeAttribute(int minimumAge)
        {
            _minimumAge = minimumAge;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime birthDate)
            {
                var age = DateTime.Now.Year - birthDate.Year;

                // Adjust age if birthday hasn't occurred yet this year
                if (DateTime.Now.Month < birthDate.Month || (DateTime.Now.Month == birthDate.Month && DateTime.Now.Day < birthDate.Day))
                {
                    age--;
                }

                if (age < _minimumAge)
                {
                    return new ValidationResult($"Must be at least {_minimumAge} years old.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
