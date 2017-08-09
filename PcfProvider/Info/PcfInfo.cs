using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PcfProvider.Info
{
	public class PcfInfo
	{
		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string ApiVersion { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string AppSshEndpoint { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string AppSshHostKeyFingerprint { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string AppSshOauthClient { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string AuthorizationEndpoint { get; set; }

		public string Build { get; set; }

		public string Description { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string DopplerLoggingEndpoint { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string LoggingEndpoint { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public object MinCliVersion { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public object MinRecommendedCliVersion { get; set; }

		public string Name { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string RoutingEndpoint { get; set; }

		public string Support { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string TokenEndpoint { get; set; }

		public int Version { get; set; }
	}
}