using System.Collections.Generic;
using System.Web.Mvc;

namespace Rhumborl.MVC.AngularMvcValidation.UiTest.Validators
{
    public class MyCustomValidatorAdapter : DataAnnotationsModelValidator<MyCustomValidatorAttribute>
    {
        public MyCustomValidatorAdapter(ModelMetadata metadata, ControllerContext context, MyCustomValidatorAttribute attribute)
            : base(metadata, context, attribute)
        {
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
        {
            return new ModelClientValidationRule[] {
                new ModelClientValidationCustomRule(this.Attribute.ErrorMessage)
            };
        }
    }
}