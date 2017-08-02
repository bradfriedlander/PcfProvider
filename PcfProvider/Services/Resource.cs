using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PcfProvider.Services
{
	[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
	public class Resource
	{
		public Metadata Metadata { get; set; }

		[JsonProperty("entity")]
		public PcfServiceInfo ServiceInfo { get; set; }
	}
}