using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace KwRuntime_Installer
{
	
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class data
	{
		private static ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (object.Equals(null, resourceMan))
				{
					resourceMan = new ResourceManager("test03.data", typeof(data).Assembly);
				}
				return resourceMan;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				
				return resourceCulture;
			}
			set
			{
				resourceCulture = value;
			}
		}

		internal static Icon this_Icon => (Icon)ResourceManager.GetObject("this.Icon", resourceCulture);

		internal static Bitmap pictureBox1_Image => (Bitmap)ResourceManager.GetObject("pictureBox1.Image", resourceCulture);

		internal static string Kawix_bytes => ResourceManager.GetString("Kawix.bytes", resourceCulture);

		internal static string Kawix_Gui_bytes => ResourceManager.GetString("Kawix.Gui.bytes", resourceCulture);

		internal data()
		{
		}
	}
}
