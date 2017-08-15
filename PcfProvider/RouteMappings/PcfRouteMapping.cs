using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PcfProvider.Apps;
using PcfProvider.Routes;

namespace PcfProvider.RouteMappings
{
	public class PcfRouteMapping : InfoBase.PcfInfo
	{
		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public Guid AppGuid { get; set; }

		[JsonIgnore]
		public PcfAppInfo AppInfo { get; set; }

		[JsonIgnore]
		public string AppName { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public int? AppPort { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string AppUrl { get; set; }

		[JsonIgnore]
		public override string Name { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public Guid RouteGuid { get; set; }

		[JsonIgnore]
		public PcfRouteInfo RouteInfo { get; set; }

		[JsonIgnore]
		public string RouteName { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string RouteUrl { get; set; }
	}
}