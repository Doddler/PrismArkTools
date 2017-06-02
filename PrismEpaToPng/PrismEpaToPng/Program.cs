using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrismEpaToPng
{
	class Program
	{
		static void Decode(byte[] src, byte[] dest, int width)
		{
			var offsets = new int[16]
			{
				0, 1, width, width + 1,
				2, width - 1, width << 1, 3,
				(width << 1) + 2, width + 2, (width << 1) + 1, (width << 1) - 1,
				(width << 1) - 2, width - 2, width*3, 4
			};

			var srcptr = 0;
			var dstptr = 0;

			while ((srcptr < src.Length) && (dstptr < dest.Length))
			{
				var code = src[srcptr++];
				int length = code & 0x07;

				if ((code & 0xF0) != 0)
				{
					if ((code & 0x08) != 0)
					{
						length = (length << 8) + src[srcptr++];
					}

					if (length != 0)
					{
						code >>= 4;

						var back = dstptr - offsets[code];

						if (dstptr + length > dest.Length)
							length = dest.Length - dstptr;

						for (var i = 0; i < length; i++)
							dest[dstptr + i] = dest[back + i];

						dstptr += length;
					}
				}
				else if (code != 0)
				{
					Buffer.BlockCopy(src, srcptr, dest, dstptr, code);
					srcptr += code;
					dstptr += code;
				}
			}
		}

		static void Convert(string inputPath, string outputPath)
		{
			var bytes = File.ReadAllBytes(inputPath);
			var ms = new MemoryStream(bytes);
			var br = new BinaryReader(ms);

			var sig = br.ReadChars(2);
			if (sig[0] != 'E' && sig[1] != 'P')
				throw new FileLoadException("File is not a valid EPA file!");

			ms.Seek(3, SeekOrigin.Begin);

			var flag = br.ReadByte();
			var bppflag = br.ReadInt32();
			var width = br.ReadInt32();
			var height = br.ReadInt32();

			var bpp = 8;

			if (bppflag == 1)
				bpp = 24;
			if (bppflag == 2)
				bpp = 32;

			if (flag != 1)
				throw new Exception("Cannot process file, no handler for epa types other than type 1");

			var pal = new Color[256];

			var firstr = 0;
			var firstg = 0;
			var firstb = 0;

			if (bpp == 8)
			{
				for (var i = 0; i < 256; i++)
				{
					var b = br.ReadByte();
					var g = br.ReadByte();
					var r = br.ReadByte();

					if (i == 0)
					{
						pal[i] = Color.FromArgb(0, r, g, b);
						firstr = r;
						firstg = g;
						firstb = b;
					}
					else
					{
						if(firstr == r && firstg == g && firstb == b)
							pal[i] = Color.FromArgb(0, r, g, b);
						else
							pal[i] = Color.FromArgb(255, r, g, b);
					}
				}
			}

			var srcsize = bytes.Length - 16;
			var destsize = width * height * (bpp >> 3);

			if (bpp == 8)
				srcsize -= 256 * 3;

			var sourcedata = new byte[srcsize];
			var destdata = new byte[destsize];

			Buffer.BlockCopy(bytes, (int)ms.Position, sourcedata, 0, srcsize);

			Decode(sourcedata, destdata, width);

			var bmp = new Bitmap(width, height);

			var sectionlen = width*height;

			for (var x = 0; x < width; x++)
			{
				for (var y = 0; y < height; y++)
				{
					if (bpp == 8)
					{
						var index = destdata[y * width + x];
						bmp.SetPixel(x, y, pal[index]);
					}
					else
					{
						var b = destdata[(y * width + x)];
						var g = destdata[(y * width + x) + sectionlen];
						var r = destdata[(y * width + x) + sectionlen * 2];

						var a = 255;
						if (bpp == 32)
							a = destdata[(y * width + x) + sectionlen * 3];

						bmp.SetPixel(x, y, Color.FromArgb(a, r, g, b));
					}
				}
			}

			bmp.Save(outputPath, ImageFormat.Png);

			//File.WriteAllBytes(outputPath, destdata);
		}

		static void Main(string[] args)
		{
			//Convert(@"D:\Projects\prismark\Park_tools_v2\park_tools\archive2\prism_name_text01.EPA",
			//	@"D:\Projects\prismark\Park_tools_v2\park_tools\prism_name_text01.png");
			//foreach (var f in Directory.GetFiles(@"D:\Dropbox\Mangagamer\PrismArk\epa\", "*.epa"))
			//{
			//	var root = Path.GetFileNameWithoutExtension(f);
			//	var outpath = Path.Combine(Path.GetDirectoryName(f), root + ".png");

			//	Convert(f, outpath);
			//}

			if (args.Length != 2)
			{
				Console.WriteLine("Usage: PrismEpaToPng (Source File) (Destination File)");
				return;
			}

			Convert(args[0], args[1]);
		}
	}
}
