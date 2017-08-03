using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PcfProvider.ServiceBindings
{
	[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
	public class RootObject
	{
		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public object NextUrl { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public object PrevUrl { get; set; }

		public List<Resource> Resources { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public int TotalPages { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public int TotalResults { get; set; }
	}
}