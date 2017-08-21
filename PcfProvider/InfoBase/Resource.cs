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
		///     This method sets the instance identifier and copies other metadata values.
		/// </summary>
		/// <remarks>
		///     <para><see cref="PcfInfo.InstanceId" /> is set to <see cref="Metadata.Guid" />.</para>
		///     <para>
		///         The copied values are: <see cref="Metadata.CreatedAt" />, <see cref="Metadata.UpdatedAt" />, and <see cref="Metadata.Url" />.
		///     </para>
		/// </remarks>
		public void SetInstanceDataFromMetadata()
		{
			Info.InstanceId = Metadata.Guid;
			Info.CreatedAt = Metadata.CreatedAt;
			Info.UpdatedAt = Metadata.UpdatedAt;
			Info.Url = Metadata.Url;
		}
	}
}