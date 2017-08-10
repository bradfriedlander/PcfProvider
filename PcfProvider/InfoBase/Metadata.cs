using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PcfProvider.InfoBase
{
	[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
	public class Metadata
	{
		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public DateTime CreatedAt { get; set; }

		public Guid Guid { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public DateTime? UpdatedAt { get; set; }

		public string Url { get; set; }
	}
}