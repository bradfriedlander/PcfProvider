using System;
using Newtonsoft.Json;

namespace PcfProvider.InfoBase
{
	public abstract class PcfInfo
	{
		[JsonIgnore]
		public Guid InstanceId { get; set; }

		public abstract string Name { get; set; }
	}
}