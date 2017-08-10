using System;
using Newtonsoft.Json;

namespace PcfProvider.InfoBase
{
	public class PcfInfo
	{
		[JsonIgnore]
		public Guid InstanceId { get; set; }
	}
}