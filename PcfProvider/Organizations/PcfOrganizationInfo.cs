using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PcfProvider.Organizations
{
	[JsonObject("entity", NamingStrategyType = typeof(CamelCaseNamingStrategy))]
	public class PcfOrganizationInfo : InfoBase.PcfInfo
	{
		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string AppEventsUrl { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string AuditorsUrl { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public bool BillingEnabled { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string BillingManagersUrl { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string DomainsUrl { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string ManagersUrl { get; set; }

		public override string Name { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string PrivateDomainsUrl { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string QuotaDefinitionGuid { get; set; }

		public string QuotaDefinitionUrl { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string SpaceQuotaDefinitionsUrl { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string SpacesUrl { get; set; }

		public string Status { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string UsersUrl { get; set; }
	}
}