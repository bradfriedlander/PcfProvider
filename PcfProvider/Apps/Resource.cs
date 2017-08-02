using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PcfProvider.Apps
{
	[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
	public class Resource
	{
		[JsonProperty("entity")]
		public PcfAppInfo AppInfo { get; set; }

		public Metadata Metadata { get; set; }
	}
}