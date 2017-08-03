using System;
using System.Collections.Generic;

namespace PcfProvider.ServiceBindings
{
	public class RootObject
	{
		public object next_url { get; set; }

		public object prev_url { get; set; }

		public List<Resource> resources { get; set; }

		public int total_pages { get; set; }

		public int total_results { get; set; }
	}
}