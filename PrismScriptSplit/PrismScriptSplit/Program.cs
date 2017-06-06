using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PrismScriptSplit
{
    class FunctionDef
    {
        public string FunctionName;
        public uint Id;
        public string Format;

        public FunctionDef(string functionname, uint id, string format)
        {
            FunctionName = functionname;
            Id = id;
            Format = format;
        }
    }

	static class Program
	{
		public static List<string> OutText = new List<string>();
		public static List<string> FileNames = new List<string>();

		public static List<int> Positions = new List<int>();
		public static List<int> Labels = new List<int>();

		public static MemoryStream TextDataStream;
		public static BinaryReader TextDataReader;

		public static List<string> UnknownOpCodes = new List<string>();

        public static Dictionary<uint, FunctionDef> Functions = new Dictionary<uint, FunctionDef>();

		public static int unknowns = 0;

#if DEBUG
        public static bool UseDebugPaths = true;
#else
	    public static bool UseDebugPaths = false;
#endif

        static string ReadString(int offset)
		{
			TextDataStream.Seek(offset, SeekOrigin.Begin);

			var ba = new List<byte>();
			while (true)
			{
				var b = TextDataReader.ReadByte();
				if (b == 0)
					break;

				ba.Add(b);
			}

			var s = Encoding.GetEncoding("Shift-JIS").GetString(ba.ToArray());

			return s;
		}

		static void ReadFileNameData(string inputfile)
		{
			var b = File.ReadAllBytes(inputfile);
			var ms = new MemoryStream(b);
			var br = new BinaryReader(ms);

			if (new string(br.ReadChars(12)) != "PJADV_FL0001")
				throw new Exception("Failed to load filename.dat!");

			var count = br.ReadInt32();

			for (var i = 0; i < count; i++)
			{
				var name = new string(br.ReadChars(32)).Trim('\0');
				FileNames.Add(name);
			}
		}
        
        static void ReadTextData(string inputfile)
		{
			var b = File.ReadAllBytes(inputfile);
			TextDataStream = new MemoryStream(b);
			TextDataReader = new BinaryReader(TextDataStream);

			byte key = 0xC5;

			for (var i = 0; i < b.Length; i++)
			{
				b[i] ^= key;
				key += 0x5C;
			}

            //File.WriteAllBytes(@"D:\Dropbox\Mangagamer\PrismArk\textdataunencoded.bin", b);
		}

		static string GetTextString(uint position)
		{
			if (position == 0)
				return "";

			TextDataStream.Seek((long) position, SeekOrigin.Begin);

			var ba = new List<byte>();
			while (true)
			{
				var b = TextDataReader.ReadByte();
				if (b == 0)
					break;

				ba.Add(b);
			}

			var s = Encoding.GetEncoding(932).GetString(ba.ToArray());

			return s;
		}

		static void OutputText(int pos, uint op, uint high, uint low, string param = "")
		{
			if (param != "")
				OutText.Add($"0x{pos:x8} | Ukn{high:x2}{low:x2}{op:x4} : {param}");
			else
				OutText.Add($"0x{pos:x8} | Ukn{high:x2}{low:x2}{op:x4}");
		}

		static void OutputCommand(int pos, string command)
		{
			//OutText.Add($"0x{pos:x8} | {command}");
			//OutText.Add($"\t0x{pos:x8} | {command}");
			OutText.Add($"\t{command}");
		}

		static string VLookup(uint value)
		{
			var val = value & 0xFFFFFFF;
			if ((val & 0x8000000) != 0)
				val |= 0xF0000000;

			switch (value >> 28)
			{
				case 1:
					//return $"var(1,{val})";
					return $"l{val}";
				case 2:
					//return $"var(2,{val})";
					return $"g{val}";
				case 3:
					//return $"var(3,{val})";
					return $"t{val}";
				case 0:
					return val.ToString();
				case 4:
					//return "uk761404";
					return $"tmp{val}";
			}
			return "0x"+(value.ToString("x8"));
		}

		static string ToHex(this string str, int count = 0)
		{
			var i = int.Parse(str);

			if (count == 0)
				return i.ToString("x");

			return i.ToString($"x{count}");


		}

		static void AddLabel(int pos)
		{
			if (!Labels.Contains(pos))
				Labels.Add(pos);
		}
		static void AddLabel(string strpos)
		{
			var pos = int.Parse(strpos);
		    if (pos == 0)
		        return;
			if (!Labels.Contains(pos))
				Labels.Add(pos);
		}

		static void Process(string inputfile, string outputfile, bool outputLineNumbers)
		{
			var b = File.ReadAllBytes(inputfile);
			var ms = new MemoryStream(b);
			var br = new BinaryReader(ms);

			var sig = new string(br.ReadChars(12));
			if (sig != "PJADV_SF0001")
				throw new Exception("File type not valid Prism Ark script file!");

			var count = br.ReadInt32();
			var unknown = br.ReadChars(16); //who knows?

			var data = new uint[(b.Length - 32) / 4];
			for (var i = 0; i < data.Length; i++)
				data[i] = br.ReadUInt32();

			var lastunknown = 0;

			for (var i = 0; i < data.Length; i++)
			{
				var pos = 32 + i * 4;

				Positions.Add(pos);

			    if (Functions.ContainsKey(data[i]))
			    {
			        var func = Functions[data[i]];

			        if (func.FunctionName == "text")
			        {
                        //special case for display text
                        var fname = "0";
                        if (data[i + 1] != 0)
                            fname = "\"" + FileNames[(int)data[i + 1] - 1] + "\"";
                        var type = VLookup(data[i + 5]);
                        if (type == "196608")
                            type = "CLICK_WAIT";
                        if (type == "131072")
                            type = "CONTINUE";
                        OutputCommand(pos, $"text {fname} \"{GetTextString(data[i + 2])}\" \"{GetTextString(data[i + 3])}\" {VLookup(data[i + 4])} {type} {VLookup(data[i + 6])}");
                        i += 6;
                    }
                    else if (func.FunctionName == "text2")
                    {
                        //special case for display text
                        var type = VLookup(data[i + 4]);
                        if (type == "196608")
                            type = "CLICK_WAIT";
                        if (type == "131072")
                            type = "CONTINUE";
                        OutputCommand(pos, $"text2 {VLookup(data[i + 1])} \"{GetTextString(data[i + 2])}\" \"{GetTextString(data[i + 3])}\" {type} {VLookup(data[i + 5])}");
                        i += 5;
                    }
                    else
			        {
			            var text = new List<string>();
                        text.Add(func.FunctionName);
			            
			            for (var j = 0; j < func.Format.Length; j++)
			            {
                            i++;
                            var dat = data[i];
			                var type = func.Format[j];
                            
			                switch (type)
			                {
                                case 'i':
                                    text.Add(VLookup(dat));
			                        break;
                                case 'h':
                                    text.Add($"0x{ VLookup(dat).ToHex()}");
                                    break;
                                case 's':
                                    text.Add("\"" + GetTextString(dat) + "\"");
			                        break;
                                case 'f':
                                    text.Add("\"" + FileNames[(int)data[i]-1] + "\"");
			                        break;
                                case 'l':
                                    if (dat != 0)
                                    {
                                        AddLabel(VLookup(dat));
                                        text.Add($"label_{VLookup(dat).ToHex()}");
                                    }
                                    else
                                    {
                                        text.Add(dat.ToString());
                                    }
                                    break;
                                default:
                                    Console.WriteLine("Unexpected function format key " + type);
			                        break;
			                }
			            }

                        OutputCommand(pos, string.Join(" ", text));
                    }
                    
                    continue;
                }
                
				if(pos - 4 != lastunknown)
					if(!UnknownOpCodes.Contains($"0x{data[i]:x8}"))
						UnknownOpCodes.Add($"0x{data[i]:x8}");

				lastunknown = pos;

				unknowns++;
				OutputCommand(pos, $"Unknown {data[i]:x8}");
				continue;
			}

			var realout = new List<string>();
            if(!outputLineNumbers)
                realout.Add("#start");
            else
			    realout.Add("!00000000\t#start");

            for (var i = 0; i < OutText.Count; i++)
            {
                var pos = Positions[i];

                if (Labels.Contains(pos))
			    {
			        if (!outputLineNumbers)
                        realout.Add("#label_" + pos.ToString("x"));
                    else
			            realout.Add($"!{pos:x8}\t" + "#label_" + pos.ToString("x"));
                }
			    if (!outputLineNumbers)
                    realout.Add(OutText[i]);
                else
				    realout.Add($"!{pos:x8}\t" + OutText[i]);
			}

			File.WriteAllLines(outputfile, realout);
			UnknownOpCodes.Sort();
			File.WriteAllLines(@"D:\Dropbox\Mangagamer\PrismArk\missingops.txt", UnknownOpCodes);
		}

	    static void AddHandlerData(string funcname, uint id, string format)
	    {
	        Functions.Add(id, new FunctionDef(funcname, id, format));
	    }

	    static void SetHandlerData(string handlerpath)
	    {
	        var lines = File.ReadAllLines(handlerpath);

	        foreach (var l in lines)
	        {
	            if (l.StartsWith("//") || string.IsNullOrEmpty(l))
	                continue;

	            var s = l.Split(new[] {'\t', ' '}, StringSplitOptions.RemoveEmptyEntries);

	            var id = uint.Parse(s[1].Substring(2), NumberStyles.HexNumber);

	            if (s.Length == 2 || s[2].StartsWith("//"))
	                AddHandlerData(s[0], id, "");
	            else
	                AddHandlerData(s[0], id, s[2]);
	        }
	    }

	    static void Main(string[] args)
	    {
	        var debug = args.Any(a => a == "debug");

            if (UseDebugPaths)
		    {
		        SetHandlerData(@"D:\Dropbox\Projects\PrismArkTools\PrismScriptSplit\functiondef.txt");
                ReadTextData(@"D:\Dropbox\Mangagamer\PrismArk\textdataorig.bin");
		        ReadFileNameData(@"D:\Dropbox\Mangagamer\PrismArk\filenameorig.dat");
		        Process(@"D:\Dropbox\Mangagamer\PrismArk\scenarioorig.dat", @"D:\Dropbox\Mangagamer\PrismArk\script.txt",
		            true);
		        Console.WriteLine("Done! Number of unhandled operators: " + unknowns);
		        Console.ReadKey();
		        return;
		    }

		    var path = ".\\";

		    //if (args.Length >= 1)
		    //    path = args[0];
            
		    SetHandlerData("functiondef.txt");
		    ReadTextData(Path.Combine(path, "textdata.bin"));
		    ReadFileNameData(Path.Combine(path, "filename.dat"));
		    Process(Path.Combine(path, "scenario.dat"), Path.Combine(path, "scenario.txt"), debug);
        }
	}
}
