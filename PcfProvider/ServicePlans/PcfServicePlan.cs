using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PcfProvider.ServicePlans
{
	public class PcfServicePlan
	{
		public bool Active { get; set; }

		public bool Bindable { get; set; }

		public string Description { get; set; }

		public object Extra { get; set; }

		public bool Free { get; set; }

		public string Name { get; set; }

		public bool Public { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public Guid ServiceGuid { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string ServiceInstancesUrl { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string ServiceUrl { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public Guid UniqueId { get; set; }
	}
}