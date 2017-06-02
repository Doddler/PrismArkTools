using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PrismBattlePacker
{
	class FileEntry
	{
		public string Filename;
		public string FullPath;
		public byte Key;
		public int EntryOffset;
		public int UnpackedSize;
		public int FileOffset;
		public int PackedSize;
	}

	class Program
	{
		public static int CompressedSize = 0;
		public static int UncompressedSize = 0;
		public static int FileCount = 0;

		public static bool FastCompile = false;

		public static ConcurrentQueue<FileEntry> FileQueue = new ConcurrentQueue<FileEntry>();
		public static ConcurrentDictionary<FileEntry, byte[]> CompletedCompression = new ConcurrentDictionary<FileEntry, byte[]>();

		public static string ToRelativePath(string filePath, string refPath)
		{
			var pathNormalized = Path.GetFullPath(filePath);

			var refNormalized = Path.GetFullPath(refPath);
			refNormalized = refNormalized.TrimEnd('\\', '/');

			if (!pathNormalized.StartsWith(refNormalized))
				throw new ArgumentException();
			var res = pathNormalized.Substring(refNormalized.Length + 1);
			return res;
		}

		static void EncryptBlock(byte[] buff, int offset, int size, byte key, int keyoffset)
		{
			if (size <= 0)
				return;

			for (var i = 0; i < size; i++)
			{
				buff[offset + i] = SubTable.FlippedTable[keyoffset * 256 + buff[offset + i]];
				buff[offset + i] += key;
				keyoffset++;
				keyoffset %= 6;
			}
		}
		
		static void EncryptStream(Stream s, int offset, int size, byte key, int keyoffset)
		{
			if (size <= 0)
				return;

			s.Seek(offset, SeekOrigin.Begin);

			var bw = new BinaryWriter(s);
			var br = new BinaryReader(s);
			
			for (var i = 0; i < size; i++)
			{
				var b = br.ReadByte();
				b = SubTable.FlippedTable[keyoffset * 256 + b];
				b += key;
				s.Seek(-1, SeekOrigin.Current);
				bw.Write(b);
				keyoffset++;
				keyoffset %= 6;
			}
		}
		
		static void BuildTable(byte[] input, int ptr, int len, int[] table)
		{
			//build longest suffix-prefix table
			if (ptr + len > input.Length)
				len = input.Length - ptr;

			//var table = new int[len];

			table[0] = 0;

			var j = 0;

			for (var i = 1; i < len; i++)
			{
				while (j > 0 && input[j + ptr] != input[i + ptr])
					j = table[j - 1];

				if (input[j + ptr] == input[i + ptr])
					j++;

				table[i] = j;
			}
		}

		static unsafe Tuple<int, int> FindRleMatch(byte[] input, int ptr, int searchdist, int maxlen, int[] table)
		{
			//using knuth-morris-pratt

			BuildTable(input, ptr, maxlen, table);

			var matchpos = -1;
			var matchlen = -1;

			var realsearchdist = Math.Min(ptr, searchdist);
			var realmax = ptr + maxlen - 1;
			if (realmax >= input.Length)
				realmax = input.Length - 1;
			
			fixed (byte* inputsource = input)
			{
				var j = 0; //matched characters

				var srcptr = inputsource + ptr - realsearchdist;
				var dstptr = inputsource + ptr;

				//it works, but I don't know why
				for (var i = ptr - realsearchdist; i < realmax; i++)
				{
					if (j == 0 && i >= ptr)
						break;

					if (ptr + j > realmax)
						break;

					while (j > 0 && *srcptr != *dstptr)
					{
						j = table[j - 1];
						dstptr = inputsource + ptr + j;
					}

					if (*srcptr == *dstptr)
					{
						j++;
						dstptr++;
						if (j > 1 && j > matchlen && i - j + 1 < ptr)
						{
							matchlen = j;
							matchpos = i - matchlen + 1;
							if (matchlen >= maxlen)
								return new Tuple<int, int>(matchpos, matchlen);
						}
					}

					srcptr++;
				}
			}

			return new Tuple<int, int>(matchpos, matchlen);
		}
		
		static byte[] GetRleData(byte[] input, ushort mask, byte skipbits, ushort sub, int testbit)
		{
			var bytedata = new byte[input.Length * 3]; //should be safe...

			var segment = new byte[16]; //maximum length of a segment
			var segmentpos = 0;
			var controlbyte = (byte)0;
			var actionpos = 0;

			var maxrlelen = (0xFFFF >> skipbits);
			var maxsearchdist = 0xFFFF & mask;

			if (FastCompile)
				maxsearchdist = 0x50 & mask; //lol

			var table = new int[maxrlelen]; //we make the rle table here once and reuse it to avoid garbage colleciton woes
			
			var srcptr = 0;
			var dstptr = 0;

			while (srcptr < input.Length)
			{
				var res = FindRleMatch(input, srcptr, maxsearchdist, maxrlelen, table);
				
				var match = res.Item1;
				var matchlen = res.Item2;

				if (match != -1 && matchlen >= 2)
				{
					var len = (matchlen - 2) << skipbits;
					var offset = match - srcptr + sub;
					
					var rle = (ushort)(len | offset);
					
					segment[segmentpos++] = (byte)(rle & 0xFF);
					segment[segmentpos++] = (byte)(rle >> 8);
					actionpos++;

					srcptr += matchlen;
				}
				else
				{
					//segmentlist.Add(new[] {input[srcptr++]});
					segment[segmentpos++] = input[srcptr++];
					controlbyte |= (byte)(1 << (testbit-1-actionpos));
					actionpos++;
				}

				if (actionpos >= testbit)
				{
					bytedata[dstptr++] = (byte)controlbyte;
					Buffer.BlockCopy(segment, 0, bytedata, dstptr, segmentpos);
					dstptr += segmentpos;
					controlbyte = 0;
					actionpos = 0;
					segmentpos = 0;
					testbit = 8; //after the first command bit is set, we set it to test all 8 bits
				}
			}

			//output the end of stream marker.
			segment[segmentpos++] = 0xFF;
			segment[segmentpos++] = 0xFF;

			bytedata[dstptr++] = (byte)controlbyte;
			Buffer.BlockCopy(segment, 0, bytedata, dstptr, segmentpos);
			dstptr += segmentpos;

			//we now have all our data... create a properly sized byte array and move stuff over. Because... yeah.
			var byteout = new byte[dstptr];
			Buffer.BlockCopy(bytedata, 0, byteout, 0, dstptr);

			return byteout;
		}
		
		static byte[] Compress(byte[] input)
		{
			//apparently we only use this type...
			var data2 = GetRleData(input, 0x7FF, 0xB, 0x800, 4);
			data2[0] |= 0x40;
			
			return data2;
		}

		static void WriteEmptyBytes(BinaryWriter br, int count)
		{
			for (var i = 0; i < count; i++)
				br.Write((byte)0);
		}

		static void IntIntoByteArray(byte[] buffer, int val, int offset)
		{
			var bytes = BitConverter.GetBytes(val);
			for (var i = 0; i < bytes.Length; i++)
				buffer[offset + i] = bytes[i];
		}

		static void CompressFileLoop()
		{
			FileEntry entry;

			while (FileQueue.TryDequeue(out entry))
			{
				var sw = Stopwatch.StartNew();
				
				var unpackeddata = File.ReadAllBytes(entry.FullPath);
				var ext = Path.GetExtension(entry.FullPath);

				byte[] packeddata;

				if (ext == ".ogg" || ext == ".scc")
					packeddata = unpackeddata;
				else
					packeddata = Compress(unpackeddata);

				//if (packeddata.Length > unpackeddata.Length)
				//	packeddata = unpackeddata;
				
				sw.Stop();

				Console.WriteLine("- Packed {4} from {0} to {1} in {2}ms ({3}%).", unpackeddata.Length, packeddata.Length, sw.ElapsedMilliseconds, Math.Round((float)packeddata.Length / (float)unpackeddata.Length * 100f), entry.Filename);

				if (!CompletedCompression.TryAdd(entry, packeddata))
					throw new Exception("Failed to transfer compressed file data from worker thread!");
			}
			
		}

		static void Pack(string inputfolder, string outputpath)
		{
			SubTable.FlipTable();

			//var buff = new byte[] {0x52, 0x00};

			//EncryptBlock(buff, 0, 2, 20, 0);
			//DecryptBlock(buff, 2, 20, 0);

			var entrylist = new List<FileEntry>();
			var filedata = new List<byte[]>();

			var fs = new FileStream(outputpath, FileMode.Create);
			var br = new BinaryWriter(fs);

			var random = new Random();

			Console.WriteLine("- Building file list...");

			var headerlength = 0;

			//build the file and entry lists
			foreach (var f in Directory.GetFiles(inputfolder, "*.*", SearchOption.AllDirectories))
			{
				var fname = ToRelativePath(f, inputfolder);
				var fi = new FileInfo(f);

				var entry = new FileEntry
				{
					Filename = fname,
					FullPath = f,
					UnpackedSize = (int)fi.Length,
					Key = (byte)random.Next(255)
				};

				entrylist.Add(entry);
			}

			var fcount = entrylist.Count;

			//skip the file offset and compressed size table for now
			WriteEmptyBytes(br, 0xC + 0xC * fcount);

			Console.WriteLine("- Building file index table...");

			//write the file entry headers.
			for (var i = 0; i < fcount; i++)
			{
				var entry = entrylist[i];

				entry.EntryOffset = (int)fs.Position;

				var entryunpacked = new byte[132];
				var bcount = Encoding.GetEncoding(932).GetByteCount(entry.Filename);
				if (bcount > 0x7F)
				{
					Console.WriteLine("Cannot pack file {0}, it's filename is too long!", entry.Filename);
					return;
				}
				Encoding.GetEncoding(932).GetBytes(entry.Filename, 0, entry.Filename.Length, entryunpacked, 0);
				entryunpacked[0x7F] = entry.Key;
				IntIntoByteArray(entryunpacked, entry.UnpackedSize, 0x80);

				var packedentry = Compress(entryunpacked);

				br.Write(packedentry);
				
				FileQueue.Enqueue(entry);
			}

			headerlength = (int)fs.Position;
			
			var tasks = new Task[Environment.ProcessorCount];

			for (var i = 0; i < tasks.Length; i++)
				tasks[i] = Task.Factory.StartNew(CompressFileLoop);
			
			//time to write the files themselves
			for (var i = 0; i < fcount; i++)
			{
				var entry = entrylist[i];

				while (true)
				{
					if (CompletedCompression.ContainsKey(entry))
						break;

					Thread.Sleep(1);
				}

				byte[] packeddata;
				CompletedCompression.TryRemove(entry, out packeddata);

				EncryptBlock(packeddata, 0, packeddata.Length, entry.Key, 0);
				
				entry.PackedSize = packeddata.Length;
				entry.FileOffset = (int)fs.Position;
				
				CompressedSize += entry.PackedSize;
				UncompressedSize += entry.UnpackedSize;
				FileCount++;

				br.Write(packeddata);
			}

			//everythings written except the headers. Gotta do that stuff.
			
			fs.Seek(0xC, SeekOrigin.Begin);

			for (var i = 0; i < entrylist.Count; i++)
			{
				br.Write(entrylist[i].EntryOffset);
				br.Write(entrylist[i].FileOffset);
				br.Write(entrylist[i].PackedSize);
			}

			EncryptStream(fs, 0, headerlength, 0, 0);

			fs.Seek(0, SeekOrigin.Begin);
			
			br.Write(new []{'P','R','S','M'});
			br.Write(fcount);
			br.Write(headerlength);

			fs.Close();
		}

		static void Main(string[] args)
		{
			if (args.Contains("-fast"))
			{
				args = args.Where(ar => ar != "-fast").ToArray();
				FastCompile = true;
			}

			if (args.Length != 2)
			{
				Console.WriteLine("Usage: PrismBattlePacker [-fast] (Source File) (Destination Folder)");
				Console.WriteLine("Use -fast flag to speed up packing a significant hit to compression size.");
				return;
			}
			
			var sw = Stopwatch.StartNew();
			Pack(args[0], args[1]);
			sw.Stop();

			Console.WriteLine("Done!");
			Console.WriteLine("Completed packing {0} files from {1} bytes to {2} bytes in {3} seconds.", FileCount, UncompressedSize, CompressedSize, Math.Round(sw.Elapsed.TotalSeconds));
		}
	}
}
