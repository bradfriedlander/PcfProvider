using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PcfProvider.Apps
{
	[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
	public class RootObject : InfoBase.RootObject<PcfAppInfo>
	{
	}
}