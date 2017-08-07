using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PcfProvider.Users
{
	public class Metadata
	{
		public string Guid { get; set; }
		public string Url { get; set; }
		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string CreatedAt { get; set; }
		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string UpdatedAt { get; set; }
	}
}
