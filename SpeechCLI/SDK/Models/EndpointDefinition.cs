// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace CRIS.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// EndpointDefinition
    /// </summary>
    public partial class EndpointDefinition
    {
        /// <summary>
        /// Initializes a new instance of the EndpointDefinition class.
        /// </summary>
        public EndpointDefinition()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the EndpointDefinition class.
        /// </summary>
        /// <param name="modelsProperty">Information about the deployed
        /// models</param>
        /// <param name="name">The name of the object</param>
        /// <param name="locale">The locale of the contained data</param>
        /// <param name="concurrentRecognitions">The number of concurrent
        /// recognitions the endpoint supports</param>
        /// <param name="contentLoggingEnabled">A value indicating whether
        /// content logging (audio &amp;amp; transcriptions) is being used for
        /// a deployment.
        /// Suppressing content logging will result in a higher cost for the
        /// deployment.
        /// Free subscriptions can only deploy true</param>
        /// <param name="description">The description of the object</param>
        /// <param name="properties">The custom properties of this
        /// entity</param>
        public EndpointDefinition(IList<ModelIdentity> modelsProperty, string name, string locale, int? concurrentRecognitions = default(int?), bool? contentLoggingEnabled = default(bool?), string description = default(string), IDictionary<string, string> properties = default(IDictionary<string, string>))
        {
            ModelsProperty = modelsProperty;
            ConcurrentRecognitions = concurrentRecognitions;
            ContentLoggingEnabled = contentLoggingEnabled;
            Name = name;
            Description = description;
            Properties = properties;
            Locale = locale;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets information about the deployed models
        /// </summary>
        [JsonProperty(PropertyName = "models")]
        public IList<ModelIdentity> ModelsProperty { get; set; }

        /// <summary>
        /// Gets or sets the number of concurrent recognitions the endpoint
        /// supports
        /// </summary>
        [JsonProperty(PropertyName = "concurrentRecognitions")]
        public int? ConcurrentRecognitions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether content logging (audio
        /// &amp;amp;amp; transcriptions) is being used for a deployment.
        /// Suppressing content logging will result in a higher cost for the
        /// deployment.
        /// Free subscriptions can only deploy true
        /// </summary>
        [JsonProperty(PropertyName = "contentLoggingEnabled")]
        public bool? ContentLoggingEnabled { get; set; }

        /// <summary>
        /// Gets or sets the name of the object
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the object
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the custom properties of this entity
        /// </summary>
        [JsonProperty(PropertyName = "properties")]
        public IDictionary<string, string> Properties { get; set; }

        /// <summary>
        /// Gets or sets the locale of the contained data
        /// </summary>
        [JsonProperty(PropertyName = "locale")]
        public string Locale { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (ModelsProperty == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "ModelsProperty");
            }
            if (Name == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Name");
            }
            if (Locale == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Locale");
            }
            if (ModelsProperty != null)
            {
                foreach (var element in ModelsProperty)
                {
                    if (element != null)
                    {
                        element.Validate();
                    }
                }
            }
        }
    }
}
