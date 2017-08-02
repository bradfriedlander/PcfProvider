using System;
using System.Management.Automation;
using static System.String;

namespace PcfProvider
{
	internal class PcfDriveInfo : PSDriveInfo
	{
		public PcfDriveInfo(PSDriveInfo driveInfo, string uri, bool isLocal) : base(driveInfo)
		{
			Uri = uri;
			DriveInfo = driveInfo;
			Connection = new PcfConnection(driveInfo, uri, isLocal);
		}

		public PcfConnection Connection { get; }

		public PSDriveInfo DriveInfo { get; }

		public string Password { get; set; }

		public string Uri { get; set; }

		public string UserName { get; set; }

		public const string PathSeparator = @"\";

		/// <summary>
		///     Checks if a given path is actually a drive name.
		/// </summary>
		/// <param name="path">The path to check.</param>
		/// <returns><c>true</c> if <paramref name="path" /> represents a drive; otherwise <c>false</c>.</returns>
		public bool PathIsDrive(string path)
		{
			return
				path.Equals(@"\")
				|| IsNullOrEmpty(path.Replace($"{DriveInfo.Root}:", Empty))
				|| IsNullOrEmpty(path.Replace($"{DriveInfo.Root}{PathSeparator}:", Empty));
		}
	}
}