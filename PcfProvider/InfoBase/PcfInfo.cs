using System;
using Newtonsoft.Json;

namespace PcfProvider.InfoBase
{
	public abstract class PcfInfo
	{
		/// <summary>
		///     Gets or sets the creation date/time for the instance ( <see langword="false" /><see cref="Metadata" />).
		/// </summary>
		/// <value>The creation date/time.</value>
		public DateTime CreatedAt { get; set; }

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

		/// <summary>
		///     Gets or sets the updated date/time for the instance ( <see langword="false" /><see cref="Metadata" />).
		/// </summary>
		/// <value>The updated date/time.</value>
		public DateTime? UpdatedAt { get; set; }

		/// <summary>
		///     Gets or sets the URL for the instance ( <see langword="false" /><see cref="Metadata" />).
		/// </summary>
		/// <value>The URL for the instance.</value>
		public string Url { get; set; }
	}
}