using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PcfProvider.LoginInfo
{
	public class PcfLoginInfo
	{
		public App App { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string CommitId { get; set; }

		public string EntityId { get; set; }

		public IdpDefinitions IdpDefinitions { get; set; }

		public Links Links { get; set; }

		public Prompts Prompts { get; set; }

		public string Timestamp { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string Zone_name { get; set; }
	}
}