using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SpeechCLI.Attributes
{
    public class EnumAttribute : ValidationAttribute
    {
        private string[] _expected;

        public EnumAttribute(params string[] expected) : base("Value for {0} is not one of the expected.")
        {
            _expected = expected;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value.GetType() != typeof(string) || !Array.Exists(_expected, p => p == value.ToString()))
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }

            return ValidationResult.Success;
        }
    }
}
