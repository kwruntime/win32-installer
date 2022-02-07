using System;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace KwRuntime_Installer
{
	public class FileTypes
	{
		[DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);

		public static void SetAssociation(string extension, string keyName, string openWith, string fileDescription)
		{
			RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\Classes", writable: true)?.CreateSubKey(extension);
			registryKey?.SetValue("", keyName);
			RegistryKey registryKey2 = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Classes", writable: true)?.CreateSubKey(keyName);
			registryKey2?.SetValue("", fileDescription);
			registryKey2?.CreateSubKey("DefaultIcon")?.SetValue("", "\"" + openWith + "\",0");
			RegistryKey obj = registryKey2?.CreateSubKey("Shell");
			obj?.CreateSubKey("edit")?.CreateSubKey("command")?.SetValue("", "\"" + openWith + "\" \"%1\"");
			obj?.CreateSubKey("open")?.CreateSubKey("command")?.SetValue("", "\"" + openWith + "\" \"%1\"");
			registryKey?.Close();
			registryKey2?.Close();
			obj?.Close();
			RegistryKey registryKey3 = null;
			try
			{
				registryKey3 = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\" + extension, writable: true);
				registryKey3?.DeleteSubKey("UserChoice", throwOnMissingSubKey: false);
			}
			catch (Exception)
			{
			}
			finally
			{
				registryKey3?.Close();
			}
			SHChangeNotify(134217728u, 0u, IntPtr.Zero, IntPtr.Zero);
		}
	}
}
