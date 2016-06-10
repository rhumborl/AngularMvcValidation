using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Rhumborl.MVC.AngularMvcValidation.UiTest
{
    public class ValidatorMetadataGenerator
    {
        public string GenerateJson(Type modelType, object instance, ControllerContext controllerContext)
        {
            //ModelMetadataProviders.Current.GetMetadataForType()

            // get the metadata
            ModelMetadata modelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => instance, modelType);
            //ModelMetadataProvider metaDataProvider = new CachedDataAnnotationsModelMetadataProvider();
            //ModelMetadata modelMetadata = metaDataProvider.GetMetadataForType(() => null, modelType);

            // create a context
            FormContext formContext = new FormContext();


            // populate context with validation rules
            GenerateRecursive(modelMetadata, formContext, controllerContext);

            // get string representation of metadata (may need to do this another way)
            string validationJson = this.SerializeToJson(formContext);
            return validationJson;
        }

        private void GenerateRecursive(ModelMetadata modelMetadata, FormContext formContext, ControllerContext controllerContext, string prefix = null)
        {
            prefix = (prefix == null) ? modelMetadata.PropertyName : (prefix + "." + modelMetadata.PropertyName);
            if (modelMetadata.PropertyName != null)
            {
                string qualifiedPropertyName = prefix;
                var formMetadata = formContext.GetValidationMetadataForField(qualifiedPropertyName, true);
                IEnumerable<ModelValidator> validators = ModelValidatorProviders.Providers.GetValidators(modelMetadata, controllerContext);
                foreach (ModelClientValidationRule rule in validators.SelectMany(v => v.GetClientValidationRules()))
                {
                    formMetadata.ValidationRules.Add(rule);
                }
            }


            // populate context with validation rules
            foreach (var propertyMetadata in modelMetadata.Properties)
            {
                GenerateRecursive(propertyMetadata, formContext, controllerContext, prefix);
            }
        }

        private string SerializeToJson(FormContext formContext)
        {
            return formContext.GetJsonValidationMetadata();
        }
    }
}