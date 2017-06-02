using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrismPngToEpa
{
	class Program
	{
		static byte[] GetPixelData(Bitmap bmp, int depth)
		{
			byte[] bytes = null;

			if (depth == 24)
				bytes = new byte[bmp.Width * bmp.Height * 3];
			if (depth == 32)
				bytes = new byte[bmp.Width * bmp.Height * 4];
			if (bytes == null)
				throw new Exception("Unexpected bit depth " + depth);

			var width = bmp.Width;
			var height = bmp.Height;

			var sectionlen = width * height;

			for (var x = 0; x < width; x++)
			{
				for (var y = 0; y < height; y++)
				{
					var pixel = bmp.GetPixel(x, y);
					bytes[y * width + x] = pixel.B;
					bytes[y * width + x + sectionlen] = pixel.G;
					bytes[y * width + x + sectionlen * 2] = pixel.R;

					if (depth == 32)
						bytes[y * width + x + sectionlen * 3] = pixel.A;
				}
			}

			return bytes;
		}

		static Tuple<int, int> GetRle(byte[] input, int[] offsets, int srcptr)
		{
			var match = -1;
			var matchlen = -1;

			//loop through all the valid offsets for source bytes
			for (var i = 0; i < offsets.Length; i++)
			{
				var src2 = srcptr - offsets[i];

				//obviously the source is invalid if it's out of bounds or both source and dest are equal
				if (src2 < 0 || src2 == srcptr)
					continue;

				//if the byte at the offset doesn't match the byte at the current point, it's no good
				if (input[srcptr] != input[src2])
					continue;

				var dst2 = srcptr;
				var len = 0;

				//count how many bytes at this offset match the next set of bytes
				while (dst2 < input.Length && input[src2] == input[dst2] && len < 0x7FF)
				{
					len++;
					src2++;
					dst2++;
				}

				//if it's a longer match than the last match, mark that as the longest match
				if (len > matchlen)
				{
					match = i;
					matchlen = len;
				}
			}
			return new Tuple<int, int>(match, matchlen);
		}

		static byte[] Compress(byte[] input, int width)
		{
			var bytes = new byte[input.Length*3]; //this should be safe...
			
			//the list of offset positions we're going to check for matches
			var offsets = new int[16]
			{
				0, 1, width, width + 1,
				2, width - 1, width << 1, 3,
				(width << 1) + 2, width + 2, (width << 1) + 1, (width << 1) - 1,
				(width << 1) - 2, width - 2, width*3, 4
			};

			var srcptr = 0;
			var dstptr = 0;

			//do compression here

			//loop through the entire source bytes
			while (srcptr < input.Length)
			{
				//get the length and type of the longest rle match
				var res = GetRle(input, offsets, srcptr);

				var match = res.Item1;
				var matchlen = res.Item2;
				
				//if we've found a match
				if (match != -1)
				{
					//the offset is stored in the top 4 bits
					byte code = (byte) (match << 4);
					if (matchlen < 8)
					{
						//we can fit everything in one byte
						code |= (byte) matchlen;
						bytes[dstptr++] = code;
					}
					else
					{
						//we need two bytes. Mark the flag accordingly
						code |= (byte) 0x8;
						//we can store up to 11 bits for length, the upper 3 bits are stored with the code
						code |= (byte) (matchlen >> 8);

						//output both bytes
						bytes[dstptr++] = code;
						bytes[dstptr++] = (byte) (matchlen & 0xFF);
					}

					//advance the source pointer
					srcptr += matchlen;
				}
				else
				{
					//we can't copy from the buffer, so we gotta output bytes directly.
					//count how many bytes we need to count...
					var src2 = srcptr;
					var count = 0;
					while (src2 < input.Length && count < 0xF)
					{
						var rle = GetRle(input, offsets, src2);
						if (rle.Item1 != -1 && rle.Item2 > 1)
							break;
						src2++;
						count++;
					}
					

					bytes[dstptr++] = (byte)count; //code flag of number of bytes to copy
					for(var i = 0; i < count; i++)
						bytes[dstptr++] = input[srcptr++]; //and the value of the byte
				}
			}

			var byteout = new byte[dstptr];
			Buffer.BlockCopy(bytes, 0, byteout, 0, dstptr);

			return byteout;
		}

		static void Convert(string inputpath, string outputpath)
		{
			var bmp = new Bitmap(inputpath);

			var ms = new MemoryStream();
			var br = new BinaryWriter(ms);

			br.Write(new[] { 'E', 'P' });
			br.Write((byte)0x1);
			br.Write((byte)0x1);

			var depth = 1;

			switch (bmp.PixelFormat)
			{
				case PixelFormat.Format24bppRgb:
					depth = 24;
					br.Write(1);
					break;
				case PixelFormat.Format32bppArgb:
					depth = 32;
					br.Write(2);
					break;
				default:
					throw new Exception("Unexpected pixel format: " + bmp.PixelFormat);
			}

			br.Write(bmp.Width);
			br.Write(bmp.Height);

			var inbytes = GetPixelData(bmp, depth);

			br.Write(Compress(inbytes, bmp.Width));

			File.WriteAllBytes(outputpath, ms.ToArray());
		}

		static void Main(string[] args)
		{
			//Convert(@"D:\Dropbox\Mangagamer\PrismArk\epa\sys_title_logo_m2.png",
			//	@"D:\Dropbox\Mangagamer\PrismArk\epa\sys_title_logo_m2.EPA");

			if (args.Length != 2)
			{
				Console.WriteLine("Usage: PrismPngToEpa (Source File) (Destination File)");
				return;
			}

			Convert(args[0], args[1]);
		}
	}
}
