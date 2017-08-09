using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PcfProvider.ServiceInstance
{
	public class PcfServiceInstance
	{
		public Credentials Credentials { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string DashboardUrl { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public object GatewayData { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public LastOperation LastOperation { get; set; }

		public string Name { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string RoutesUrl { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string ServiceBindingsUrl { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string ServiceGuid { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string ServiceKeysUrl { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string ServicePlanGuid { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string ServicePlanUrl { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string ServiceUrl { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string SpaceGuid { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string SpaceUrl { get; set; }

		public List<object> Tags { get; set; }

		public string Type { get; set; }
	}
}