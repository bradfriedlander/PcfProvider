﻿using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PcfProvider.Apps
{
	[JsonObject("entity", NamingStrategyType = typeof(CamelCaseNamingStrategy))]
	public class PcfAppInfo
	{
		public PcfAppInfo()
		{
			ServiceBindings = new List<ServiceBindings.PcfServiceBinding>();
		}

		[JsonIgnore()]
		public string AppGuid { get; set; }

		public string Buildpack { get; set; }

		public object Command { get; set; }

		public bool Console { get; set; }

		public object Debug { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string DetectedBuildpack { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string DetectedBuildpackGuid { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string DetectedStartCommand { get; set; }

		public bool Diego { get; set; }

		[JsonProperty("disk_quota")]
		public int DiskKb { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public DockerCredentials DockerCredentials { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public object DockerImage { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public bool EnableSsh { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public EnvironmentJson EnvironmentJson { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string EventsUrl { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public object HealthCheckHttpEndpoint { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public object HealthCheckTimeout { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string HealthCheckType { get; set; }

		public int Instances { get; set; }

		[JsonProperty("memory")]
		public int MemoryKb { get; set; }

		public string Name { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string PackageState { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string PackageUpdatedAt { get; set; }

		public List<int> ports { get; set; }

		public bool Production { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string routeMappingsUrl { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string RoutesUrl { get; set; }

		[JsonIgnore()]
		public List<ServiceBindings.PcfServiceBinding> ServiceBindings { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string ServiceBindingsUrl { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string SpaceGuid { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string SpaceUrl { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string StackGuid { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string StackUrl { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public object StagingFailedDescription { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public object StagingFailedReason { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string StagingTaskId { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string State { get; set; }

		public string Version { get; set; }
	}
}