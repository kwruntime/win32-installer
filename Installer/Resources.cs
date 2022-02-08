﻿using System;
using System.Drawing;
using System.IO;

namespace Installer
{
	public class Resources
	{
		public static Bitmap  pictureBox1_Image{
			get{
				var base64 = "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";
				var bytes = Convert.FromBase64String (base64);
				//var b = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter ();
				using (var ms = new MemoryStream (bytes)) {
					//return (Bitmap)b.Deserialize (ms);
					return new Bitmap(new MemoryStream(bytes));
				}

			}
		}
	}
}

