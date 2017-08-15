using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PcfProvider.InfoBase
{
	[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
	public class Resource<T> where T : PcfInfo
	{
		[JsonProperty("entity")]
		public T Info { get; set; }

		/// <summary>
		///     Gets or sets the metadata.
		/// </summary>
		/// <value>This is the metadata for <see cref="Info" />.</value>
		public Metadata Metadata { get; set; }

		/// <summary>
		///     This method sets the instance identifier.
		/// </summary>
		/// <remarks>
		///     <para><see cref="PcfInfo.InstanceId" /> is set to <see cref="Metadata.Guid" />.</para>
		/// </remarks>
		public void SetInstanceId()
		{
			Info.InstanceId = Metadata.Guid;
		}
	}
}