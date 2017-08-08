using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Provider;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using PcfAppInfo = PcfProvider.Apps.PcfAppInfo;
using PcfDomainInfo = PcfProvider.Domains.PcfDomainInfo;
using PcfServiceInfo = PcfProvider.Services.PcfServiceInfo;
using PcfUserInfo = PcfProvider.Users.PcfUserInfo;

namespace PcfProvider
{
	[CmdletProvider("Pcf", ProviderCapabilities.Credentials | ProviderCapabilities.ShouldProcess)]
	public class PcfPSProvider : NavigationCmdletProvider, IContentCmdletProvider
	{
		public PcfPSProvider()
		{
			_trace.WriteLine($"Entering {nameof(PcfPSProvider)} constructor", isBlankBefore: true);
			if (!Helpers.CheckNullOrEmpty(currentDriveInfo))
			{
				_trace.WriteLine($"{currentDriveInfo.Name}: http://{currentDriveInfo.Uri}('{currentDriveInfo.UserName}')");
			}
		}

		public enum PathType
		{
			Drive = 0,
			Category,
			PcfEntity,
			PcfSubCategory,
			PcfSubEntity
		}

		private static readonly string[] firstLevelNames = new string[] { "apps", "organizations", "routes", "services" };

		private static readonly Dictionary<string, string[]> secondLevelNames = new Dictionary<string, string[]>
		{
			["organizations"] = new string[] { "domains", "managers", "users" }
		};

		private static PcfDriveInfo currentDriveInfo;
		private static bool isLogItems;
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
			_trace.WriteLine($"Entering {nameof(ExpandPath)}({path})");
			return LogReturn(() => FindMatchingNames(path));
		}

		/// <summary>
		///     This method gets the child items. This is the full object for each c.
		/// </summary>
		/// <param name="path">This is the path.</param>
		/// <param name="recurse">If set to <c>true</c>, then recurse through all child containers.</param>
		protected override void GetChildItems(string path, bool recurse)
		{
			_trace.WriteLine($"Entering {nameof(GetChildItems)}({path}, {recurse.ToString()})");
			var containers = GetContainerNames(path);
			var count = containers.ContainerNames.Length;
			string category = count > 0 ? containers.ContainerNames[0] : "";
			string containerName = count > 0 ? containers.ContainerNames.Last() : "";
			string entityName = count > 1 ? containers.ContainerNames[1] : "";
			switch (containers.Level)
			{
				case PathType.Drive:
					foreach (var name in firstLevelNames)
					{
						WriteItemObject(FirstLevelObject(name), path, true);
						if (recurse)
						{
							GetChildItems(name, recurse);
						}
					}
					break;

				case PathType.Category:
					switch (category)
					{
						case "apps":
							GetApps(category).ForEach(ai => WriteItemObject(ai, path, false));
							break;

						case "organizations":
							GetOrganizations(category).ForEach(oi =>
							{
								WriteItemObject(oi, path, true);
								if (recurse)
								{
									GetChildItems(oi.Name, recurse);
								}
							});
							break;

						case "services":
							GetServices(category).ForEach(si => WriteItemObject(si, path, false));
							break;
					}
					break;

				case PathType.PcfEntity:
					if (secondLevelNames.ContainsKey(category))
					{
						secondLevelNames[category].ToList().ForEach(c => WriteItemObject(c, path, true));
					}
					break;

				case PathType.PcfSubCategory:
					if (secondLevelNames.ContainsKey(category) && secondLevelNames[category].Contains(containerName))
					{
						switch (containerName)
						{
							case "users":
								GetUsers(entityName).ForEach(ui => WriteItemObject(ui, path, false));
								break;

							case "domains":
								GetDomains(entityName).ForEach(di => WriteItemObject(di, path, false));
								break;
						}
					}
					break;

				case PathType.PcfSubEntity:
					break;
			}
		}

		protected override string GetChildName(string path)
		{
			_trace.WriteLine($"Entering {nameof(GetChildName)}({path})");
			var containers = GetContainerNames(path);
			return LogReturn(() => PathType.Drive == containers.Level ? "" : containers.ContainerNames.Last());
		}

		/// <summary>
		///     This method gets the names of all child objects.
		/// </summary>
		/// <param name="path">This is the path.</param>
		/// <param name="returnContainers">This controls which containers to return.</param>
		protected override void GetChildNames(string path, ReturnContainers returnContainers)
		{
			_trace.WriteLine($"Entering {nameof(GetChildNames)}({path}, returnContainers)");
			var containers = GetContainerNames(path);
			var count = containers.ContainerNames.Length;
			string category = count > 0 ? containers.ContainerNames[0] : "";
			string containerName = count > 0 ? containers.ContainerNames.Last() : "";
			string entityName = count > 1 ? containers.ContainerNames[1] : "";
			switch (containers.Level)
			{
				case PathType.Drive:
					foreach (var name in firstLevelNames)
					{
						WriteItemObject(FirstLevelObject(name), path, true);
					}
					break;

				case PathType.Category:
					switch (containerName)
					{
						case "apps":
							GetApps(containerName).ForEach(ai => WriteItemObject(ai.Name, path, false));
							break;

						case "organizations":
							GetOrganizations(containerName).ForEach(oi => WriteItemObject(oi.Name, path, true));
							break;

						case "services":
							GetServices(containerName).ForEach(si => WriteItemObject(si.Name, path, false));
							break;
					}
					break;

				case PathType.PcfEntity:
					if (secondLevelNames.ContainsKey(category))
					{
						secondLevelNames[category].ToList().ForEach(ci => WriteItemObject(ci, path, true));
					}
					break;

				case PathType.PcfSubCategory:
					if (secondLevelNames.ContainsKey(category) && secondLevelNames[category].Contains(containerName))
					{
						switch (containerName)
						{
							case "users":
								GetUsers(entityName).ForEach(ui => WriteItemObject(ui.Name, path, false));
								break;

							case "domains":
								GetDomains(entityName).ForEach(di => WriteItemObject(di.Name, path, false));
								break;
						}
					}
					break;

				case PathType.PcfSubEntity:
					break;
			}
		}

		protected override void GetItem(string path)
		{
			_trace.WriteLine($"Entering {nameof(GetItem)}({path})");
			var containers = GetContainerNames(path);
			var level = containers.Level;
			var count = containers.ContainerNames.Length;
			string category = count > 0 ? containers.ContainerNames[0] : "";
			string containerName = count > 0 ? containers.ContainerNames.Last() : "";
			string entityName;
			switch (level)
			{
				case PathType.Drive:
					WriteItemObject(currentDriveInfo, path, true);
					break;

				case PathType.Category:
					var categories = FindMatchingNames(path);
					if (categories.Length == 1)
					{
						WriteItemObject(categories[0], path, true);
					}
					break;

				case PathType.PcfEntity:
					category = containers.ContainerNames[0];
					entityName = containers.ContainerNames.Last();
					switch (category)
					{
						case "apps":
							GetApps(category).Where(ai => ai.Name == entityName).ToList().ForEach(ai => WriteItemObject(ai, path, false));
							break;

						case "organizations":
							GetOrganizations(category).Where(oi => oi.Name == entityName).ToList().ForEach(oi => WriteItemObject(oi, path, true));
							break;

						case "services":
							GetServices(category).Where(si => si.Name == entityName).ToList().ForEach(si => WriteItemObject(si, path, false));
							break;
					}
					break;

				case PathType.PcfSubCategory:
					category = containers.ContainerNames[0];
					if (secondLevelNames.ContainsKey(category))
					{
						var subentity = containers.ContainerNames[2];
						if (secondLevelNames[category].Contains(subentity))
						{
							WriteItemObject(subentity, path, true);
						}
					}
					break;

				case PathType.PcfSubEntity:
					// TODO: Add support
					break;
			}
		}

		protected override bool HasChildItems(string path)
		{
			_trace.WriteLine($"Entering {nameof(HasChildItems)}({path})");
			if (currentDriveInfo.PathIsDrive(path))
			{
				return LogReturn(() => true);
			}
			var containers = GetContainerNames(path);
			var count = containers.ContainerNames.Length;
			string category = count > 0 ? containers.ContainerNames[0] : "";
			string containerName = count > 0 ? containers.ContainerNames.Last() : "";
			string entityName = count > 1 ? containers.ContainerNames[1] : "";
			switch (containers.Level)
			{
				case PathType.Drive:
				case PathType.Category:
					return LogReturn(() => true);

				case PathType.PcfEntity:
					return LogReturn(() => secondLevelNames.ContainsKey(category));

				case PathType.PcfSubCategory:
					var thirdLevel = containers.ContainerNames.Last();
					if (secondLevelNames.ContainsKey(category))
					{
						return LogReturn(() => secondLevelNames[category].Contains(thirdLevel));
					}
					return LogReturn(() => false);

				case PathType.PcfSubEntity:
					// TODO: Add support
					break;

				default:
					var message = $"'{containers.Level}' is not a supported enumeration.";
					WriteErrorRecord(
						new ArgumentOutOfRangeException(nameof(containers.Level), containers.Level, message),
						message,
						ErrorCategory.InvalidArgument,
						path);
					break;
			}
			return LogReturn(() => false);
		}

		protected override bool IsItemContainer(string path)
		{
			_trace.WriteLine($"Entering {nameof(IsItemContainer)}({path})");
			var containers = GetContainerNames(path);
			switch (containers.Level)
			{
				case PathType.Drive:
				case PathType.Category:
					return LogReturn(() => true);

				case PathType.PcfEntity:
					var category = containers.ContainerNames[0];
					return LogReturn(() => secondLevelNames.ContainsKey(category));

				case PathType.PcfSubCategory:
					return LogReturn(() => true);

				case PathType.PcfSubEntity:
					return LogReturn(() => false);
			}
			return LogReturn(() => false);
		}

		/// <summary>
		///     Joins two strings with a provider specific path separator.
		/// </summary>
		/// <param name="parent">The parent segment of a path to be joined with the child.</param>
		/// <param name="child">The child segment of a path to be joined with the parent.</param>
		/// <returns>A string that represents the parent and child segments of the path joined by a path separator.</returns>

		protected override bool IsValidPath(string path)
		{
			_trace.WriteLine($"Entering {nameof(IsValidPath)}({path})");
			return true;
		}

		protected override bool ItemExists(string path)
		{
			_trace.WriteLine($"Entering {nameof(ItemExists)}({path})");
			var containers = GetContainerNames(path);
			var count = containers.ContainerNames.Length;
			var level = containers.Level;
			string category = count > 0 ? containers.ContainerNames[0] : "";
			string containerName = count > 0 ? containers.ContainerNames.Last() : "";
			string entityName = count > 1 ? containers.ContainerNames[1] : "";
			string subCategory = count > 2 ? containers.ContainerNames[2] : "";
			switch (level)
			{
				case PathType.Drive:
					return LogReturn(() => true);

				case PathType.Category:
					return LogReturn(() => FindMatchingNames(path).Length > 0);

				case PathType.PcfEntity:
					switch (category)
					{
						case "apps":
							return LogReturn(() => GetApps(category).Any(ai => entityName == ai.Name));

						case "services":
							return LogReturn(() => GetServices(category).Any(si => entityName == si.Name));

						case "organizations":
							return LogReturn(() => GetOrganizations(category).Any(oi => entityName == oi.Name));
					}
					break;

				case PathType.PcfSubCategory:
					return LogReturn(() => secondLevelNames.ContainsKey(category));

				case PathType.PcfSubEntity:
					if (secondLevelNames.ContainsKey(category) && secondLevelNames[category].Contains(subCategory))
					{
						switch (subCategory)
						{
							case "users":
								return LogReturn(() => GetUsers(entityName).Any(ui => ui.Name == containerName));

							case "domains":
								return LogReturn(() => GetDomains(entityName).Any(di => di.Name == containerName));

							case "managers":
								// TODO: add support
								return LogReturn(() => false);
						}
					}
					return LogReturn(() => false);

				default:
					var message = $"'{level}' is not a supported enumeration.";
					WriteErrorRecord(
						new ArgumentOutOfRangeException(nameof(level), level, message),
						message,
						ErrorCategory.InvalidArgument,
						path);
					break;
			}
			return LogReturn(() => false);
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
			isLogItems = newDriveParameters["IsLogItems"].IsSet ? ((SwitchParameter)(newDriveParameters["IsLogItems"].Value)).ToBool() : false;
			var isLocal = newDriveParameters["IsLocal"].IsSet ? ((SwitchParameter)(newDriveParameters["IsLocal"].Value)).ToBool() : false;
			uri = isLocal ? "local.pcfdev.io" : newDriveParameters["Uri"].IsSet ? newDriveParameters["Uri"].Value.ToString() : null;
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
			currentDriveInfo = new PcfDriveInfo(drive, uri, isLocal, _trace)
			{
				UserName = userName,
			};
			currentDriveInfo.CurrentLocation = currentDriveInfo.Root;
			currentDriveInfo.Connection.Login(userName, password);
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
			runDefParm = new RuntimeDefinedParameter("IsLogItems", typeof(SwitchParameter), optional);
			parameters.Add("IsLogItems", runDefParm);
			return parameters;
		}

		protected override string NormalizeRelativePath(string path, string basePath)
		{
			_trace.WriteLine($"Entering {nameof(NormalizeRelativePath)}({path}, {basePath})");
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
			return LogReturn(() => normalPath.Remove(0, normalBasePath.Length));
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

		/// <summary>
		///     This method writes the <paramref name="item" /> to the PowerShell object output after logging the request using <see
		///     cref="CmdletProvider.WriteItemObject(object, string, bool)" />.
		/// </summary>
		/// <param name="item">This is the item object to return to the PowerShell pipeline.</param>
		/// <param name="path">This is the path of <paramref name="item" />.</param>
		/// <param name="isContainer">If set to <c>true</c>, <paramref name="item" /> is a container.</param>
		/// <param name="callerName">This is the name of the calling method.</param>
		protected void WriteItemObject(object item, string path, bool isContainer, [CallerMemberName]string callerName = "")
		{
			var itemType = item.GetType().ToString();
			string itemName;
			switch (itemType)
			{
				case "System.String":
					itemName = item as string;
					break;

				default:
					itemName = ((dynamic)item).Name;
					break;
			}
			if (isLogItems)
			{
				_trace.WriteLine($"[{callerName}]: {nameof(WriteItemObject)}('{itemName}', '{path}', {isContainer})");
			}
			base.WriteItemObject(item, path, isContainer);
		}

		private static string[] FindMatchingNames(string path)
		{
			if (currentDriveInfo.PathIsDrive(path))
			{
				return firstLevelNames;
			}
			var regexString = Regex.Escape(path).Replace(@"\\", "").Replace("*", ".*");
			var regex = new Regex($"^{regexString}$");
			return firstLevelNames.Where(fln => regex.IsMatch(fln)).Select(fln => fln).ToArray();
		}

		private string FirstLevelObject(string value) => value;

		private List<PcfAppInfo> GetApps(string container) => currentDriveInfo.Connection.GetAllApps(container);

		private (PathType Level, string[] ContainerNames) GetContainerNames(string path)
		{
			if (currentDriveInfo.PathIsDrive(path))
			{
				return (PathType.Drive, new string[] { });
			}
			var normalizedPath = NormalizePath(path);
			var containerNames = path.Split(new[] { PcfDriveInfo.PathSeparator }, StringSplitOptions.RemoveEmptyEntries);
			return ((PathType)(containerNames.Length), containerNames);
		}

		private List<PcfDomainInfo> GetDomains(string organization) => currentDriveInfo.Connection.GetAllDomains(organization);

		private List<Organizations.PcfOrganizationInfo> GetOrganizations(string container) => currentDriveInfo.Connection.GetAllOrganizations(container);

		private List<PcfServiceInfo> GetServices(string container) => currentDriveInfo.Connection.GetAllServices(container);

		private List<PcfUserInfo> GetUsers(string organization) => currentDriveInfo.Connection.GetAllUsers(organization);

		private T LogReturn<T>(Func<T> function, [CallerMemberName]string callerName = "")
		{
			T returnValue = function();
			if (isLogItems)
			{
				_trace.WriteLine($"[{callerName}] returned '{returnValue}'");
			}
			return returnValue;
		}

		private string NormalizePath(string path) => path?.Replace("/", @"\").Replace(@".\", @"\") ?? string.Empty;

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
	}
}