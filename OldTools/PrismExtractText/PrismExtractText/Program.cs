using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrismExtractText
{
	class Program
	{
		static string ReadString(BinaryReader reader)
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

			return s;
		}

		static void Main(string[] args)
		{
			var scenarioname = "scenario.dat";
			var textdataname = "textdata.bin";
			var outputname = "prismark.txt";

			var textdata = File.ReadAllBytes(textdataname);
			var textstream = new MemoryStream(textdata);
			var text = new BinaryReader(textstream);

			var scenariodata = File.ReadAllBytes(scenarioname);
			var scenariostream = new MemoryStream(scenariodata);
			var scenario = new BinaryReader(scenariostream);


			byte key = 0xC5;

			for (var i = 0; i < textdata.Length; i++)
			{
				textdata[i] ^= key;
				key += 0x5C;
			}

			var jumplist = new SortedDictionary<int, int>();
			var skiplist = new SortedDictionary<int, int>();


			var output = new List<string>();
			var textoutkey = 0x80000307;
			var continuationkey = 0x80000406;
			var optionoutkey = 0x01010203;

			string oname = "";
			string otext = "";
			
			for (var i = 0; i < scenariodata.Length - 4; i++)
			{
				scenariostream.Seek(i, SeekOrigin.Begin);

				var pos = scenariostream.Position;

				var j = scenario.ReadUInt32();

				if (j == optionoutkey)
				{
					oname = "option";
					otext = "";

					var textoffset = scenario.ReadInt32();
					var jumppoint = scenario.ReadInt32();

					textstream.Seek(textoffset, SeekOrigin.Begin);
					otext = ReadString(text);

					//f(jump)
					output.Add(oname + "@" + otext);

				}

				if (j == continuationkey)
				{
					var j2 = scenario.ReadInt32();
					if (j2 != 0)
						continue;

					oname = "";
					var nameoffset = scenario.ReadInt32();
					var textoffset = scenario.ReadInt32();

					if (nameoffset != 0)
					{
						textstream.Seek(nameoffset, SeekOrigin.Begin);
						oname = ReadString(text);
					}

					textstream.Seek(textoffset, SeekOrigin.Begin);
					otext = ReadString(text);
					

					if (oname == "")
						output.Add("cnt@" + otext);
					else
						output.Add("cnt@" + oname + "@" + otext);
				}

				if (j == textoutkey)
				{
					oname = "";

					var namelen = scenario.ReadInt32();
					var nameoffset = scenario.ReadInt32();
					var textoffset = scenario.ReadInt32();
					var textlen = scenario.ReadInt32();

					if (nameoffset != 0)
					{
						textstream.Seek(nameoffset, SeekOrigin.Begin);
						oname = ReadString(text);
					}

					textstream.Seek(textoffset, SeekOrigin.Begin);
					otext = ReadString(text);
					
					if (oname == "")
						output.Add(otext);
					else
						output.Add(oname + "@" + otext);

				}
			}

			File.WriteAllLines(outputname, output);
		}
	}
}
