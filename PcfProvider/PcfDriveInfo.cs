using System;
using System.Management.Automation;
using static System.String;

namespace PcfProvider
{
	internal class PcfDriveInfo : PSDriveInfo
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="PcfDriveInfo" /> class.
		/// </summary>
		/// <param name="driveInfo">This is the drive information as provided by PowerShell.</param>
		/// <param name="uri">This is the URI of the PCF site.</param>
		/// <param name="isLocal">If set to <c>true</c>, the drive is on the local (PcfDev) <inheritdoc /> of PCF.</param>
		/// <param name="tracer">This is the class supporting diagnostic tracing.</param>
		public PcfDriveInfo(PSDriveInfo driveInfo, string uri, bool isLocal, TraceTest tracer) : base(driveInfo)
		{
			Uri = uri;
			DriveInfo = driveInfo;
			Tracer = tracer;
			Connection = new PcfConnection(driveInfo, uri, isLocal, tracer);
		}

		public PcfConnection Connection { get; }

		/// <summary>
		///     This gets the drive information provided by PowerShell.
		/// </summary>
		/// <value>The drive information.</value>
		public PSDriveInfo DriveInfo { get; }

		public TraceTest Tracer { get; }

		/// <summary>
		///     Gets or sets the URI of the PCF site.
		/// </summary>
		/// <value>This is the URI of the PCF site.</value>
		public string Uri { get; set; }

		/// <summary>
		///     Gets or sets the name of the user of the PCF site.
		/// </summary>
		/// <value>This is the name of the user.</value>
		public string UserName { get; set; }

		/// <summary>
		///     This is the path separator used by this provider.
		/// </summary>
		public const string PathSeparator = @"\";

		/// <summary>
		///     Checks if a given path is actually a drive name.
		/// </summary>
		/// <param name="path">The path to check.</param>
		/// <returns><c>true</c> if <paramref name="path" /> represents a drive; otherwise <c>false</c>.</returns>
		public bool PathIsDrive(string path)
		{
			return
				path.Equals(PathSeparator)
				|| IsNullOrEmpty(path.Replace($"{DriveInfo.Root}:", Empty))
				|| IsNullOrEmpty(path.Replace($"{DriveInfo.Root}{PathSeparator}:", Empty));
		}
	}
}