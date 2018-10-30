using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CRIS.Models
{
    public class Entity
    {
        /// <summary>
        /// Gets or sets the identifier of this entity
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public System.Guid Id { get; set; }

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
        /// Gets or sets the time-stamp when the object was created
        /// </summary>
        [JsonProperty(PropertyName = "createdDateTime")]
        public System.DateTime CreatedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the time-stamp when the current status was entered
        /// </summary>
        [JsonProperty(PropertyName = "lastActionDateTime")]
        public System.DateTime LastActionDateTime { get; set; }

        /// <summary>
        /// Gets or sets the status of the object. Possible values include:
        /// 'NotStarted', 'Running', 'Succeeded', 'Failed'
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
    }
}
