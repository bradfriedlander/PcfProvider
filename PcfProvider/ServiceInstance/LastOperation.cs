using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PcfProvider.ServiceInstance
{
	public class LastOperation
	{
		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string CreatedAt { get; set; }

		public string Description { get; set; }

		public string State { get; set; }

		public string Type { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string UpdatedAt { get; set; }
	}
}