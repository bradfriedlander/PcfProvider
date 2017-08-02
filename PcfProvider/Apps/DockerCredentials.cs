using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PcfProvider.Apps
{
	[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
	public class DockerCredentials
	{
		public object Password { get; set; }

		public object Username { get; set; }
	}
}