using System;
using Newtonsoft.Json;

namespace PcfProvider.InfoBase
{
	public class RootObjectSimple<T>
	{
		[JsonIgnore]
		public DateTime UtcExpiration { get; set; }
	}
}