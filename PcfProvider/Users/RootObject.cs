using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PcfProvider.Users
{
	[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
	public class RootObject : InfoBase.RootObject<PcfUserInfo>
	{
	}
}