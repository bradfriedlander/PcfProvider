using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PcfProvider.Services
{
	[JsonObject("entity", NamingStrategyType = typeof(CamelCaseNamingStrategy))]
	public class PcfServiceInfo
	{
		public bool Active { get; set; }

		public bool Bindable { get; set; }

		public string Description { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public object DocumentationUrl { get; set; }

		public string Extra { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public object InfoUrl { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public object LongDescription { get; set; }

		[JsonProperty("label")]
		public string Name { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public bool planUpdateable { get; set; }

		public object Provider { get; set; }

		public List<object> Requires { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string ServiceBrokerGuid { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string ServicePlansUrl { get; set; }

		public List<object> Tags { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string UniqueId { get; set; }

		public object Url { get; set; }

		public object Version { get; set; }
	}
}