using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using TarCompression;
using TinyJson;
using System.Reflection;

namespace KwRuntime_Installer
{
	public class Form1 : Form
	{
		private StringBuilder sb = new StringBuilder();

		private IContainer components;

		private TextBox textBox1;

		private Label label1;

		private PictureBox pictureBox1;

		private Panel panel1;

		public static bool IsLinux
		{
			get
			{
				int platform = (int)Environment.OSVersion.Platform;
				if (platform != 4 && platform != 6)
				{
					return platform == 128;
				}
				return true;
			}
		}

		public Form1()
		{
			InitializeComponent();
			sb.AppendLine("Starting installation...");
			textBox1.Text = sb.ToString();
			textBox1.HideSelection = true;
			base.Shown += delegate
			{
				new Thread(SecureDownload).Start();
			};
		}

		private void SecureDownload()
		{
			int num = 3;
			while (true)
			{
				try
				{
					Download();
					break;
				}
				catch (Exception ex)
				{
					SecureAppendLine("[ERROR] " + ex.Message);
					SecureAppendLine("INSTALL FAILED");
					num--;
					if (num > 0)
					{
						SecureAppendLine("Retrying install ...");
						Thread.Sleep(2000);
						sb.Clear();
						continue;
					}
					break;
				}
			}
		}

		public static bool InternalCheckIsWow64()
		{
			if ((Environment.OSVersion.Version.Major != 5 || Environment.OSVersion.Version.Minor < 1) && Environment.OSVersion.Version.Major < 6)
			{
				return false;
			}
			using( Process process = Process.GetCurrentProcess()){
				bool wow64Process;
				return IsWow64Process(process.Handle, out wow64Process) && wow64Process;
			}
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool IsWow64Process([In] IntPtr hProcess, out bool wow64Process);

		private void DownloadFile(Uri uri, string path)
		{
		}

		private void Download()
		{
			ServicePointManager.Expect100Continue = true;
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			string text = Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE") ?? Environment.GetEnvironmentVariable("HOME"), "KwRuntime");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			string path = Path.Combine(text, "bin");
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			string text2 = Path.Combine(text, "node");
			if (!Directory.Exists(text2))
			{
				Directory.CreateDirectory(text2);
			}
			string text3 = Path.Combine(text, "runtime");
			if (!Directory.Exists(text3))
			{
				Directory.CreateDirectory(text3);
			}
			Uri address = new Uri("https://raw.githubusercontent.com/kwruntime/core/main/install.info.json");
			string json = "";
			using (WebClient webClient = new WebClient())
			{
				json = webClient.DownloadString(address);
			}
			Dictionary<string, object> dictionary = json.FromJson<Dictionary<string, object>>();
			string key = (IsLinux ? "linux" : "win32");
			string text4 = (IsLinux ? "" : ".exe");
			Dictionary<string, object> dictionary2 = (Dictionary<string, object>)dictionary[key];
			string text5 = "x64";
			if (!IsLinux && IntPtr.Size != 8 && !InternalCheckIsWow64())
			{
				text5 = "x86";
			}
			Dictionary<string, object> dictionary3 = (Dictionary<string, object>)((List<object>)((Dictionary<string, object>)dictionary2["node"])[text5])[0];
			string text6 = (string)dictionary3["version"];
			string text7 = (string)dictionary3["href"];
			SecureAppendLine("URL:" + text7);
			SecureAppend("Downloading node " + text5 + "-" + text6 + " ...  ");
			string text8 = Path.Combine(text2, (string)dictionary3["name"]);
			using (WebClient webClient2 = new WebClient())
			{
				if (File.Exists(text8))
				{
					File.Delete(text8);
				}
				webClient2.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
				webClient2.Headers.Add("user-agent", "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
				webClient2.DownloadFile(new Uri(text7), text8);
				Thread.Sleep(1000);
			}
			SecureAppendLine("Extracting node ...  ");
			Tar.ExtractTarGz(text8, text2);
			SecureAppendLine("Downloading runtime ...  ");
			List<object> list = (List<object>)dictionary2["files"];
			if (list != null)
			{
				foreach (object item in list)
				{
					Dictionary<string, object> dictionary4 = (Dictionary<string, object>)item;
					string text9 = Path.Combine(text, (string)dictionary4["path"]);
					using(WebClient webClient3 = new WebClient()){						
						
						SecureAppendLine(" - " + dictionary4["href"]);
						webClient3.Headers.Add("user-agent", "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
						webClient3.DownloadFile((string)dictionary4["href"], text9);
					}
					if (dictionary4.ContainsKey("compression"))
					{
						if (!((string)dictionary4["compression"] == "tar+gz"))
						{
							throw new Exception("Compression type not supported");
						}
						Tar.ExtractTarGz(text9, Path.GetDirectoryName(text9));
						File.Delete(text9);
					}
				}
			}
			if (!IsLinux)
			{
				
				//File.WriteAllBytes(Path.Combine(text3, "default_executable.dll"), Convert.FromBase64String(data.Kawix_bytes));
				//File.WriteAllBytes(Path.Combine(text3, "default_gui_executable.dll"), Convert.FromBase64String(data.Kawix_Gui_bytes));
			}
			SecureAppendLine("Finishing installation");
			if (IsLinux)
			{
				string text10 = Path.Combine(text2, text5, text6, "node" + text4);
				Process process = new Process();
				process.StartInfo.FileName = "chmod";
				process.StartInfo.Arguments = "+x \"" + text10 + "\"";
				process.OutputDataReceived += delegate(object sender, DataReceivedEventArgs args)
				{
					AppendLine(args.Data);
				};
				process.ErrorDataReceived += delegate(object sender, DataReceivedEventArgs args)
				{
					AppendLine(args.Data);
				};
				process.Start();
				process.WaitForExit();
			}
			Process obj = new Process
			{
				StartInfo = 
				{
					UseShellExecute = false,
					RedirectStandardError = true,
					RedirectStandardOutput = true,
					CreateNoWindow = true,
					FileName = Path.Combine(text2, text5, text6, "node" + text4),
					Arguments = "\"" + Path.Combine(text3, "kwruntime.js") + "\" --self-install"
				}
				};
			StringBuilder sb = new StringBuilder();
			obj.OutputDataReceived += delegate(object sender, DataReceivedEventArgs args)
			{
				SecureAppendLine(args.Data);
				sb.AppendLine(args.Data);
			};
			obj.ErrorDataReceived += delegate(object sender, DataReceivedEventArgs args)
			{
				SecureAppendLine(args.Data);
				sb.AppendLine(args.Data);
			};
			obj.Start();
			obj.BeginErrorReadLine();
			obj.BeginOutputReadLine();
			obj.WaitForExit();
			string text11 = sb.ToString();
			if (text11.ToUpper().IndexOf("STACK") >= 0 && text11.ToUpper().IndexOf("ERROR") >= 0)
			{
				SecureAppendLine("INSTALL FAILED");
			}
			else
			{
				SecureAppendLine("¡INSTALL COMPLETED!");
			}
		}

		private void AutoClose()
		{
			Thread.Sleep(10000);
			Close();
		}

		private void SecureAppendLine(string s)
		{
			Invoke((MethodInvoker)delegate
				{
					AppendLine(s);
				});
		}

		private void AppendLine(string s)
		{
			sb.AppendLine(s);
			textBox1.Text = sb.ToString();
			textBox1.DeselectAll();
			textBox1.SelectionStart = textBox1.Text.Length;
			textBox1.ScrollToCaret();
		}

		private void SecureAppend(string s)
		{
			Invoke((MethodInvoker)delegate
				{
					Append(s);
				});
		}

		private void Append(string s)
		{
			sb.Append(s);
			textBox1.Text = sb.ToString();
			textBox1.DeselectAll();
			textBox1.SelectionStart = textBox1.Text.Length;
			textBox1.ScrollToCaret();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			
			//base.Icon = Icon.ExtractAssociatedIcon(Assembly.GetCallingAssembly().Location);
			Icon = Icon.FromHandle(Installer.Resource1.pictureBox1_Image.GetHicon());
			this.panel1 = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.pictureBox1).BeginInit();
			base.SuspendLayout();
			this.panel1.BackColor = System.Drawing.Color.White;
			this.panel1.Controls.Add(this.label1);
			this.panel1.Controls.Add(this.pictureBox1);
			this.panel1.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
			this.panel1.Location = new System.Drawing.Point(-1, -3);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(517, 177);
			this.panel1.TabIndex = 0;
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.label1.Location = new System.Drawing.Point(240, 67);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(208, 42);
			this.label1.TabIndex = 1;
			this.label1.Text = "KwRuntime";
			this.pictureBox1.ErrorImage = null;
			this.pictureBox1.Image = Installer.Resource1.pictureBox1_Image;
			this.pictureBox1.Location = new System.Drawing.Point(78, 26);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(129, 131);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBox1.BackColor = System.Drawing.Color.WhiteSmoke;
			this.textBox1.Font = new System.Drawing.Font("Consolas", 9.6f);
			this.textBox1.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
			this.textBox1.Location = new System.Drawing.Point(17, 193);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(484, 127);
			this.textBox1.TabIndex = 1;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.WhiteSmoke;
			base.ClientSize = new System.Drawing.Size(515, 334);
			base.Controls.Add(this.textBox1);
			base.Controls.Add(this.panel1);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "Form1";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "KwRuntime Installer";
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.pictureBox1).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
