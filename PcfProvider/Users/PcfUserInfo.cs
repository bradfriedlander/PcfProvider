using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PcfProvider.Users
{
	public class PcfUserInfo : InfoBase.PcfInfo
	{
		public bool Active { get; set; }

		public bool Admin { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string AuditedOrganizationsUrl { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string AuditedSpacesUrl { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string BillingManagedOrganizationsUrl { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public object DefaultSpaceGuid { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string ManagedOrganizationsUrl { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string ManagedSpacesUrl { get; set; }

		[JsonProperty("username")]
		public override string Name { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string OrganizationsUrl { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string SpacesUrl { get; set; }
	}
}