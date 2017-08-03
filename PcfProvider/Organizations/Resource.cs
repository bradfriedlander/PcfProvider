using Newtonsoft.Json;

namespace PcfProvider.Organizations
{
	public class Resource
	{
		public Metadata Metadata { get; set; }

		[JsonProperty("entity")]
		public PcfOrganization Organization { get; set; }
	}
}