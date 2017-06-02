using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace PrismMerge
{
	class Program
	{
		private static Encoding Japanese = Encoding.GetEncoding("Shift-JIS");
		private static Encoding English = Encoding.UTF8;

		static void WriteString(BinaryWriter writer, string sin)
		{
			if (sin.Length%2 == 1)
				sin += " \0\0";



			var sout = Encoding.Convert(English, Japanese, English.GetBytes(sin));

			foreach (var c in sout)
			{
				writer.Write(c);
				writer.Write((byte)0);
			}

			//writer.Write(sout);

			byte b = 0;

			writer.Write(b);
			writer.Write(b);
		}

		private static void Main(string[] args)
		{
			var textdatainname = "textdataorig.bin";
			var textdataoutname = "textdata.bin";
			var scenarioinname = "scenarioorig.dat";
			var scenariooutname = "scenario.dat";
			var textinname = "prismark.txt";

			var textdatabytes = File.ReadAllBytes(textdatainname);
			var textdatastream = new MemoryStream(textdatabytes);
			var textdata = new BinaryReader(textdatastream);

			var textdataoutstream = new MemoryStream();
			var textdataout = new BinaryWriter(textdataoutstream);

			var scenariobytes = File.ReadAllBytes(scenarioinname);
			var scenariostream = new MemoryStream(scenariobytes);
			var scenariodata = new BinaryReader(scenariostream);

			var scenariooutstream = new MemoryStream();
			var scenariodataout = new BinaryWriter(scenariooutstream);

			textdatastream.Seek(12, SeekOrigin.Begin);
			var entrycount = textdata.ReadInt32();

			var textin = File.ReadAllLines(textinname);
			var line = 0;

			textdataoutstream.Seek(0, SeekOrigin.End);
			
			//decode current textdata
			byte key = 0xC5;

			for (var i = 0; i < textdatabytes.Length; i++)
			{
				textdatabytes[i] ^= key;
				key += 0x5C;
			}

			textdataout.Write(textdatabytes);
			scenariodataout.Write(scenariobytes);

			var output = new List<string>();
			const uint textoutkey = 0x80000307;
			const uint continuationkey = 0x80000406;
			const uint optionoutkey = 0x01010203;

			for (var i = 0; i < scenariobytes.Length - 4; i++)
			{
				scenariostream.Seek(i, SeekOrigin.Begin);
				var j = scenariodata.ReadUInt32();

				if (j == optionoutkey)
				{
					scenariodataout.Seek(i + 4, SeekOrigin.Begin);

					var s = textin[line].Split(new[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
					if(s.Length == 1)
						throw new Exception("Expecting option on line '"+line+"'");

					var text = s[1];

					scenariodataout.Write((int)textdataoutstream.Position);
					WriteString(textdataout, text);

					line++;
				}

				if (j == continuationkey)
				{
					var j2 = scenariodata.ReadUInt32();

					if (j2 != 0)
						continue;

					var name = "";
					var text = "";

					var s = textin[line].Split(new[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
					if (s.Length == 1 || s[0] != "cnt")
						throw new Exception("Expecting continuation (cnt) on line '"+line+"'");

					if(s.Length == 2)
						text = s[1];
					else
					{
						name = s[1];
						text = s[2];
					}

					if (name == "")
						scenariodataout.Write(0);
					else
					{
						scenariodataout.Write((int)textdataoutstream.Position);
						WriteString(textdataout, name);
					}

					scenariodataout.Write((int)textdataoutstream.Position);
					WriteString(textdataout, text);

					line++;
				}

				if (j == textoutkey)
				{
					scenariodataout.Seek(i, SeekOrigin.Begin);

					var name = "";
					var text = "";

					var s = textin[line].Split(new[] {'@'}, StringSplitOptions.RemoveEmptyEntries);
					if (s.Length == 1)
						text = s[0];
					else
					{
						name = s[0];
						text = s[1];
					}

					//text += "\0"; //null for the... null.

					scenariodataout.Seek(8, SeekOrigin.Current);

					//scenariodata.ReadInt32(); //skip unknown
					if (name == "")
						scenariodataout.Write(0);
					else
					{
						scenariodataout.Write((int)textdataoutstream.Position);
						WriteString(textdataout, name);
					}

					scenariodataout.Write((int)textdataoutstream.Position);
					WriteString(textdataout, text);

					line++;
				}

				if(textdatastream.Position%2==1)
					textdataout.Write("\0");
			}

			File.WriteAllBytes(scenariooutname, scenariooutstream.ToArray());

			var tout = textdataoutstream.ToArray();

			File.WriteAllBytes("textdatauncompressed.dat", tout);
			
			//decode current textdata
			key = 0xC5;

			for (var i = 0; i < tout.Length; i++)
			{
				tout[i] ^= key;
				key += 0x5C;
			}


			File.WriteAllBytes(textdataoutname, tout);
		}
	}
}
