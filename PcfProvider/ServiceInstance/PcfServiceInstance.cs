using System.Collections.Generic;

namespace PcfProvider.ServiceInstance
{
	public class PcfServiceInstance
	{
		public Credentials credentials { get; set; }

		public string dashboard_url { get; set; }

		public object gateway_data { get; set; }

		public LastOperation last_operation { get; set; }

		public string name { get; set; }

		public string routes_url { get; set; }

		public string service_bindings_url { get; set; }

		public string service_guid { get; set; }

		public string service_keys_url { get; set; }

		public string service_plan_guid { get; set; }

		public string service_plan_url { get; set; }

		public string service_url { get; set; }

		public string space_guid { get; set; }

		public string space_url { get; set; }

		public List<object> tags { get; set; }

		public string type { get; set; }
	}
}