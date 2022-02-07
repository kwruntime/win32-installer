using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace TarCompression
{
	public class Tar
	{
		public static void ExtractTar(string filename, string outputDir)
		{
			using FileStream stream = File.OpenRead(filename);
			ExtractTar(stream, outputDir);
		}

		public static void ExtractTar(Stream stream, string outputDir)
		{
			byte[] array = new byte[100];
			while (true)
			{
				stream.Read(array, 0, 100);
				string text = Encoding.ASCII.GetString(array).Trim(new char[1]);
				if (string.IsNullOrEmpty(text))
				{
					break;
				}
				stream.Seek(24L, SeekOrigin.Current);
				stream.Read(array, 0, 12);
				long num = Convert.ToInt64(Encoding.UTF8.GetString(array, 0, 12).Trim(new char[1]).Trim(), 8);
				stream.Seek(376L, SeekOrigin.Current);
				string path = Path.Combine(outputDir, text);
				if (!Directory.Exists(Path.GetDirectoryName(path)))
				{
					Directory.CreateDirectory(Path.GetDirectoryName(path));
				}
				if (!text.EndsWith("/"))
				{
					using FileStream fileStream = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write);
					byte[] array2 = new byte[num];
					stream.Read(array2, 0, array2.Length);
					fileStream.Write(array2, 0, array2.Length);
				}
				long position = stream.Position;
				long num2 = 512 - position % 512;
				if (num2 == 512)
				{
					num2 = 0L;
				}
				stream.Seek(num2, SeekOrigin.Current);
			}
		}

		public static void ExtractTarGz(string filename, string outputDir)
		{
			using FileStream stream = File.OpenRead(filename);
			ExtractTarGz(stream, outputDir);
		}

		public static void ExtractTarGz(Stream stream, string outputDir)
		{
			using GZipStream gZipStream = new GZipStream(stream, CompressionMode.Decompress);
			using MemoryStream memoryStream = new MemoryStream();
			byte[] buffer = new byte[4096];
			int num;
			do
			{
				num = gZipStream.Read(buffer, 0, 4096);
				memoryStream.Write(buffer, 0, num);
			}
			while (num == 4096);
			memoryStream.Seek(0L, SeekOrigin.Begin);
			ExtractTar(memoryStream, outputDir);
		}
	}
}
