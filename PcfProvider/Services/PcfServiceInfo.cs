using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PcfProvider.Services
{
	[JsonObject("entity", NamingStrategyType = typeof(CamelCaseNamingStrategy))]
	public class PcfServiceInfo : InfoBase.PcfInfo
	{
		public PcfServiceInfo()
		{
			Plans = new List<ServicePlans.PcfServicePlan>();
		}

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
		public override string Name { get; set; }

		[JsonIgnore()]
		public List<ServicePlans.PcfServicePlan> Plans { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public bool PlanUpdateable { get; set; }

		public object Provider { get; set; }

		public List<object> Requires { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string ServiceBrokerGuid { get; set; }

		[JsonIgnore()]
		public Guid ServiceGuid { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string ServicePlansUrl { get; set; }

		public List<object> Tags { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string UniqueId { get; set; }

		public object Url { get; set; }

		public object Version { get; set; }
	}
}