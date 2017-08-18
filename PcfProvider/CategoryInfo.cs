using System;
using System.Collections.Generic;

namespace PcfProvider
{
	public class CategoryInfo
	{
		public Func<IEnumerable<InfoBase.PcfInfo>> Function { get; set; }

		public Type InfoType { get; set; }

		public bool IsContainer { get; set; }

		public Type RootType { get; set; }
	}
}