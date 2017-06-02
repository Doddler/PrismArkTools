using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrismBattleUnpacker
{
	struct FileEntry
	{
		public string Filename;
		public byte Key;
		public int UnpackedSize;
		public int FileOffset;
		public int PackedSize;

		public void Read(BinaryReader br)
		{
			
		}
	}

	struct Index
	{
		public int EntryOffset;
		public int FileOffset;
		public int PackedSize;

		public void Read(BinaryReader br)
		{
			EntryOffset = br.ReadInt32();
			FileOffset = br.ReadInt32();
			PackedSize = br.ReadInt32();
		}
	}

	class Program
	{
		static void DecryptBlock(byte[] buff, int size, byte key, int keyoffset)
		{
			if (size <= 0)
				return;

			for (var i = 0; i < size; i++)
			{
				buff[i] -= key;
				buff[i] = SubTable.Table[keyoffset * 256 + buff[i]];
				keyoffset++;
				keyoffset %= 6;
			}
		}

		static void DecompressBlock(byte[] dest, byte[] src, int srcptr)
		{
			ushort mask = 0;
			byte skipbits = 0;
			ushort sub = 0;
			
			var destptr = 0;

			var blockflag = src[srcptr] & 0xC0;
			switch (blockflag)
			{
				case 0x00:
					mask = 0x3FF;
					skipbits = 0xA;
					sub = 0x400;
					break;
				case 0x40:
					mask = 0x7FF;
					skipbits = 0xB;
					sub = 0x800;
					break;
				case 0x80:
					mask = 0xFFF;
					skipbits = 0xC;
					sub = 0x1000;
					break;
				default:
					throw new Exception("Error decompressing data.");
			}

			int testbit = 8;

			while (true)
			{
				var cmdbyte = src[srcptr++];

				if (testbit == 0)
					testbit = 0x80;

				do
				{
					if ((cmdbyte & testbit) != 0)
						dest[destptr++] = src[srcptr++]; //if the bit is set, we just copy the next byte
					else
					{
						var rle = BitConverter.ToUInt16(src, srcptr);
						if (rle == 0xFFFF)
							return;
						
						var repeatcount = (rle >> skipbits) + 2; //how many bits to copy
						var sampleoffset = destptr + (ushort)(rle & mask) - sub; //where to copy from

						var dist = destptr - sampleoffset;
						//Console.WriteLine(repeatcount);

						for (var i = 0; i < repeatcount; i++)
							dest[destptr++] = dest[sampleoffset++];

						srcptr += 2;
					}
					
					testbit >>= 1;
				} while (testbit != 0);
			}
		}

		static void Unpack(string inputfile, string outputfolder)
		{
			var fs = new FileStream(inputfile, FileMode.Open);
			var br = new BinaryReader(fs);

			var sig = new string(br.ReadChars(4));
			if (sig != "PRSM")
				throw new FileLoadException("File is not a valid prism ark battle archive!");

			var fcount = br.ReadInt32();
			var headersize = br.ReadInt32();

			fs.Seek(0, SeekOrigin.Begin);

			var header = br.ReadBytes(headersize);
			DecryptBlock(header, headersize, 0, 0);

			File.WriteAllBytes(Path.Combine(outputfolder, "header.txt"), header);

			var indexes = new Index[fcount];

			var headerms = new MemoryStream(header);
			var headerbr = new BinaryReader(headerms);

			headerms.Seek(0xC, SeekOrigin.Begin);

			for (var i = 0; i < fcount; i++)
			{
				indexes[i].Read(headerbr);
			}

			var packedext = new List<string>();
			var unpackedext = new List<string>();

			for (var i = 0; i < fcount; i++)
			{
				var entry = new FileEntry();

				var entrydata = new byte[132];
				DecompressBlock(entrydata, header, indexes[i].EntryOffset);

				entry.Filename = Encoding.GetEncoding(932).GetString(entrydata, 0, 0x7F).TrimEnd('\0');
				entry.Key = entrydata[0x7F];
				entry.UnpackedSize = BitConverter.ToInt32(entrydata, 0x80);
				entry.PackedSize = indexes[i].PackedSize;
				entry.FileOffset = indexes[i].FileOffset;

				Console.WriteLine("- Extrating {0}...", entry.Filename);

				fs.Seek(entry.FileOffset, SeekOrigin.Begin);
				var packed = br.ReadBytes(entry.PackedSize);
				DecryptBlock(packed, entry.PackedSize, entry.Key, 0);

				byte[] unpacked;

				if (entry.PackedSize == entry.UnpackedSize)
				{
					unpacked = packed;
					//Console.WriteLine("File isn't packed!!");
					var ext = Path.GetExtension(entry.Filename);
					if(!unpackedext.Contains(ext))
						unpackedext.Add(ext);
				}
				else
				{
					unpacked = new byte[entry.UnpackedSize];
					DecompressBlock(unpacked, packed, 0);
					var ext = Path.GetExtension(entry.Filename);
					if (!packedext.Contains(ext))
						packedext.Add(ext);
				}

				var outpath = Path.Combine(outputfolder, entry.Filename);
				var outdir = Path.GetDirectoryName(outpath);
				if (!Directory.Exists(outdir))
					Directory.CreateDirectory(outdir);

				File.WriteAllBytes(outpath, unpacked);
			}

			Console.WriteLine("Packed extensions: " + string.Join(", ", packedext));
			Console.WriteLine("Unpacked extensions: " + string.Join(", ", unpackedext));

			Console.ReadKey();
		}

		static void Main(string[] args)
		{
			//Unpack(@"D:\Projects\prismark\Park_tools_v2\park_tools\test.dat", @"D:\Projects\prismark\Park_tools_v2\park_tools\testout2");
			if (args.Length != 2)
			{
				Console.WriteLine("Usage: PrismBattleUnpacker (Source File) (Destination Folder)");
				return;
			}
			Unpack(args[0], args[1]);
		}
	}
}
