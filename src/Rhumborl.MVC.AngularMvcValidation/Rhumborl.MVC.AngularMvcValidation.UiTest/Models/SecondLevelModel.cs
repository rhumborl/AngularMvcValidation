using System.ComponentModel.DataAnnotations;
using Rhumborl.MVC.AngularMvcValidation.UiTest.Validators;

namespace Rhumborl.MVC.AngularMvcValidation.UiTest.Models
{
    public class SecondLevelModel
    {
        public int ID { get; set; }

        [Required]
        [RegularExpression("Value")]
        public string Value { get; set; }

        [MyCustomValidator(ErrorMessage = "Not test")]
        public string CustomProp { get; set; }
    }
}