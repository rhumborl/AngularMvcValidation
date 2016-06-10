using System.Web.Mvc;

namespace Rhumborl.MVC.AngularMvcValidation.UiTest.Validators
{
    public class ModelClientValidationCustomRule : ModelClientValidationRule
    {
        public ModelClientValidationCustomRule(string errorMessage)
        {
            this.ErrorMessage = errorMessage;
            this.ValidationType = "custom";
        }
    }
}