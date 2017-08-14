using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TestPcfProvider
{
	internal class CommandFileReader
	{
		public CommandFileReader(string pathToFile)
		{
			this.pathToFile = pathToFile;
			Commands = string.IsNullOrEmpty(pathToFile) ? GetInternalScript() : GetExternalScript(pathToFile);
		}

		public List<string> Commands { get; }

		private object pathToFile;

		private List<string> GetExternalScript(string pathToFile)
		{
			if (!File.Exists(pathToFile))
			{
				throw new FileNotFoundException("Missing file", pathToFile);
			}
			return File.ReadAllLines(pathToFile).ToList();
		}

		private List<string> GetInternalScript()
		{
			return new List<string> {
				"Get-ChildItem",
				@"Import-Module -Name '.\PcfProvider'",
				"Get-PSProvider",
				"New-PSDrive PCF -PSProvider 'Pcf' -Root '\' -Uri run.pivotal.io -UserName (Get-Content 'user.txt') -Password (Get-Content 'password.txt') #-IsLogItems"
			};
		}
	}
}