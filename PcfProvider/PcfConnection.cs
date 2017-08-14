using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json;
using PcfAppInfo = PcfProvider.Apps.PcfAppInfo;
using PcfDomainInfo = PcfProvider.Domains.PcfDomainInfo;
using PcfManagerInfo = PcfProvider.Managers.PcfManagerInfo;
using PcfOrganizationInfo = PcfProvider.Organizations.PcfOrganizationInfo;
using PcfRouteInfo = PcfProvider.Routes.PcfRouteInfo;
using PcfServiceBinding = PcfProvider.ServiceBindings.PcfServiceBinding;
using PcfServiceInfo = PcfProvider.Services.PcfServiceInfo;
using PcfServicePlan = PcfProvider.ServicePlans.PcfServicePlan;
using PcfUserInfo = PcfProvider.Users.PcfUserInfo;

namespace PcfProvider
{
	/// <summary>
	///     This class provides the information and methods used to communicate with the PCF site.
	/// </summary>
	public class PcfConnection
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="PcfConnection" /> class.
		/// </summary>
		/// <param name="driveInfo">This is the drive information as provided by PowerShell.</param>
		/// <param name="uri">This is the URI of the PCF site.</param>
		/// <param name="isLocal">If set to <c>true</c>, the drive is on the local (PcfDev) <inheritdoc /> of PCF.</param>
		/// <param name="tracer">This is the class supporting diagnostic tracing.</param>
		public PcfConnection(PSDriveInfo driveInfo, string uri, bool isLocal, TraceTest tracer)
		{
			Uri = uri;
			Credential = driveInfo.Credential;
			ExpirationTime = DateTime.UtcNow;
			IsLocal = isLocal;
			Tracer = tracer;
		}

		public string AccessToken { get; private set; }

		public PSCredential Credential { get; }

		public DateTime ExpirationTime { get; private set; }

		public bool IsLocal { get; }

		public string RefreshToken { get; private set; }

		public TraceTest Tracer { get; }

		public string Uri { get; }

		public static Dictionary<Type, object> CachedRootObjects = new Dictionary<Type, object>();

		public static Dictionary<Type, int> RootObjectLifetimeInSeconds = new Dictionary<Type, int>()
		{
			[typeof(Apps.RootObject)] = 30,
			[typeof(Organizations.RootObject)] = 600,
			[typeof(Routes.RootObject)] = 30,
			[typeof(Services.RootObject)] = 600,
			[typeof(Info.PcfInfo)] = 3600,
			[typeof(LoginInfo.PcfLoginInfo)] = 3600
		};

		public List<PcfAppInfo> GetAllApps(string container)
		{
			var allApps = GetFromCache<Apps.RootObject, PcfAppInfo>(
				() =>
				{
					var appsRoot = GetAllInfo<PcfAppInfo, Apps.RootObject>(container);
					appsRoot.Resources.ForEach(r => r.Info.InstanceId = r.Metadata.Guid);
					GetAllServiceBindings(appsRoot);
					return appsRoot;
				});
			return allApps.Resources.Select(r => r.Info).ToList();
		}

		public List<PcfOrganizationInfo> GetAllOrganizations(string container)
		{
			var allOrganizations = GetFromCache<Organizations.RootObject, PcfOrganizationInfo>(
				() => GetAllInfo<PcfOrganizationInfo, Organizations.RootObject>(container));
			return allOrganizations.Resources.Select(r => r.Info).ToList();
		}

		public List<PcfRouteInfo> GetAllRoutes(string container)
		{
			var allRoutes = GetFromCache<Routes.RootObject, PcfRouteInfo>(
				() => GetAllInfo<PcfRouteInfo, Routes.RootObject>(container));
			return allRoutes.Resources.Select(r => r.Info).ToList();
		}

		public List<PcfServicePlan> GetAllServicePlans()
		{
			var allServicePlans = GetAllInfo<PcfServicePlan, ServicePlans.RootObject>("", "/v2/service_plans");
			return allServicePlans.Resources.Select(r => r.Info).ToList();
		}

		public List<PcfServiceInfo> GetAllServices(string container)
		{
			var allServices = GetFromCache<Services.RootObject, PcfServiceInfo>(
				() =>
					{
						var servicesRoot = GetAllInfo<PcfServiceInfo, Services.RootObject>(container);
						servicesRoot.Resources.ForEach(r => r.Info.InstanceId = r.Metadata.Guid);
						foreach (var servicePlan in GetAllServicePlans())
						{
							servicesRoot.Resources
								.Find(r => r.Info.InstanceId == servicePlan.ServiceGuid)
								?.Info.Plans.Add(servicePlan);
						}
						return servicesRoot;
					});
			return allServices.Resources.Select(r => r.Info).ToList();
		}

		public void Login(string username, string password)
		{
			var authenticationResults = GetAuthToken(username, password);
			SaveAuthorization(authenticationResults);
		}

		internal List<TInfo> GetAll<TInfo, TRoot>(string container, string uri = "") where TRoot : InfoBase.RootObject<TInfo>
		{
			var allInfo = GetAllInfo<TInfo, TRoot>(container, uri);
			return allInfo.Resources.Select(r => r.Info).ToList();
		}

		internal List<PcfDomainInfo> GetAllDomains(string organization)
		{
			var orgs = GetAllOrganizations("organizations");
			var org = orgs.Find(oi => oi.Name == organization);
			if (Helpers.CheckNullOrEmpty(org))
			{
				return new List<PcfDomainInfo>();
			}
			var allDomainInfo = GetAllInfo<PcfDomainInfo, Domains.RootObject>(organization, org.DomainsUrl);
			var allPrivateDomainInfo = GetAllInfo<PcfDomainInfo, Domains.RootObject>(organization, org.PrivateDomainsUrl);
			return allDomainInfo.Resources
				.Select(r => r.Info)
				.Union(allPrivateDomainInfo.Resources.Select(r => r.Info))
				.ToList();
		}

		internal TRoot GetAllInfo<TInfo, TRoot>(string container, string uri = "") where TRoot : InfoBase.RootObject<TInfo>
		{
			var rawInfo = GetRawContainerContents(container, uri);
			return JsonConvert.DeserializeObject<TRoot>(rawInfo);
		}

		internal List<PcfManagerInfo> GetAllManagers(string organization)
		{
			var orgs = GetAllOrganizations("organizations");
			var org = orgs.Find(oi => oi.Name == organization);
			if (Helpers.CheckNullOrEmpty(org))
			{
				return new List<PcfManagerInfo>();
			}
			var allManagerInfo = GetAllInfo<PcfManagerInfo, Managers.RootObject>(organization, org.ManagersUrl);
			return allManagerInfo.Resources.Select(r => r.Info).ToList();
		}

		internal List<PcfUserInfo> GetAllUsers(string organization)
		{
			var orgs = GetAllOrganizations("organizations");
			var org = orgs.Find(oi => oi.Name == organization);
			if (Helpers.CheckNullOrEmpty(org))
			{
				return new List<PcfUserInfo>();
			}
			var allUserInfo = GetAllInfo<PcfUserInfo, Users.RootObject>(organization, org.UsersUrl);
			return allUserInfo.Resources.Select(r => r.Info).ToList();
		}

		private void GetAllServiceBindings(Apps.RootObject allApps)
		{
			foreach (var app in allApps.Resources.Select(r => r.Info))
			{
				var serviceBindingUrl = app.ServiceBindingsUrl;
				var serviceBindings = GetAllInfo<PcfServiceBinding, ServiceBindings.RootObject>("service_bindings", serviceBindingUrl);
				foreach (var serviceBinding in serviceBindings.Resources)
				{
					var serviceInstanceUrl = serviceBinding.Info.ServiceInstanceUrl;
					var rawServiceInstanceInfo = GetRawContainerContents("service_instance", serviceInstanceUrl);
					var serviceInstance = JsonConvert.DeserializeObject<ServiceInstance.RootObject>(rawServiceInstanceInfo);
					serviceBinding.Info.ServiceInstance = serviceInstance.ServiceInstance;
					app.ServiceBindings.Add(serviceBinding.Info);
				}
			}
		}

		private string GetAuthHeader(string userName)
		{
			return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userName}:"));
		}

		private OAuthResponse GetAuthToken(string userName, string password)
		{
			var data = IsLocal
				? $"grant_type=password&password={password}&scope=&username={userName}"
				: $"username={userName}&password={password}&client_id=cf&grant_type=password&response_type=token";
			return GetOauthReponse(data);
		}

		/// <summary>
		///     This method gets the root object from a cache.
		/// </summary>
		/// <typeparam name="TRoot">This is the type of the root object.</typeparam>
		/// <typeparam name="TInfo">This is the type of the information contained in the root object.</typeparam>
		/// <param name="function">This is the function that retrieves the root object.</param>
		/// <param name="callerName">This is the name of the calling method.</param>
		/// <returns>This is a current instance of the root object.</returns>
		/// <remarks>
		///     <para>
		///         Note 1: <see cref="RootObjectLifetimeInSeconds" /> contains the lifetime (in seconds) for each root object type managed by the
		///         cache. If there is no entry, 30 seconds is used.
		///     </para>
		/// </remarks>
		private TRoot GetFromCache<TRoot, TInfo>(Func<TRoot> function, [CallerMemberName]string callerName = "") where TRoot : InfoBase.RootObject<TInfo>

		{
			var rootType = typeof(TRoot);
			if (CachedRootObjects.ContainsKey(rootType) && (CachedRootObjects[rootType] as TRoot).UtcExpiration > DateTime.UtcNow)
			{
				Tracer.WriteLineIndent($"Reusing contents of {rootType}.", callerName);
				return CachedRootObjects[rootType] as TRoot;
			}
			Tracer.WriteLineIndent($"Populating contents of {rootType}.", callerName);
			var rootObject = function();
			// Note 1
			var lifetimeSeconds = RootObjectLifetimeInSeconds.ContainsKey(rootType) ? RootObjectLifetimeInSeconds[rootType] : 30;
			rootObject.UtcExpiration = DateTime.UtcNow.AddSeconds(lifetimeSeconds);
			if (CachedRootObjects.ContainsKey(rootType))
			{
				CachedRootObjects[rootType] = rootObject;
			}
			else
			{
				CachedRootObjects.Add(rootType, rootObject);
			}
			return rootObject;
		}

		/// <summary>
		///     This method gets the root object from a cache.
		/// </summary>
		/// <typeparam name="TRoot">This is the type of the root object.</typeparam>
		/// <param name="function">This is the function that retrieves the root object.</param>
		/// <param name="callerName">This is the name of the calling method.</param>
		/// <returns>This is a current instance of the root object.</returns>
		/// <remarks>
		///     <para>
		///         Note 1: <see cref="RootObjectLifetimeInSeconds" /> contains the lifetime (in seconds) for each root object type managed by the
		///         cache. If there is no entry, 30 seconds is used.
		///     </para>
		/// </remarks>
		private TRoot GetFromCache<TRoot>(Func<TRoot> function, [CallerMemberName]string callerName = "") where TRoot : InfoBase.RootObjectSimple<TRoot>

		{
			var rootType = typeof(TRoot);
			if (CachedRootObjects.ContainsKey(rootType) && (CachedRootObjects[rootType] as TRoot).UtcExpiration > DateTime.UtcNow)
			{
				Tracer.WriteLineIndent($"Reusing contents of {rootType}.", callerName);
				return CachedRootObjects[rootType] as TRoot;
			}
			Tracer.WriteLineIndent($"Populating contents of {rootType}.", callerName);
			var rootObject = function();
			// Note 1
			var lifetimeSeconds = RootObjectLifetimeInSeconds.ContainsKey(rootType) ? RootObjectLifetimeInSeconds[rootType] : 30;
			rootObject.UtcExpiration = DateTime.UtcNow.AddSeconds(lifetimeSeconds);
			if (CachedRootObjects.ContainsKey(rootType))
			{
				CachedRootObjects[rootType] = rootObject;
			}
			else
			{
				CachedRootObjects.Add(rootType, rootObject);
			}
			return rootObject;
		}

		private OAuthResponse GetOauthReponse(string data)
		{
			var pcfInfo = GetFromCache(() => JsonConvert.DeserializeObject<Info.PcfInfo>(GetRestResponse($"http://api.{Uri}", "/v2/info")));
			var loginInfo = GetFromCache(() => JsonConvert.DeserializeObject<LoginInfo.PcfLoginInfo>(GetRestResponse(pcfInfo.AuthorizationEndpoint, "/login")));
			var uri = IsLocal
				? $"http://login.{Uri}/oauth/token"
				: $"{loginInfo.Links.Login}/oauth/token";
			var request = (HttpWebRequest)WebRequest.Create(uri);
			request.ProtocolVersion = HttpVersion.Version11;
			request.Headers.Add("Authorization", $"Basic {GetAuthHeader("cf")}");
			request.Accept = "Application/json";
			request.ContentType = "application/x-www-form-urlencoded";
			request.Method = "POST";
			byte[] dataStream = Encoding.UTF8.GetBytes(data);
			request.ContentLength = dataStream.Length;
			using (var newStream = request.GetRequestStream())
			{
				newStream.Write(dataStream, 0, dataStream.Length);
			}
			using (var response = request.GetResponse())
			using (var reader = new StreamReader(response.GetResponseStream()))
			{
				return JsonConvert.DeserializeObject<OAuthResponse>(reader.ReadToEnd());
			}
		}

		private string GetRawContainerContents(string container, string url = "")
		{
			if (DateTime.UtcNow >= ExpirationTime)
			{
				RefreshAuthToken();
			}
			var httpType = IsLocal ? "http" : "https";
			var command = Helpers.CheckNullOrEmpty(url) ? $"/v2/{container}" : url;
			var request = (HttpWebRequest)WebRequest.Create($"{httpType}://api.{Uri}{command}");
			request.Headers.Add("Authorization", $"Bearer {AccessToken}");
			request.Method = "GET";
			using (var response = request.GetResponse())
			using (var reader = new StreamReader(response.GetResponseStream()))
			{
				return Helpers.SaveJson(container, reader.ReadToEnd());
			}
		}

		private string GetRestResponse(string uri, string restEndpoint)
		{
			var simpleEndpoint = restEndpoint.StartsWith("/") ? restEndpoint.Substring(1) : restEndpoint;
			var request = (HttpWebRequest)WebRequest.Create($"{uri}/{simpleEndpoint}");
			request.ProtocolVersion = HttpVersion.Version11;
			request.Accept = "Application/json";
			request.Method = "GET";
			using (var response = request.GetResponse())
			using (var reader = new StreamReader(response.GetResponseStream()))
			{
				return reader.ReadToEnd();
			}
		}

		/// <summary> <token method refreshes the authentication token. </summary>
		private void RefreshAuthToken()
		{
			var data = $"client_id=cf&grant_type=refresh_token&refresh_token={RefreshToken}&response_type=token";
			SaveAuthorization(GetOauthReponse(data));
		}

		/// <summary>
		///     This method saves the authorization information.
		/// </summary>
		/// <param name="authenticationResults">The authentication results.</param>
		private void SaveAuthorization(OAuthResponse authenticationResults)
		{
			AccessToken = authenticationResults.AccessToken;
			RefreshToken = authenticationResults.RefreshToken;
			ExpirationTime = DateTime.UtcNow.AddSeconds(authenticationResults.ExpiresIn);
		}
	}
}