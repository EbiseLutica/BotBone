using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Disboard.Misskey.Models
{
	public class PollParameter
    {
        [JsonProperty("choices")]
        public IEnumerable<string> Choices { get; set; }

        [JsonProperty("expiresAt")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime? ExpiresAt { get; set; }

        [JsonProperty("multiple")]
        public bool Multiple { get; set; }
    }
}