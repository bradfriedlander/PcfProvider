using System;
using Newtonsoft.Json;

namespace PcfProvider.Users
{
	public class Resource
	{
		public Metadata Metadata { get; set; }

		[JsonProperty("entity")]
		public PcfUserInfo UserInfo { get; set; }
	}
}