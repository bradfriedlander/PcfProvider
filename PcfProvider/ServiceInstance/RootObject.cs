using System;
using Newtonsoft.Json;

namespace PcfProvider.ServiceInstance
{
	public class RootObject
	{
		public Metadata metadata { get; set; }

		[JsonProperty("entity")]
		public PcfServiceInstance ServiceInstance { get; set; }
	}
}