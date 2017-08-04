using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using PcfAppInfo = PcfProvider.Apps.PcfAppInfo;
using PcfOrganization = PcfProvider.Organizations.PcfOrganization;
using PcfServiceInfo = PcfProvider.Services.PcfServiceInfo;

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

		public Services.RootObject AllServices { get; private set; }

		public PSCredential Credential { get; }

		public DateTime ExpirationTime { get; private set; }

		public bool IsLocal { get; }

		public string RefreshToken { get; private set; }

		public TraceTest Tracer { get; }

		public string Uri { get; }

		public List<PcfAppInfo> GetAllApps(string container)
		{
			var allApps = new List<PcfAppInfo>();
			if (Helpers.CheckNullOrEmpty(AllApps))
			{
				var rawAppInfo = GetRawContainerContents(container);
				AllApps = JsonConvert.DeserializeObject<Apps.RootObject>(rawAppInfo);
				AllApps.Resources.ForEach(r =>
				{
					r.AppInfo.AppGuid = r.Metadata.Guid;
					allApps.Add(r.AppInfo);
				});
				GetAllServiceBindings(allApps);
			}
			else
			{
				Tracer.WriteLine($"==> {nameof(GetAllApps)}: Reusing contents of {nameof(AllApps)}.");
				AllApps.Resources.ForEach(r => allApps.Add(r.AppInfo));
			}
			return allApps;
		}

		public List<PcfOrganization> GetAllOrganizations(string container)
		{
			var allApps = new List<PcfOrganization>();
			if (Helpers.CheckNullOrEmpty(AllOrganizations))
			{
				var rawAppInfo = GetRawContainerContents(container);
				AllOrganizations = JsonConvert.DeserializeObject<Organizations.RootObject>(rawAppInfo);
			}
			else
			{
				Tracer.WriteLine($"==> {nameof(GetAllOrganizations)}: Reusing contents of {nameof(AllOrganizations)}.");
			}
			AllOrganizations.Resources.ForEach(r => allApps.Add(r.Organization));
			return allApps;
		}

		public List<PcfServiceInfo> GetAllServices(string container)
		{
			var allServices = new List<PcfServiceInfo>();
			if (Helpers.CheckNullOrEmpty(AllOrganizations))
			{
				var rawServiceInfo = GetRawContainerContents(container);
				AllServices = JsonConvert.DeserializeObject<Services.RootObject>(rawServiceInfo);
			}
			else
			{
				Tracer.WriteLine($"==> {nameof(GetAllServices)}: Reusing contents of {nameof(AllServices)}.");
			}
			AllServices.Resources.ForEach(r => allServices.Add(r.ServiceInfo));
			return allServices;
		}

		public void Login(string username, string password)
		{
			var authenticationResults = GetAuthToken(username, password);
			SaveAuthorization(authenticationResults);
		}

		private void GetAllServiceBindings(List<PcfAppInfo> allApps)
		{
			foreach (var app in allApps)
			{
				var serviceBindingUrl = app.ServiceBindingsUrl;
				var rawServiceBindingInfo = GetRawContainerContents("service_bindings", serviceBindingUrl);
				var serviceBindings = JsonConvert.DeserializeObject<ServiceBindings.RootObject>(rawServiceBindingInfo);
				foreach (var serviceBinding in serviceBindings.Resources)
				{
					var serviceInstanceUrl = serviceBinding.ServiceBinding.service_instance_url;
					var rawServiceInstanceInfo = GetRawContainerContents("service_instance", serviceInstanceUrl);
					var serviceInstance = JsonConvert.DeserializeObject<ServiceInstance.RootObject>(rawServiceInstanceInfo);
					serviceBinding.ServiceBinding.ServiceInstance = serviceInstance.ServiceInstance;
					app.ServiceBindings.Add(serviceBinding.ServiceBinding);
				}
			}
		}

		private string GetAuthHeader(string userName)
		{
			return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userName}:"));
		}

		private OAuthResponse GetAuthToken(string userName, string password)
		{
			var data = $"username={userName}&password={password}&client_id=cf&grant_type=password&response_type=token";
			return GetOauthReponse(data);
		}

		private OAuthResponse GetOauthReponse(string data)
		{
			var httpType = IsLocal ? "http" : "https";
			var request = (HttpWebRequest)WebRequest.Create($"{httpType}://login.{Uri}/oauth/token");
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