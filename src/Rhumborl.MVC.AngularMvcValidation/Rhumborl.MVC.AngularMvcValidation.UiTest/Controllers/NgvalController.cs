using System;
using System.Web.Mvc;

namespace Rhumborl.MVC.AngularMvcValidation.UiTest.Controllers
{
    public class NgvalController : Controller
    {
        [HttpGet]
        public ActionResult TypeMetadata(string typeName)
        {
            Type modelType = Type.GetType(typeName);

            ValidatorMetadataGenerator2 gen = new ValidatorMetadataGenerator2();
            string json = gen.GenerateJson(modelType, null, this.ControllerContext);

            return this.Content(json);
        }
    }
}