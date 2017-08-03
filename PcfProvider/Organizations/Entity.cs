namespace PcfProvider.Organizations
{
	public class Entity
	{
		public string name { get; set; }
		public bool billing_enabled { get; set; }
		public string quota_definition_guid { get; set; }
		public string status { get; set; }
		public string quota_definition_url { get; set; }
		public string spaces_url { get; set; }
		public string domains_url { get; set; }
		public string private_domains_url { get; set; }
		public string users_url { get; set; }
		public string managers_url { get; set; }
		public string billing_managers_url { get; set; }
		public string auditors_url { get; set; }
		public string app_events_url { get; set; }
		public string space_quota_definitions_url { get; set; }
	}
}
