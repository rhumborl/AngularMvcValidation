using System.ComponentModel.DataAnnotations;

namespace Rhumborl.MVC.AngularMvcValidation.UiTest.Models
{
    public class TopLevelModel
    {
        [Required(ErrorMessage = "You must enter an id for this Top Level Model")]
        [Range(0, 100, ErrorMessage = "The {0} value must be between {1} and {2} inclusive")]
        public int ID { get; set; }

        [Required]
        [StringLength(10)]
        public string Name { get; set; }

        public SecondLevelModel Child { get; set; }
    }
}