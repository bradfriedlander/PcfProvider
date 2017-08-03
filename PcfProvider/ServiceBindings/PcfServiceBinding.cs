using System.Collections.Generic;
using Newtonsoft.Json;

namespace PcfProvider.ServiceBindings
{
	public class PcfServiceBinding
	{
		public string app_guid { get; set; }

		public string app_url { get; set; }

		public BindingOptions binding_options { get; set; }

		public Credentials credentials { get; set; }

		public object gateway_data { get; set; }

		public string gateway_name { get; set; }

		public string service_instance_guid { get; set; }

		public string service_instance_url { get; set; }

		[JsonIgnore()]
		public ServiceInstance.PcfServiceInstance ServiceInstance { get; set; }

		public object syslog_drain_url { get; set; }

		public List<object> volume_mounts { get; set; }
	}
}