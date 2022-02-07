using System;
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text;
using System.IO;

namespace KwRuntime_Installer
{
	internal static class Program
	{


		private static int RunAsGuiForBat(string[] args){

			string location = Assembly.GetExecutingAssembly().Location.ToLower();
			int num = location.LastIndexOf("-gui.exe");
			string text = "";
			bool flag = true;
			if (num >= 0)
			{
				text = location.Substring(0, num);
			}
			else
			{
				flag = false;
				text = location.Substring(0, location.Length - 4);
			}
			Environment.SetEnvironmentVariable("GUI_MODE", "1");
			Environment.SetEnvironmentVariable("FORCE_COLOR", "2");
			string text2 = text + ".exe";
			if (!flag || !File.Exists(text2))
			{
				text2 = text + ".bat";
				if (!File.Exists(text2))
				{
					text2 = text + ".cmd";
				}
			}
			Process process = new Process();
			process.StartInfo.FileName = text2;
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string text3 in args)
			{
				stringBuilder.Append("\"" + text3 + "\" ");
			}
			if (stringBuilder.Length > 0)
			{
				process.StartInfo.Arguments = stringBuilder.ToString();
			}
			process.StartInfo.UseShellExecute = false;
			if (flag)
			{
				process.StartInfo.CreateNoWindow = true;
			}
			process.EnableRaisingEvents = true;
			process.Start();
			process.WaitForExit();
			return process.ExitCode;

		}

		[STAThread]
		private static int Main(string[] args)
		{
			
			string location = Assembly.GetExecutingAssembly().Location.ToLower();
			if (location.EndsWith ("-gui.exe")) {
				return RunAsGuiForBat (args);
			}
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run((Form)(object)new Form1());
			return 0;
		}
	}
}
