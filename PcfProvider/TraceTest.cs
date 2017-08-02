using System.Diagnostics;

namespace PcfProvider
{
	public class TraceTest
	{
		public TraceTest(string area, string application)
		{
			Application = application;
			Area = area;
			Activated = true;
		}

		public bool Activated { get; private set; }

		public string Application { get; }

		public string Area { get; }

		public void SetActivated(bool activated) => Activated = activated;

		public void WriteLine(string message, string area = null, string application = null)
		{
			if (Activated)
			{
				Trace.WriteLine($"{application ?? Application}: {area ?? Area}: {message}");
			}
		}
	}
}