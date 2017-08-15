using System;
using Newtonsoft.Json;

namespace PcfProvider.InfoBase
{
	public abstract class PcfInfo
	{
		/// <summary>
		///     This get or sets the identifier for this instance of the inheriting object.
		/// </summary>
		/// <value>This is the instance identifier.</value>
		[JsonIgnore]
		public Guid InstanceId { get; set; }

		/// <summary>
		///     This gets or sets the name for this instance of the inheriting object.
		/// </summary>
		/// <value>This is the name.</value>
		/// <remarks>
		///     <para>This always has to be overridden by the inheriting class.</para>
		/// </remarks>
		public abstract string Name { get; set; }
	}
}