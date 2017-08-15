using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PcfProvider.ServiceBindings
{
	public class PcfServiceBinding : InfoBase.PcfInfo
	{
		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public Guid AppGuid { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string AppUrl { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public BindingOptions BindingOptions { get; set; }

		public Credentials Credentials { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string GatewayData { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string GatewayName { get; set; }

		[JsonIgnore]
		public override string Name { get; set; }

		[JsonIgnore()]
		public ServiceInstance.PcfServiceInstance ServiceInstance { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public Guid ServiceInstanceGuid { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string ServiceInstanceUrl { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string SyslogDrainUrl { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public List<object> VolumeMounts { get; set; }
	}
}