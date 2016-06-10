using System.ComponentModel.DataAnnotations;

namespace Rhumborl.MVC.AngularMvcValidation.UiTest.Validators
{
    public class MyCustomValidatorAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return base.IsValid(value, validationContext);
        }
        public override bool IsValid(object value)
        {
            return value != null && (value.ToString().ToLower() == "test");
        }
    }
}