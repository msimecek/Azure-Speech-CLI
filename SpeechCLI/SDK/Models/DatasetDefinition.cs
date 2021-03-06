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
    /// DatasetDefinition
    /// </summary>
    public partial class DatasetDefinition
    {
        /// <summary>
        /// Initializes a new instance of the DatasetDefinition class.
        /// </summary>
        public DatasetDefinition()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the DatasetDefinition class.
        /// </summary>
        /// <param name="dataImportKind">The kind of the dataset (e.g. acoustic
        /// data, language data ...). Possible values include: 'Language',
        /// 'Acoustic', 'Pronunciation', 'CustomVoice',
        /// 'LanguageGeneration'</param>
        /// <param name="name">The name of the object</param>
        /// <param name="locale">The locale of the contained data</param>
        /// <param name="description">The description of the object</param>
        /// <param name="properties">The custom properties of this
        /// entity</param>
        public DatasetDefinition(string dataImportKind, string name, string locale, string description = default(string), IDictionary<string, string> properties = default(IDictionary<string, string>))
        {
            DataImportKind = dataImportKind;
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
        /// Gets or sets the kind of the dataset (e.g. acoustic data, language
        /// data ...). Possible values include: 'Language', 'Acoustic',
        /// 'Pronunciation', 'CustomVoice', 'LanguageGeneration'
        /// </summary>
        [JsonProperty(PropertyName = "dataImportKind")]
        public string DataImportKind { get; set; }

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
            if (DataImportKind == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "DataImportKind");
            }
            if (Name == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Name");
            }
            if (Locale == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Locale");
            }
        }
    }
}
