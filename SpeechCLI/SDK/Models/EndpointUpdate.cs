// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace CRIS.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// EndpointUpdate
    /// </summary>
    public partial class EndpointUpdate
    {
        /// <summary>
        /// Initializes a new instance of the EndpointUpdate class.
        /// </summary>
        public EndpointUpdate()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the EndpointUpdate class.
        /// </summary>
        /// <param name="name">The name of the object</param>
        /// <param name="description">The description of the object</param>
        public EndpointUpdate(string name, string description = default(string))
        {
            Name = name;
            Description = description;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

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
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Name == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Name");
            }
        }
    }
}
