using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PcfProvider.ServiceInstance
{
	[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
	public class Metadata
	{
		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string CreatedAt { get; set; }

		public string Guid { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string UpdatedAt { get; set; }

		public string Url { get; set; }
	}
}