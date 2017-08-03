using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcfProvider.Organizations
{

	public class RootObject
	{
		public int total_results { get; set; }
		public int total_pages { get; set; }
		public object prev_url { get; set; }
		public object next_url { get; set; }
		public List<Resource> resources { get; set; }
	}
}
