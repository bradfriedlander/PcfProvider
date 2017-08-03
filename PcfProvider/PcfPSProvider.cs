using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Provider;
using System.Text.RegularExpressions;
using PcfAppInfo = PcfProvider.Apps.PcfAppInfo;
using PcfServiceInfo = PcfProvider.Services.PcfServiceInfo;

namespace PcfProvider
{
	[CmdletProvider("Pcf", ProviderCapabilities.Credentials | ProviderCapabilities.ShouldProcess)]
	public class PcfPSProvider : NavigationCmdletProvider, IContentCmdletProvider
	{
		public PcfPSProvider() : base()
		{
			_trace.WriteLine($"Entering {nameof(PcfPSProvider)} constructor", isBlankBefore: true);
			if (!Helpers.CheckNullOrEmpty(currentDriveInfo))
			{
				_trace.WriteLine($"{currentDriveInfo.Name}: http://{currentDriveInfo.Uri}('{currentDriveInfo.UserName}')");
			}
		}

		private static readonly string[] firstLevelNames = new string[] { "apps", "organizations", "routes", "services" };
		private static PcfDriveInfo currentDriveInfo;
		private static string password;
		private static string uri;
		private static string userName;
		private readonly TraceTest _trace = new TraceTest("PS", nameof(PcfPSProvider));

		public void ClearContent(string path)
		{
			_trace.WriteLine($"Entering {nameof(ClearContent)}({path})");
			throw new NotImplementedException();
		}

		public object ClearContentDynamicParameters(string path)
		{
			_trace.WriteLine($"Entering {nameof(ClearContentDynamicParameters)}({path})");
			throw new NotImplementedException();
		}

		public IContentReader GetContentReader(string path)
		{
			_trace.WriteLine($"Entering {nameof(GetContentReader)}({path})");
			throw new NotImplementedException();
		}

		public object GetContentReaderDynamicParameters(string path)
		{
			_trace.WriteLine($"Entering {nameof(GetContentReaderDynamicParameters)}({path})");
			throw new NotImplementedException();
		}

		public IContentWriter GetContentWriter(string path)
		{
			_trace.WriteLine($"Entering {nameof(GetContentWriter)}({path})");
			throw new NotImplementedException();
		}

		public object GetContentWriterDynamicParameters(string path)
		{
			_trace.WriteLine($"Entering {nameof(GetContentWriterDynamicParameters)}({path})");
			throw new NotImplementedException();
		}

		protected override string[] ExpandPath(string path)
		{
			return FindMatchingNames(path);
		}

		/// <summary>
		///     This method gets the child items. This is the full object for each c.
		/// </summary>
		/// <param name="path">This is the path.</param>
		/// <param name="recurse">If set to <c>true</c>, then recurse through all child containers.</param>
		protected override void GetChildItems(string path, bool recurse)
		{
			_trace.WriteLine($"Entering {nameof(GetChildItems)}({path}, {recurse.ToString()})");
			if (currentDriveInfo.PathIsDrive(path))
			{
				foreach (var name in firstLevelNames)
				{
					WriteItemObject(FirstLevelObject(name), path, true);
					if(recurse)
					{
						GetChildItems(name, recurse);
					}
				}
				return;
			}
			var containerNames = GetContainerNames(path);
			if (1 == containerNames.Length)
			{
				var containerName = containerNames.Last();
				switch (containerName)
				{
					case "apps":
						GetApps(containerName).ForEach(ai => WriteItemObject(ai, path, false));
						break;

					case "organizations":
						GetOrganizations(containerName).ForEach(ai => WriteItemObject(ai, path, false));
						break;

					case "services":
						GetServices(containerName).ForEach(ai => WriteItemObject(ai, path, false));
						break;

					default:
						WriteItemObject("None", path, false);
						break;
				}
				return;
			}
			return;
		}

		protected override string GetChildName(string path)
		{
			_trace.WriteLine($"Entering {nameof(GetChildName)}({path})");
			return Path.GetFileName(path);
		}

		/// <summary>
		///     This method gets the names of all child objects.
		/// </summary>
		/// <param name="path">This is the path.</param>
		/// <param name="returnContainers">This controls which containers to return.</param>
		protected override void GetChildNames(string path, ReturnContainers returnContainers)
		{
			_trace.WriteLine($"Entering {nameof(GetChildNames)}({path}, returnContainers)");
			if (currentDriveInfo.PathIsDrive(path))
			{
				foreach (var name in firstLevelNames)
				{
					WriteItemObject(FirstLevelObject(name), path, true);
				}
			}
			var containerNames = GetContainerNames(path);
			if (1 == containerNames.Length)
			{
				var containerName = containerNames.Last();
				switch (containerName)
				{
					case "apps":
						GetApps(containerName).ForEach(ai => WriteItemObject(ai.Name, path, false));
						break;

					case "services":
						GetServices(containerName).ForEach(ai => WriteItemObject(ai.Name, path, false));
						break;

					default:
						break;
				}
			}
		}

		protected override bool HasChildItems(string path)
		{
			_trace.WriteLine($"Entering {nameof(HasChildItems)}({path})");
			if (currentDriveInfo.PathIsDrive(path))
			{
				return true;
			}
			var containerNames = GetContainerNames(path);
			return 2 < containerNames.Length;
		}

		protected override bool IsItemContainer(string path)
		{
			_trace.WriteLine($"Entering {nameof(IsItemContainer)}({path})");
			return true;
		}

		//protected override string GetParentPath(string path, string root)
		//{
		//	_trace.WriteLine($"Entering {nameof(GetParentPath)}({path}, {root})");
		//	if (!string.IsNullOrEmpty(root) && !path.Contains(root))
		//	{
		//		return null;
		//	}
		//	if (currentDriveInfo.PathIsDrive(path))
		//	{
		//		return null;
		//	}
		//	return path.Substring(0, path.LastIndexOf(PcfDriveInfo.PathSeparator, StringComparison.OrdinalIgnoreCase));
		//}
		protected override bool IsValidPath(string path)
		{
			_trace.WriteLine($"Entering {nameof(IsValidPath)}({path})");
			return true;
		}

		protected override bool ItemExists(string path)
		{
			_trace.WriteLine($"Entering {nameof(ItemExists)}({path})");
			if (currentDriveInfo.PathIsDrive(path))
			{
				return true;
			}
			if (FindMatchingNames(path).Length > 0)
			{
				return true;
			}
			// TODO: fix this for checking second level items
			return true;
		}

		protected override object ItemExistsDynamicParameters(string path)
		{
			_trace.WriteLine($"Entering {nameof(ItemExistsDynamicParameters)}({path})");
			return null;
		}

		protected override PSDriveInfo NewDrive(PSDriveInfo drive)
		{
			_trace.WriteLine($"Entering {nameof(NewDrive)}");
			if (drive == null)
			{
				WriteErrorRecord(new ArgumentNullException(nameof(drive)), "NullDrive", ErrorCategory.InvalidArgument, null);
				return null;
			}
			if (PcfDoesNotExist(drive.Root))
			{
				WriteErrorRecord(new ArgumentException(nameof(drive.Root)), "NoRoot", ErrorCategory.InvalidArgument, drive.Root ?? "NONE");
				return null;
			}
			var newDriveParameters = DynamicParameters as RuntimeDefinedParameterDictionary;
			var isLocal = newDriveParameters["IsLocal"].IsSet ? ((SwitchParameter)(newDriveParameters["IsLocal"].Value)).ToBool() : false;
			uri = isLocal ? "api.local.pcfdev.io" : newDriveParameters["Uri"].IsSet ? newDriveParameters["Uri"].Value.ToString() : null;
			if (string.IsNullOrEmpty(uri))
			{
				WriteErrorRecord(new ArgumentNullException(nameof(uri)), "NoUri", ErrorCategory.InvalidArgument, null);
				return null;
			}
			userName = newDriveParameters["UserName"].IsSet ? newDriveParameters["UserName"].Value.ToString() : (isLocal ? "admin" : null);
			if (string.IsNullOrEmpty(userName))
			{
				WriteErrorRecord(new ArgumentNullException(nameof(userName)), "NoUserName", ErrorCategory.InvalidArgument, null);
				return null;
			}
			password = newDriveParameters["Password"].IsSet ? newDriveParameters["Password"].Value.ToString() : (isLocal ? "admin" : null);
			if (string.IsNullOrEmpty(password))
			{
				WriteErrorRecord(new ArgumentNullException(nameof(password)), "NoPassword", ErrorCategory.InvalidArgument, null);
				return null;
			}
			currentDriveInfo = new PcfDriveInfo(drive, uri, isLocal)
			{
				UserName = userName,
				Password = password
			};
			currentDriveInfo.CurrentLocation = currentDriveInfo.Root;
			currentDriveInfo.Connection.Login(currentDriveInfo.UserName, currentDriveInfo.Password);
			return currentDriveInfo;
		}

		protected override object NewDriveDynamicParameters()
		{
			var parameters = new RuntimeDefinedParameterDictionary();
			RuntimeDefinedParameter runDefParm = null;
			var required = new Collection<Attribute>
			{
				new ParameterAttribute()
				{
					ParameterSetName = "Required",
					Mandatory = true,
					ValueFromPipeline = false
				}
			};
			var optional = new Collection<Attribute>
			{
				new ParameterAttribute()
				{
					ParameterSetName = "Optional",
					Mandatory = false,
					ValueFromPipeline = false
				}
			};
			runDefParm = new RuntimeDefinedParameter("Uri", typeof(string), optional);
			parameters.Add("Uri", runDefParm);
			runDefParm = new RuntimeDefinedParameter("UserName", typeof(string), optional);
			parameters.Add("UserName", runDefParm);
			runDefParm = new RuntimeDefinedParameter("Password", typeof(string), optional);
			parameters.Add("Password", runDefParm);
			runDefParm = new RuntimeDefinedParameter("IsLocal", typeof(SwitchParameter), optional);
			parameters.Add("IsLocal", runDefParm);
			return parameters;
		}

		protected override string NormalizeRelativePath(string path, string basePath)
		{
			_trace.WriteLine($"Entering {nameof(NormalizeRelativePath)}({path}, {basePath})");
			// Normalize the paths first
			string normalPath = NormalizePath(path);
			normalPath = RemoveDriveFromPath(normalPath);
			string normalBasePath = NormalizePath(basePath);
			normalBasePath = RemoveDriveFromPath(normalBasePath);

			if (String.IsNullOrEmpty(normalBasePath))
			{
				return normalPath;
			}
			if (!normalPath.StartsWith(normalBasePath))
			{
				return null;
			}
			return normalPath.Remove(0, normalBasePath.Length);
		}

		protected override PSDriveInfo RemoveDrive(PSDriveInfo drive)
		{
			_trace.WriteLine($"Entering {nameof(RemoveDrive)}");
			if (drive == null)
			{
				WriteErrorRecord(new ArgumentNullException(nameof(drive)), "NullDrive", ErrorCategory.InvalidArgument, drive);
				return null;
			}
			var pcfDrive = drive as PcfDriveInfo;
			if (pcfDrive == null)
			{
				return null;
			}
			// TODO: log out of PCF
			currentDriveInfo = null;
			return pcfDrive;
		}

		private static string[] FindMatchingNames(string path)
		{
			if (currentDriveInfo.PathIsDrive(path))
			{
				return firstLevelNames;
			}
			var regexString = Regex.Escape(path).Replace(@"\\\*", ".*");
			var regex = new Regex($"^{regexString}$");
			return firstLevelNames.Where(fln => regex.IsMatch(fln)).Select(fln => fln).ToArray();
		}

		private static string[] GetContainerNames(string path)
		{
			return path.Split(new[] { PcfDriveInfo.PathSeparator }, StringSplitOptions.RemoveEmptyEntries);
		}

		private string FirstLevelObject(string value) => value;

		private List<PcfAppInfo> GetApps(string container) => currentDriveInfo.Connection.GetAllApps(container);

		private List<Organizations.PcfOrganization> GetOrganizations(string container) => currentDriveInfo.Connection.GetAllOrganizations(container);

		private List<PcfServiceInfo> GetServices(string container) => currentDriveInfo.Connection.GetAllServices(container);

		private string NormalizePath(string path) => path?.Replace(@".\", @"\") ?? string.Empty;

		private bool PcfDoesNotExist(string root)
		{
			if (string.IsNullOrEmpty(root))
			{
				return true;
			}
			// TODO: Check if PCF endpoint exists
			return false;
		}

		private string RemoveDriveFromPath(string path) => path.Replace($"{currentDriveInfo.Root}:", string.Empty);

		/// <summary>
		///     Writes the error record.
		/// </summary>
		/// <param name="exception">This is the exception.</param>
		/// <param name="message">This is the message explaining the error.</param>
		/// <param name="category">This is the category of the error.</param>
		/// <param name="target">This is the object that caused the error.</param>
		private void WriteErrorRecord(Exception exception, string message, ErrorCategory category, object target)
		{
			WriteError(new ErrorRecord(exception, message, category, target));
		}

		//protected override object NewItemDynamicParameters(string path, string itemTypeName, object newItemValue)
		//{
		//	_trace.WriteLine($"Entering {nameof(NewItemDynamicParameters)}");
		//	var dic = new RuntimeDefinedParameterDictionary();
		//	ParameterAttribute attrib = null;
		//	Collection<Attribute> col = null;
		//	RuntimeDefinedParameter runDefParm = null;
		//	attrib = new ParameterAttribute()
		//	{
		//		ParameterSetName = "MyParameters",
		//		Mandatory = false,
		//		ValueFromPipeline = false
		//	};
		//	col = new Collection<Attribute>
		//	{
		//		attrib
		//	};
		//	runDefParm = new RuntimeDefinedParameter("MyParameter1", typeof(string), col);
		//	dic.Add("MyParameter1", runDefParm);
		//	runDefParm = new RuntimeDefinedParameter("MyParameter2", typeof(string), col);
		//	dic.Add("MyParameter2", runDefParm);
		//	_trace.WriteLine("END NewItemDynamicParameters");
		//	return dic;
		//}
	}
}