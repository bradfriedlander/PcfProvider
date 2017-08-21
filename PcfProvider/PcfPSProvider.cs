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
using PcfManagerInfo = PcfProvider.Managers.PcfManagerInfo;
using PcfOrganizationInfo = PcfProvider.Organizations.PcfOrganizationInfo;
using PcfRouteInfo = PcfProvider.Routes.PcfRouteInfo;
using PcfRouteMapping = PcfProvider.RouteMappings.PcfRouteMapping;
using PcfServiceInfo = PcfProvider.Services.PcfServiceInfo;
using PcfSpaceInfo = PcfProvider.Spaces.PcfSpaceInfo;
using PcfStackInfo = PcfProvider.Stacks.PcfStackInfo;
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

		private const string appsCategory = "apps";
		private const string appSpaceSubcategory = "space";
		private const string domainsSubcategory = "domains";
		private const string managersSubcategory = "managers";
		private const string orgsCategory = "organizations";
		private const string plansSubcategory = "plans";
		private const string routeMappingsCategory = "routeMappings";
		private const string routesCategory = "routes";
		private const string routesSubcategory = "routes";
		private const string serviceBindingsSubcategory = "serviceBindings";
		private const string servicesCategory = "services";
		private const string spacesCategory = "spaces";
		private const string spacesSubcategory = "organizationSpaces";
		private const string stacksCategory = "stacks";
		private const string stackSubcategory = "stack";
		private const string usersSubcategory = "users";

		private static readonly Dictionary<string, CategoryInfo> categoriesInfo = new Dictionary<string, CategoryInfo>()
		{
			[appsCategory] = new CategoryInfo()
			{
				InfoType = typeof(PcfAppInfo),
				RootType = typeof(InfoBase.RootObject<PcfAppInfo>),
				Function = () => GetApps(),
				IsContainer = true
			},
			[orgsCategory] = new CategoryInfo()
			{
				InfoType = typeof(PcfOrganizationInfo),
				RootType = typeof(InfoBase.RootObject<PcfOrganizationInfo>),
				Function = () => GetOrganizations(),
				IsContainer = true
			},
			[routesCategory] = new CategoryInfo()
			{
				InfoType = typeof(PcfRouteInfo),
				RootType = typeof(InfoBase.RootObject<PcfRouteInfo>),
				Function = () => GetRoutes(),
				IsContainer = false
			},
			[routeMappingsCategory] = new CategoryInfo()
			{
				InfoType = typeof(PcfRouteMapping),
				RootType = typeof(InfoBase.RootObject<PcfRouteMapping>),
				Function = () => GetRouteMappings(),
				IsContainer = false
			},
			[servicesCategory] = new CategoryInfo()
			{
				InfoType = typeof(PcfServiceInfo),
				RootType = typeof(InfoBase.RootObject<PcfServiceInfo>),
				Function = () => GetServices(),
				IsContainer = true
			},
			[spacesCategory] = new CategoryInfo()
			{
				InfoType = typeof(PcfSpaceInfo),
				RootType = typeof(InfoBase.RootObject<PcfSpaceInfo>),
				Function = () => GetSpaces(),
				IsContainer = false
			},
			[stacksCategory] = new CategoryInfo()
			{
				InfoType = typeof(PcfStackInfo),
				RootType = typeof(InfoBase.RootObject<PcfStackInfo>),
				Function = () => GetStacks(),
				IsContainer = false
			}
		};

		private static readonly string[] categoryNames = new string[]
		{
			appsCategory,
			orgsCategory,
			routesCategory,
			routeMappingsCategory,
			servicesCategory,
			spacesCategory,
			stacksCategory
		};

		private static readonly Dictionary<string, string[]> subCategoryNames = new Dictionary<string, string[]>
		{
			[appsCategory] = new string[] { routesSubcategory, serviceBindingsSubcategory, appSpaceSubcategory, stackSubcategory },
			[orgsCategory] = new string[] { domainsSubcategory, managersSubcategory, spacesSubcategory, usersSubcategory },
			[servicesCategory] = new string[] { plansSubcategory }
		};

		private static PcfDriveInfo currentDriveInfo;

		private static bool isLogItems;

		private static string password;

		private static string uri;

		private static string userName;

		private readonly TraceTest _trace = new TraceTest("PS", nameof(PcfPSProvider));

		public void ClearContent(string path)
		{
			WriteTrace($"Entering {nameof(ClearContent)}({path})");
			throw new NotImplementedException();
		}

		public object ClearContentDynamicParameters(string path)
		{
			WriteTrace($"Entering {nameof(ClearContentDynamicParameters)}({path})");
			throw new NotImplementedException();
		}

		public IContentReader GetContentReader(string path)
		{
			WriteTrace($"Entering {nameof(GetContentReader)}({path})");
			throw new NotImplementedException();
		}

		public object GetContentReaderDynamicParameters(string path)
		{
			WriteTrace($"Entering {nameof(GetContentReaderDynamicParameters)}({path})");
			throw new NotImplementedException();
		}

		public IContentWriter GetContentWriter(string path)
		{
			WriteTrace($"Entering {nameof(GetContentWriter)}({path})");
			throw new NotImplementedException();
		}

		public object GetContentWriterDynamicParameters(string path)
		{
			WriteTrace($"Entering {nameof(GetContentWriterDynamicParameters)}({path})");
			throw new NotImplementedException();
		}

		protected override string[] ExpandPath(string path)
		{
			WriteTrace($"Entering {nameof(ExpandPath)}({path})");
			return LogReturn(() => FindMatchingNames(path));
		}

		/// <summary>
		///     This method gets the child items. This is the full object for each c.
		/// </summary>
		/// <param name="path">This is the path.</param>
		/// <param name="recurse">If set to <c>true</c>, then recurse through all child containers.</param>
		protected override void GetChildItems(string path, bool recurse)
		{
			WriteTrace($"Entering {nameof(GetChildItems)}({path}, {recurse.ToString()})");
			var pathParts = GetPathParts(path);
			switch (pathParts.level)
			{
				case PathType.Drive:
					foreach (var name in categoryNames)
					{
						WriteItemObject(FirstLevelObject(name), path, true);
						if (recurse)
						{
							GetChildItems(name, recurse);
						}
					}
					break;

				case PathType.Category:
					RecurseForName(GetFunctionForCategory(pathParts.category)(), path, recurse);
					break;

				case PathType.PcfEntity:
					if (subCategoryNames.ContainsKey(pathParts.category))
					{
						subCategoryNames[pathParts.category].ToList().ForEach(c =>
						{
							WriteItemObject(c, path, true);
							if (recurse)
							{
								GetChildItems(MakeChildPathname(path, c), recurse);
							}
						});
					}
					break;

				case PathType.PcfSubCategory:
					if (subCategoryNames.ContainsKey(pathParts.category) && subCategoryNames[pathParts.category].Contains(pathParts.subCategory))
					{
						switch (pathParts.subCategory)
						{
							case appSpaceSubcategory:
								var appForSpace = GetApps().Find(ai => ai.Name == pathParts.entityName);
								var spaceid = appForSpace?.SpaceGuid;
								RecurseForName(GetSpaces().FindAll(si => si.InstanceId == spaceid), path, recurse);
								break;

							case domainsSubcategory:
								RecurseForName(GetDomains(pathParts.entityName), path, recurse);
								break;

							case managersSubcategory:
								RecurseForName(GetManagers(pathParts.entityName), path, recurse);
								break;

							case plansSubcategory:
								var plans = GetServices(pathParts.category)
									.Where(s => s.Name == pathParts.entityName)
									.Select(s => s.Plans);
								RecurseForName(plans, path, recurse);
								break;

							case routesSubcategory:
								var appRoutes = GetApps(pathParts.category)
									.Where(ai => ai.Name == pathParts.entityName)
									.Select(ai => ai.Routes);
								RecurseForName(appRoutes, path, recurse);
								break;

							case serviceBindingsSubcategory:
								var services = GetApps(pathParts.category)
									.Where(ai => ai.Name == pathParts.entityName)
									.Select(ai => ai.ServiceBindings.Select(sb => sb.ServiceInstance));
								RecurseForName(services, path, recurse);
								break;

							case spacesSubcategory:
								var org = GetOrganizations(pathParts.category).Find(oi => oi.Name == pathParts.entityName);
								var orgId = org?.InstanceId;
								RecurseForName(GetSpaces(spacesCategory).FindAll(si => si.OrganizationGuid == orgId), path, recurse);
								break;

							case stackSubcategory:
								var app = GetApps(pathParts.category).Find(ai => ai.Name == pathParts.entityName);
								var stackId = app?.StackGuid;
								RecurseForName(GetStacks().FindAll(s => s.InstanceId == stackId), path, recurse);
								break;

							case usersSubcategory:
								RecurseForName(GetUsers(pathParts.entityName), path, recurse);
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
			WriteTrace($"Entering {nameof(GetChildName)}({path})");
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
			WriteTrace($"Entering {nameof(GetChildNames)}({path}, returnContainers)");
			var pathParts = GetPathParts(path);
			switch (pathParts.level)
			{
				case PathType.Drive:
					foreach (var name in categoryNames)
					{
						WriteItemObject(FirstLevelObject(name), path, true);
					}
					break;

				case PathType.Category:
					foreach (var e in GetFunctionForCategory(pathParts.category)())
					{
						WriteItemObject(e.Name, path, categoriesInfo[pathParts.category].IsContainer);
					}
					break;

				case PathType.PcfEntity:
					if (subCategoryNames.ContainsKey(pathParts.category))
					{
						subCategoryNames[pathParts.category].ToList().ForEach(ci => WriteItemObject(ci, path, true));
					}
					break;

				case PathType.PcfSubCategory:
					if (subCategoryNames.ContainsKey(pathParts.category) && subCategoryNames[pathParts.category].Contains(pathParts.subCategory))
					{
						switch (pathParts.subCategory)
						{
							case appSpaceSubcategory:
								var appForSpace = GetApps().Find(ai => ai.Name == pathParts.entityName);
								var spaceid = appForSpace?.SpaceGuid;
								WriteAllItems(GetSpaces().FindAll(si => si.InstanceId == spaceid), path);
								break;

							case domainsSubcategory:
								WriteAllItems(GetDomains(pathParts.entityName), path);
								break;

							case managersSubcategory:
								WriteAllItems(GetManagers(pathParts.entityName), path);
								break;

							case plansSubcategory:
								var plans = GetServices(pathParts.category)
									.Where(s => s.Name == pathParts.entityName)
									.Select(s => s.Plans);
								foreach (var plan in plans)
								{
									WriteAllItems(plan, path);
								}
								break;

							case routesSubcategory:
								var appRoutes = GetApps(pathParts.category)
									.Where(ai => ai.Name == pathParts.entityName)
									.Select(ai => ai.Routes);
								foreach (var appRoute in appRoutes)
								{
									WriteAllItems(appRoute, path);
								}
								break;

							case serviceBindingsSubcategory:
								var services = GetApps(pathParts.category)
									.Where(ai => ai.Name == pathParts.entityName)
									.Select(ai => ai.ServiceBindings.Select(sb => sb.ServiceInstance));
								foreach (var service in services)
								{
									WriteAllItems(service, path);
								}
								break;

							case spacesSubcategory:
								var org = GetOrganizations(pathParts.category).FirstOrDefault(oi => oi.Name == pathParts.entityName);
								var orgId = org?.InstanceId;
								WriteAllItems(GetSpaces().FindAll(si => si.OrganizationGuid == orgId), path);
								break;

							case stackSubcategory:
								var appForStack = GetApps(pathParts.category).Find(ai => ai.Name == pathParts.entityName);
								var stackId = appForStack?.StackGuid;
								WriteAllItems(GetStacks().FindAll(s => s.InstanceId == stackId), path);
								break;

							case usersSubcategory:
								WriteAllItems(GetUsers(pathParts.entityName), path);
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
			WriteTrace($"Entering {nameof(GetItem)}({path})");
			var pathParts = GetPathParts(path);
			switch (pathParts.level)
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
					foreach (var entity in GetFunctionForCategory(pathParts.category)().Where(ai => ai.Name == pathParts.entityName))
					{
						WriteItemObject(entity, path, categoriesInfo[pathParts.category].IsContainer);
					}
					break;

				case PathType.PcfSubCategory:
					if (subCategoryNames.ContainsKey(pathParts.category) && subCategoryNames[pathParts.category].Contains(pathParts.containerName))
					{
						WriteItemObject(pathParts.containerName, path, true);
					}
					break;

				case PathType.PcfSubEntity:
					if (subCategoryNames.ContainsKey(pathParts.category) && subCategoryNames[pathParts.category].Contains(pathParts.subCategory))
					{
						var subEntity = pathParts.subEntity;
						switch (pathParts.subCategory)
						{
							case appSpaceSubcategory:
								var appForSpace = GetApps().Find(ai => ai.Name == pathParts.entityName);
								var spaceid = appForSpace?.SpaceGuid;
								WriteAllItems(GetSubentityMatches(subEntity, GetSpaces().FindAll(si => si.InstanceId == spaceid)), path, false, true);
								break;

							case domainsSubcategory:
								WriteAllItems(GetSubentityMatches(subEntity, GetDomains(pathParts.entityName)), path, false, true);
								break;

							case managersSubcategory:
								WriteAllItems(GetSubentityMatches(subEntity, GetManagers(pathParts.entityName)), path, false, true);
								break;

							case plansSubcategory:
								var plans = GetServices(pathParts.category)
									.Where(s => s.Name == pathParts.entityName)
									.Select(s => s.Plans);
								WriteAllItems(GetSubentityMatches(subEntity, plans), path, false, true);
								break;

							case routesSubcategory:
								var appRoutes = GetApps(pathParts.category)
									.Where(ai => ai.Name == pathParts.entityName)
									.Select(ai => ai.Routes);
								WriteAllItems(GetSubentityMatches(subEntity, appRoutes), path, false, true);
								break;

							case serviceBindingsSubcategory:
								var services = GetApps(pathParts.category)
									.Where(ai => ai.Name == pathParts.entityName)
									.Select(ai => ai.ServiceBindings.Select(sb => sb.ServiceInstance));
								WriteAllItems(GetSubentityMatches(subEntity, services), path, false, true);
								break;

							case spacesSubcategory:
								var org = GetOrganizations(pathParts.category).Find(oi => oi.Name == pathParts.entityName);
								var orgId = org?.InstanceId;
								WriteAllItems(GetSpaces(spacesCategory).FindAll(si => si.OrganizationGuid == orgId), path, false, true);
								break;

							case stackSubcategory:
								var app = GetApps(pathParts.category).Find(ai => ai.Name == pathParts.entityName);
								var stackId = app?.StackGuid;
								WriteAllItems(GetStacks().FindAll(s => s.InstanceId == stackId), path, true);
								break;

							case usersSubcategory:
								WriteAllItems(GetSubentityMatches(subEntity, GetUsers(pathParts.entityName)), path, false, true);
								break;
						}
					}
					break;
			}
		}

		protected override bool HasChildItems(string path)
		{
			WriteTrace($"Entering {nameof(HasChildItems)}({path})");
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
					return LogReturn(() => subCategoryNames.ContainsKey(category));

				case PathType.PcfSubCategory:
					var thirdLevel = containers.ContainerNames.Last();
					if (subCategoryNames.ContainsKey(category))
					{
						return LogReturn(() => subCategoryNames[category].Contains(thirdLevel));
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
			WriteTrace($"Entering {nameof(IsItemContainer)}({path})");
			var containers = GetContainerNames(path);
			switch (containers.Level)
			{
				case PathType.Drive:
				case PathType.Category:
					return LogReturn(() => true);

				case PathType.PcfEntity:
					var category = containers.ContainerNames[0];
					return LogReturn(() => subCategoryNames.ContainsKey(category));

				case PathType.PcfSubCategory:
					return LogReturn(() => true);

				case PathType.PcfSubEntity:
					return LogReturn(() => false);
			}
			return LogReturn(() => false);
		}

		protected override bool IsValidPath(string path)
		{
			WriteTrace($"Entering {nameof(IsValidPath)}({path})");
			return true;
		}

		/// <summary>
		///     Joins two strings with a provider specific path separator.
		/// </summary>
		/// <param name="parent">The parent segment of a path to be joined with the child.</param>
		/// <param name="child">The child segment of a path to be joined with the parent.</param>
		/// <returns>A string that represents the parent and child segments of the path joined by a path separator.</returns>
		protected override bool ItemExists(string path)
		{
			WriteTrace($"Entering {nameof(ItemExists)}({path})");
			var pathParts = GetPathParts(path);
			switch (pathParts.level)
			{
				case PathType.Drive:
					return LogReturn(() => true);

				case PathType.Category:
					return LogReturn(() => FindMatchingNames(path).Length > 0);

				case PathType.PcfEntity:
					return LogReturn(() => GetFunctionForCategory(pathParts.category)().Any(e => pathParts.entityName == e.Name));

				case PathType.PcfSubCategory:
					return LogReturn(() => subCategoryNames.ContainsKey(pathParts.category));

				case PathType.PcfSubEntity:
					if (subCategoryNames.ContainsKey(pathParts.category) && subCategoryNames[pathParts.category].Contains(pathParts.subCategory))
					{
						switch (pathParts.subCategory)
						{
							case appSpaceSubcategory:
								var app = GetApps().Find(ai => ai.Name == pathParts.entityName);
								var spaceid = app?.SpaceGuid;
								return LogReturn(() => GetSpaces().Any(s => s.InstanceId == spaceid));

							case domainsSubcategory:
								return LogReturn(() => GetDomains(pathParts.entityName).Any(di => di.Name == pathParts.subEntity));

							case managersSubcategory:
								return LogReturn(() => GetManagers(pathParts.entityName).Any(mi => mi.Name == pathParts.subEntity));

							case plansSubcategory:
								var plans = GetServices()
									.Where(si => si.Name == pathParts.entityName)
									.Select(si => si.Plans);
								return LogReturn(() => plans.Any());

							case routesSubcategory:
								var appRoutes = GetApps()
									.Where(ai => ai.Name == pathParts.entityName)
									.Select(ai => ai.Routes);
								return LogReturn(() => appRoutes.Any());

							case serviceBindingsSubcategory:
								var services = GetApps()
									.Where(ai => ai.Name == pathParts.entityName)
									.Select(ai => ai.ServiceBindings.Select(sb => sb.ServiceInstance));
								return LogReturn(() => services.Any(s => s.Any(si => si.Name == pathParts.subEntity)));

							case spacesSubcategory:
								var org = GetOrganizations().Find(oi => oi.Name == pathParts.entityName);
								var orgId = org?.InstanceId;
								return LogReturn(() => GetSpaces().Any(si => si.OrganizationGuid == orgId));

							case stackSubcategory:
								var appForStack = GetApps().Find(ai => ai.Name == pathParts.entityName);
								var stackId = appForStack?.StackGuid;
								return LogReturn(() => GetStacks().Any(s => s.InstanceId == stackId));

							case usersSubcategory:
								return LogReturn(() => GetUsers(pathParts.entityName).Any(ui => ui.Name == pathParts.subEntity));
						}
					}
					return LogReturn(() => false);

				default:
					var message = $"'{pathParts.level}' is not a supported enumeration.";
					WriteErrorRecord(
						new ArgumentOutOfRangeException(nameof(pathParts.level), pathParts.level, message),
						message,
						ErrorCategory.InvalidArgument,
						path);
					break;
			}
			return LogReturn(() => false);
		}

		protected override object ItemExistsDynamicParameters(string path)
		{
			WriteTrace($"Entering {nameof(ItemExistsDynamicParameters)}({path})");
			return null;
		}

		protected override PSDriveInfo NewDrive(PSDriveInfo drive)
		{
			WriteTrace($"Entering {nameof(NewDrive)}");
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
			WriteTrace($"Entering {nameof(NormalizeRelativePath)}({path}, {basePath})");
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
			WriteTrace($"Entering {nameof(RemoveDrive)}");
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
				return categoryNames;
			}
			var regexString = Regex.Escape(path).Replace(@"\\", "").Replace("*", ".*");
			var regex = new Regex($"^{regexString}$");
			return categoryNames.Where(fln => regex.IsMatch(fln)).Select(fln => fln).ToArray();
		}

		private static List<PcfAppInfo> GetApps(string container = appsCategory) => currentDriveInfo.Connection.GetAllApps(container);

		private static List<PcfDomainInfo> GetDomains(string organization) => currentDriveInfo.Connection.GetAllDomains(organization);

		private static List<PcfManagerInfo> GetManagers(string organization) => currentDriveInfo.Connection.GetAllManagers(organization);

		private static List<PcfOrganizationInfo> GetOrganizations(string container = orgsCategory) => currentDriveInfo.Connection.GetAllOrganizations(container);

		private static List<PcfRouteMapping> GetRouteMappings(string container = routeMappingsCategory) => currentDriveInfo.Connection.GetAllRouteMappings(container);

		private static List<PcfRouteInfo> GetRoutes(string container = routesCategory) => currentDriveInfo.Connection.GetAllRoutes(container);

		private static List<PcfServiceInfo> GetServices(string container = servicesCategory) => currentDriveInfo.Connection.GetAllServices(container);

		private static List<PcfSpaceInfo> GetSpaces(string container = spacesCategory) => currentDriveInfo.Connection.GetAllSpaces(container);

		private static List<PcfStackInfo> GetStacks(string container = stacksCategory) => currentDriveInfo.Connection.GetAllStacks(container);

		private static List<PcfUserInfo> GetUsers(string organization) => currentDriveInfo.Connection.GetAllUsers(organization);

		private static string MakeChildPathname(string parentPath, string childName)
		{
			return $"{parentPath.TrimEnd(new[] { '\\' })}{PcfDriveInfo.PathSeparator}{childName}";
		}

		private string FirstLevelObject(string value) => value;

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

		private Func<IEnumerable<InfoBase.PcfInfo>> GetFunctionForCategory(string category)
		{
			if (!categoriesInfo.ContainsKey(category))
			{
				WriteErrorRecord(
					new ArgumentNullException(nameof(category)),
					$"'{category}' is an unsupported category",
					ErrorCategory.InvalidArgument,
					null);
				return () => new List<InfoBase.PcfInfo>();
			}
			return categoriesInfo[category].Function;
		}

		private (string[] containerNames, PathType level, int count, string category, string containerName, string entityName, string subCategory, string subEntity) GetPathParts(string path)
		{
			var containers = GetContainerNames(path);
			var count = containers.ContainerNames.Length;
			var level = containers.Level;
			var category = count > 0 ? containers.ContainerNames[0] : "";
			var containerName = count > 0 ? containers.ContainerNames.Last() : "";
			var entityName = count > 1 ? containers.ContainerNames[1] : "";
			var subCategory = count > 2 ? containers.ContainerNames[2] : "";
			var subEntity = count > 3 ? containers.ContainerNames[3] : "";
			return (containers.ContainerNames, level, count, category, containerName, entityName, subCategory, subEntity);
		}

		private IEnumerable<InfoBase.PcfInfo> GetSubentityMatches(string subentity, IEnumerable<IEnumerable<InfoBase.PcfInfo>> entities)
		{
			return entities.SelectMany(ps => ps).ToList().FindAll(s => s.Name == subentity);
		}

		private IEnumerable<InfoBase.PcfInfo> GetSubentityMatches(string subentity, IEnumerable<InfoBase.PcfInfo> entities)
		{
			return entities.ToList().FindAll(s => s.Name == subentity);
		}

		private T LogReturn<T>(Func<T> function, [CallerMemberName]string callerName = "")
		{
			T returnValue = function();
			if (isLogItems)
			{
				_trace.WriteLineIndent($"Returned '{returnValue}'", callerName);
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

		private void RecurseForName(IEnumerable<IEnumerable<InfoBase.PcfInfo>> entity, string path, bool recurse)
		{
			foreach (var es in entity)
			{
				foreach (var e in es)
				{
					WriteItemObject(e, path, false);
					if (recurse)
					{
						GetChildItems(MakeChildPathname(path, e.Name), recurse);
					}
				}
			}
		}

		private void RecurseForName(IEnumerable<InfoBase.PcfInfo> entity, string path, bool recurse)
		{
			foreach (var e in entity)
			{
				WriteItemObject(e, path, false);
				if (recurse)
				{
					GetChildItems(MakeChildPathname(path, e.Name), recurse);
				}
			}
		}

		private string RemoveDriveFromPath(string path) => path.Replace($"{currentDriveInfo.Root}:", string.Empty);

		private void WriteAllItems(IEnumerable<InfoBase.PcfInfo> items, string path, bool isContainer = false, bool isFullItem = false)
		{
			foreach (var item in items)
			{
				if (isFullItem)
				{
					WriteItemObject(item, path, isContainer);
				}
				else
				{
					WriteItemObject(item.Name, path, isContainer);
				}
			}
		}

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

		private void WriteTrace(string message, bool isBlankBefore = false)
		{
			_trace.WriteLine(message, isBlankBefore: isBlankBefore);
			try
			{
				WriteDebug(message);
			}
			catch (Exception genEx)
			{
				_trace.WriteLine($"{genEx.Message}\r\nStack Trace: {genEx.StackTrace}");
			}
		}
	}
}