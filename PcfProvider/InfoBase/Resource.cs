using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PcfProvider.InfoBase
{
	[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
	public class Resource<T>
	{
		[JsonProperty("entity")]
		public T Info { get; set; }

		public Metadata Metadata { get; set; }
	}
}