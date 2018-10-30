using System;
using System.ComponentModel.DataAnnotations;

namespace CustomSpeechCLI.Attributes
{
    public class GuidAttribute : ValidationAttribute
    {
        public GuidAttribute() : base("The value for {0} must be a valid GUID.") { }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value.GetType() != typeof(string) || !Guid.TryParse((string)value, out Guid res))
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));

            return ValidationResult.Success;
        }
    }
}
