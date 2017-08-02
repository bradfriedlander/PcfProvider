using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection;
using PcfProvider;

namespace TestPcfProvider
{
	public static class TesterProgram
	{
		public static void Main(string[] args)
		{
			Collection<PSObject> results = null;
			var powerShellHost = PowerShell.Create();
			if (System.Diagnostics.Debugger.IsAttached)
			{
				var NameToForceLoad = typeof(PcfPSProvider).Assembly.FullName;
			}
			try
			{
				var commandReader = new CommandFileReader(args[0] ?? string.Empty);
				foreach (var cmd in commandReader.Commands)
				{
					var pipeline = powerShellHost.Runspace.CreatePipeline();
					pipeline.Commands.Add(new Command(cmd, true, false));
					WriteMsg($"Invoking '{cmd}'");
					results = pipeline.Invoke();
					var stateInfo = pipeline.PipelineStateInfo;
					WriteMsg($"=> StateInfo: {stateInfo.State.ToString()}: {stateInfo.Reason}");
					var errorReader = pipeline.Error;
					while(!errorReader.EndOfPipeline)
					{
						var error = errorReader.Read();
						WriteMsg($"=> Error: {error}");
					}
					var outputReader = pipeline.Output;
					while (!outputReader.EndOfPipeline)
					{
						var output = outputReader.Read();
						WriteMsg($"=> Output: {output}");
					}
					if (0 == results.Count)
					{
						WriteMsg("=X No results from command");
						continue;
					}
					foreach (var res in results)
					{
						WriteMsg($"=> { res}");
					}
				}
			}
			catch (Exception genEx)
			{
				WriteMsg($"Exception: {genEx.Message}\r\nStack Trace: {genEx.StackTrace}");
			}
			Console.Write("Completed all commands.");
			Console.ReadKey();
		}

		private static void WriteMsg(string message)
		{
			Console.WriteLine(message);
		}
	}
}