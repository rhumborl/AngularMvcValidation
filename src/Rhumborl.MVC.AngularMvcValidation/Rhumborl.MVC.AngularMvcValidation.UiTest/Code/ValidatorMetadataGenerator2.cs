using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Rhumborl.MVC.AngularMvcValidation.UiTest
{
    public class ValidatorMetadataGenerator2
    {
        public string GenerateJson(Type modelType, object instance, ControllerContext controllerContext)
        {
            //ModelMetadataProviders.Current.GetMetadataForType()

            // get the metadata
            ModelMetadata modelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => instance, modelType);
            //ModelMetadataProvider metaDataProvider = new CachedDataAnnotationsModelMetadataProvider();
            //ModelMetadata modelMetadata = metaDataProvider.GetMetadataForType(() => null, modelType);

            // create a context
            List<FieldValidationData> formContext = new List<FieldValidationData>();


            // populate context with validation rules
            GenerateRecursive(modelMetadata, formContext, controllerContext);

            // get string representation of metadata (may need to do this another way)
            string validationJson = this.SerializeToJson(formContext);
            return validationJson;
        }

        private void GenerateRecursive(ModelMetadata modelMetadata, List<FieldValidationData> formContext, ControllerContext controllerContext, string prefix = null)
        {
            if (modelMetadata.PropertyName != null)
            {
                IEnumerable<ModelValidator> validators = ModelValidatorProviders.Providers.GetValidators(modelMetadata, controllerContext);

                FieldValidationData data = new FieldValidationData(modelMetadata.ModelType, modelMetadata.PropertyName, prefix);

                foreach (ModelClientValidationRule rule in validators.SelectMany(v => v.GetClientValidationRules()))
                {
                    data.ValidationRules.Add(rule);
                }

                formContext.Add(data);

                prefix = data.FieldName;
            }


            // populate context with validation rules
            foreach (var propertyMetadata in modelMetadata.Properties)
            {
                GenerateRecursive(propertyMetadata, formContext, controllerContext, prefix);
            }
        }

        private string SerializeToJson(List<FieldValidationData> formContext)
        {
            //return formContext.GetJsonValidationMetadata();
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();

            SortedDictionary<string, object> contextData = new SortedDictionary<string, object>()
            {
                {
                    "Fields",
                    formContext
                }
            };

            return javaScriptSerializer.Serialize(contextData);
        }

        private class FieldValidationData
        {
            private string interpolatedParentFieldName;

            public string TypeName { get; set; }

            public string FieldName
            {
                get
                {
                    return this.interpolatedParentFieldName + this.PropertyName;
                }
            }

            public string PropertyName { get; private set; }

            public string ParentFieldName { get; private set; }

            public ICollection<ModelClientValidationRule> ValidationRules { get; }

            private FieldValidationData()
            {
                this.ValidationRules = new List<ModelClientValidationRule>();
            }

            public FieldValidationData(Type fieldType, string propertyName, string fieldPrefix)
                : this()
            {
                this.TypeName = fieldType.FullName;
                this.PropertyName = propertyName;
                this.ParentFieldName = fieldPrefix;
                this.interpolatedParentFieldName = this.ParentFieldName == null ? "" : this.ParentFieldName + ".";
            }
        }
    }
}