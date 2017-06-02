using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PrismText
{
	class Program
	{
		static void Main(string[] args)
		{
			string filenamein = @"textdata.bin";
			string filenameout = @"textdata.txt";

			var japanese = Encoding.GetEncoding("Shift-JIS");
			var english = Encoding.UTF8;

			if (true)
			{
				var file = File.ReadAllBytes(filenamein);

				//var fin = new FileStream(filenamein, FileMode.Open, FileAccess.Read);
				//var b = new BinaryReader(fin);

				byte key = 0xC5;

				for (var i = 0; i < file.Length; i++)
				{
					file[i] ^= key;
					key += 0x5C;
				}

				var reader = new BinaryReader(new MemoryStream(file));

				var header = new String(reader.ReadChars(12));
				if (header != "PJADV_TF0001")
					throw new InvalidDataException("The file has an invalid header! " + header);

				var count = reader.ReadInt32();

				List<string> stringlist = new List<string>();

				for (var i = 0; i < count; i++)
				{
					var ba = new List<byte>();
					while (true)
					{
						var b = reader.ReadByte();
						if (b == 0)
							break;

						ba.Add(b);
					}

					var s = Encoding.GetEncoding("Shift-JIS").GetString(ba.ToArray());

					stringlist.Add(s);

					reader.ReadByte();
				}

				//File.WriteAllBytes(filenameout, file);

				File.WriteAllLines(filenameout, stringlist);
			}
			else
			{
				var stream = new MemoryStream();
				var output = new BinaryWriter(stream);

				output.Write("PJADV_TF0001".ToCharArray());

				var sin = File.ReadAllLines(filenameout, english);

				output.Write(sin.Length);

				byte n = 0;

				foreach (var s in sin)
				{
					var s2 = Encoding.Convert(english, japanese, english.GetBytes(s));
					output.Write(s2);
					output.Write(n);
					output.Write(n);
				}

				byte key = 0xC5;

				var bout = stream.ToArray();

				for (var i = 0; i < bout.Length; i++)
				{
					bout[i] ^= key;
					key += 0x5C;
				}

				File.WriteAllBytes("textdata2.bin", bout);
			}
		}
	}
}
