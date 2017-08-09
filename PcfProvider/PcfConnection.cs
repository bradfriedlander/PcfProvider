using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using PcfAppInfo = PcfProvider.Apps.PcfAppInfo;
using PcfDomainInfo = PcfProvider.Domains.PcfDomainInfo;
using PcfOrganizationInfo = PcfProvider.Organizations.PcfOrganizationInfo;
using PcfRouteInfo = PcfProvider.Routes.PcfRouteInfo;
using PcfServiceBinding = PcfProvider.ServiceBindings.PcfServiceBinding;
using PcfServiceInfo = PcfProvider.Services.PcfServiceInfo;
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

		public Apps.RootObject AllApps { get; private set; }

		public Organizations.RootObject AllOrganizations { get; private set; }

		public Routes.RootObject AllRoutes { get; private set; }

		public Services.RootObject AllServices { get; private set; }

		public PSCredential Credential { get; }

		public DateTime ExpirationTime { get; private set; }

		public bool IsLocal { get; }

		public string RefreshToken { get; private set; }

		public TraceTest Tracer { get; }

		public string Uri { get; }

		public List<PcfAppInfo> GetAllApps(string container)
		{
			if (Helpers.CheckNullOrEmpty(AllApps))
			{
				AllApps = GetAllInfo<PcfAppInfo, Apps.RootObject>(container);
				AllApps.Resources.ForEach(r => r.Info.AppGuid = r.Metadata.Guid);
				GetAllServiceBindings();
			}
			else
			{
				Tracer.WriteLineIndent($"Reusing contents of {nameof(AllApps)}.");
			}
			return AllApps.Resources.Select(r => r.Info).ToList();
		}

		public List<PcfOrganizationInfo> GetAllOrganizations(string container)
		{
			if (Helpers.CheckNullOrEmpty(AllOrganizations))
			{
				AllOrganizations = GetAllInfo<PcfOrganizationInfo, Organizations.RootObject>(container);
			}
			else
			{
				Tracer.WriteLineIndent($"Reusing contents of {nameof(AllOrganizations)}.");
			}
			return AllOrganizations.Resources.Select(r => r.Info).ToList();
		}

		public List<PcfRouteInfo> GetAllRoutes(string container)
		{
			if (Helpers.CheckNullOrEmpty(AllRoutes))
			{
				AllRoutes = GetAllInfo<PcfRouteInfo, Routes.RootObject>(container);
			}
			else
			{
				Tracer.WriteLineIndent($"Reusing contents of {nameof(AllRoutes)}.");
			}
			return AllRoutes.Resources.Select(r => r.Info).ToList();
		}

		public List<PcfServiceInfo> GetAllServices(string container)
		{
			if (Helpers.CheckNullOrEmpty(AllOrganizations))
			{
				AllServices = GetAllInfo<PcfServiceInfo, Services.RootObject>(container);
			}
			else
			{
				Tracer.WriteLineIndent($"Reusing contents of {nameof(AllServices)}.");
			}
			return AllServices.Resources.Select(r => r.Info).ToList();
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

		private void GetAllServiceBindings()
		{
			foreach (var app in AllApps.Resources.Select(r => r.Info))
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

		private OAuthResponse GetOauthReponse(string data)
		{
			var pcfInfo = JsonConvert.DeserializeObject<Info.PcfInfo>(GetRestResponse($"http://api.{Uri}", "/v2/info"));
			var loginInfo = JsonConvert.DeserializeObject<LoginInfo.PcfLoginInfo>(GetRestResponse(pcfInfo.AuthorizationEndpoint, "/login"));
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