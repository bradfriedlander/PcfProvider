using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PcfProvider
{
	/// <summary>
	///     This class provides the information and methods needed to support the JSON token returned by PCF.
	/// </summary>
	public class OAuthResponse
	{
		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string AccessToken { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public int ExpiresIn { get; set; }

		public string Jti { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string RefreshToken { get; set; }

		public string Scope { get; set; }

		[JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		public string TokenType { get; set; }
	}
}