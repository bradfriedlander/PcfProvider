using Newtonsoft.Json;

namespace PcfProvider.ServiceBindings
{
	public class Resource
	{
		public Metadata Metadata { get; set; }

		[JsonProperty("entity")]
		public PcfServiceBinding ServiceBinding { get; set; }
	}
}