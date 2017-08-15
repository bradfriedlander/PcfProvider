using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PcfProvider.InfoBase
{
	/// <summary>
	///     This base class is the shard root object used for all PCF responses that return an array of resources.
	/// </summary>
	/// <typeparam name="T">This is the type of the info returned in the response.</typeparam>
	[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
	public class RootObject<T> where T : PcfInfo
	{
		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string NextUrl { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string PrevUrl { get; set; }

		public List<Resource<T>> Resources { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public int TotalPages { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public int TotalResults { get; set; }

		[JsonIgnore]
		public DateTime UtcExpiration { get; set; }
	}
}