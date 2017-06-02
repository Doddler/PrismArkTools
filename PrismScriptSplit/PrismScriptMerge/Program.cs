using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrismScriptMerge
{
    class Program
    {
        static Dictionary<string, int> LabelPositions = new Dictionary<string, int>();
        static List<Tuple<string, int>> LabelCalls = new List<Tuple<string, int>>();

        static Dictionary<string, int> StringPositions = new Dictionary<string, int>();
        static Dictionary<string, int> FilenamePositions = new Dictionary<string, int>();

        private static MemoryStream ScriptDataStream;
        private static BinaryWriter ScriptData;

        private static MemoryStream StringDataStream;
        private static BinaryWriter StringData;

        private static MemoryStream FilenameDataStream;
        private static BinaryWriter FilenameData;

        static Dictionary<string, string> HandlerData = new Dictionary<string, string>();
        static Dictionary<string, uint> FunctionCodes = new Dictionary<string, uint>();

        private static int CommandCount = 0;

        private static bool debugpaths = false;

        private static int LineNum = 0;

        static void AddHandlerData(string cmdname, uint funcval, string format)
        {
            FunctionCodes.Add(cmdname, funcval);
            HandlerData.Add(cmdname, format);
        }

        static void AddUnknownHandler(string cmdname, string format)
        {
            var val = cmdname.Substring(2);
            var id = uint.Parse(val, NumberStyles.HexNumber);

            FunctionCodes.Add(cmdname, id);
            HandlerData.Add(cmdname, format);
        }

        static void SetHandlerData()
        {
            string path;

            if (debugpaths)
                path = @"D:\Dropbox\Mangagamer\PrismArk\functiondef.txt";
            else
                path = @"functiondef.txt";


            if (!File.Exists(path))
                throw new Exception("Cannot find function definition file functiondef.txt!");

            var lines = File.ReadAllLines(path);

            foreach (var l in lines)
            {
                if (l.StartsWith("//") || string.IsNullOrEmpty(l))
                    continue;

                var s = l.Split(new[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                var id = uint.Parse(s[1].Substring(2), NumberStyles.HexNumber);

                if (s.Length == 2 || s[2].StartsWith("//"))
                    AddHandlerData(s[0], id, "");
                else
                    AddHandlerData(s[0], id, s[2]);
            }

            //AddHandlerData("jump", 0x01000202, "l");
            //AddHandlerData("call", 0x01000302, "l");
            //AddHandlerData("return", 0x01000401, "");
            //AddHandlerData("wait", 0x01000502, "i");
            //AddHandlerData("click", 0x01000602, "i");
            //AddHandlerData("jz", 0x01000703, "il");
            //AddHandlerData("gettime", 0x01000801, "");
            //AddHandlerData("waitfortime", 0x01000a02, "i");
            //AddHandlerData("scenename", 0x01000d02, "s");
            //AddHandlerData("choiceconfig", 0x01010009, "ffiiifff");
            //AddHandlerData("choicestart", 0x01010101, "");
            //AddHandlerData("choice", 0x01010203, "sl");
            //AddHandlerData("choiceexecute", 0x01010301, "");
            //AddHandlerData("setmenugroup", 0x01020002, "i");
            //AddHandlerData("setmenu", 0x0102010A, "iiiiiiifl");
            //AddHandlerData("checksave", 0x01030202, "i");

            //AddHandlerData("clearflags", 0x02000002, "i");
            //AddHandlerData("varset", 0x02000103, "ii");
            //AddHandlerData("varadd", 0x02000203, "ii");
            //AddHandlerData("varsub", 0x02000303, "ii");
            //AddHandlerData("varmul", 0x02000403, "ii");
            //AddHandlerData("vardiv", 0x02000503, "ii");
            //AddHandlerData("varmod", 0x02000603, "ii");
            //AddHandlerData("varinc", 0x02000702, "i");
            //AddHandlerData("vardec", 0x02000802, "i");
            //AddHandlerData("varbitwiseand", 0x02000903, "ii");
            //AddHandlerData("varbitwiseor", 0x02000A03, "ii");
            //AddHandlerData("varequals", 0x02010003, "ii");
            //AddHandlerData("varlessthan", 0x02010103, "ii");
            //AddHandlerData("vargreaterthan", 0x02010203, "ii");
            //AddHandlerData("varlessthanorequals", 0x02010303, "ii");
            //AddHandlerData("vargreaterorequals", 0x02010403, "ii");
            //AddHandlerData("varand", 0x02010503, "ii");
            //AddHandlerData("varor", 0x02010603, "ii");
            //AddHandlerData("varnotequals", 0x02010703, "ii");

            //AddHandlerData("layerset", 0x04000004, "ifi");
            //AddHandlerData("layerdel", 0x04000102, "i");
            //AddHandlerData("layeropacity", 0x04000503, "ii");
            //AddHandlerData("layerupdate", 0x04010103, "ii");
            //AddHandlerData("layertrans", 0x04010505, "iiii");

            //AddHandlerData("bgm", 0x05000003, "fi");
            //AddHandlerData("se", 0x05010004, "fii");

            //AddHandlerData("text", 0x80000307, "fssiii");

            //AddHandlerData("fade", 0x80010004, "iii");
            //AddHandlerData("setbg", 0x80010102, "f");

            //AddUnknownHandler("Op00100102", "i");
            //AddUnknownHandler("Op00100402", "i");
            //AddUnknownHandler("Op00100502", "i");
            //AddUnknownHandler("Op00100602", "i");

            //AddUnknownHandler("Op00100b02", "i");
            //AddUnknownHandler("Op00100d02", "i");
            //AddUnknownHandler("Op00100e02", "i");
            //AddUnknownHandler("Op00100f02", "i");
            //AddUnknownHandler("Op00101002", "i");
            //AddUnknownHandler("Op00102002", "i");

            //AddUnknownHandler("Op01000902", "i");

            //AddUnknownHandler("Op01020203", "ii");

            //AddUnknownHandler("Op00100c02", "i");
            //AddUnknownHandler("Op01000f03", "ii");

            //AddUnknownHandler("Op02020002", "i");
            //AddUnknownHandler("Op02020102", "i");

            //AddUnknownHandler("Op03000303", "is");
            //AddUnknownHandler("Op03001303", "ii");

            //AddUnknownHandler("Op04000e03", "ii");
            //AddUnknownHandler("Op04010001", "");
            //AddUnknownHandler("Op04020101", "");

            //AddUnknownHandler("Op05000102", "i");
            //AddUnknownHandler("Op05000504", "fii");
            //AddUnknownHandler("Op05010102", "i");
            //AddUnknownHandler("Op05020003", "fi");
            //AddUnknownHandler("Op05020301", "");
            //AddUnknownHandler("Op05030002", "f");

            //AddUnknownHandler("Op80010204", "ifi");
            //AddUnknownHandler("Op80010604", "fii");

        }

        static void AddString(string text, bool writeScriptData = true)
        {
            if (string.IsNullOrEmpty(text))
            {
                ScriptData.Write(0);
                return;
            }

            if (StringPositions.ContainsKey(text))
            {
                ScriptData.Write(StringPositions[text]);
                return;
            }

            var pos = (int)StringDataStream.Position;

            var bytes = Encoding.GetEncoding(932).GetBytes(text);
            var nullb = Encoding.GetEncoding(932).GetBytes("\0");

            StringData.Write(bytes);
            StringData.Write(nullb);
            StringData.Write(nullb);
            //if (bytes.Length % 2 == 1)
            //    StringData.Write(nullb);

            StringPositions.Add(text, pos);

            if (writeScriptData)
                ScriptData.Write(pos);
        }

        static void AddFilename(string text)
        {
            if (text == "0")
            {
                ScriptData.Write(0);
                return;
            }

            if (FilenamePositions.ContainsKey(text))
            {
                ScriptData.Write(FilenamePositions[text]);
                return;
            }

            var pos = (int)(FilenameDataStream.Position / 32) + 1;
            var bytes = Encoding.GetEncoding(932).GetBytes(text);

            FilenameData.Write(bytes);
            for (var i = bytes.Length; i < 32; i++)
                FilenameData.Write('\0');

            FilenamePositions.Add(text, pos);

            ScriptData.Write(pos);
        }

        static void AddNumber(string text)
        {
            uint num = 0;

            if (text.StartsWith("0"))
            {
                if (text.StartsWith("0x"))
                    num = uint.Parse(text.Substring(2), NumberStyles.HexNumber);
                else
                    num = uint.Parse(text, NumberStyles.HexNumber);

            }
            else if (text == "tmp0")
            {
                num = 4 << 28;
            }
            else if (text == "tmp1")
            {
                num = (4 << 28) + 1;
            }
            else if (text == "CLICK_WAIT")
                num = 196608;
            else if (text == "CONTINUE")
                num = 131072;
            else
            {
                if (text.StartsWith("l") || text.StartsWith("g") || text.StartsWith("t"))
                {
                    var t2 = text.Substring(1);
                    num = uint.Parse(t2);
                    if (text.StartsWith("l"))
                        num |= 1 << 28;
                    if (text.StartsWith("g"))
                        num |= 2 << 28;
                    if (text.StartsWith("t"))
                        num |= 3 << 28;
                }
                else
                    num = uint.Parse(text);
            }

            ScriptData.Write(num);
        }

        static List<string> SplitLineParameters(string line)
        {
            var dat = new List<string>();

            var curword = "";


            void LocalAddWord()
            {
                dat.Add(curword);
                curword = "";
            }

            for (var i = 0; i < line.Length; i++)
            {
                var c = line[i];

                //comment is the end
                if (c == '/')
                {
                    if (i + 1 < line.Length && line[i + 1] == '/')
                        break;
                }

                //split on space
                if (c == ' ')
                {
                    if (curword.Length > 0)
                        LocalAddWord();
                    continue;
                }

                //group all text within quotes
                if (c == '"')
                {
                    if (curword.Length > 0)
                        LocalAddWord();

                    i++;
                    c = line[i];

                    while (c != '"')
                    {
                        curword += c;

                        i++;
                        c = line[i];
                    }

                    LocalAddWord();

                    continue;
                }

                curword += c;
            }

            if (curword.Length > 0)
                dat.Add(curword);

            return dat;
        }

        static bool IsNumeric(string num)
        {
            if (uint.TryParse(num, out var i))
                return true;

            if (string.IsNullOrEmpty(num))
                return false;

            if (num == "tmp0" || num == "tmp1" || num == "CLICK_WAIT" || num == "CONTINUE")
                return true;

            if (num.StartsWith("0x"))
                return true;

            if (num[0] == 'l' || num[0] == 'g' || num[0] == 't')
            {
                var v = num.Substring(1);
                if (uint.TryParse(v, out uint j))
                    return true;
            }

            return false;
        }

        static bool OutputCommand(List<string> commandData)
        {
            var cmd = commandData[0];
            if (!HandlerData.ContainsKey(cmd))
                return false;

            var format = HandlerData[cmd];
            var id = FunctionCodes[cmd];

            if (commandData.Count == format.Length - 1)
            {
                Console.WriteLine($"Invalid parameter count for command {cmd} on line {LineNum}");
                return true;
            }

            CommandCount++;

            ScriptData.Write(id);

            for (var i = 0; i < format.Length; i++)
            {
                var type = format[i];
                var val = commandData[i + 1];

                switch (type)
                {
                    case 'h':
                    case 'i':
                        if (!IsNumeric(val))
                        {
                            Console.WriteLine($"Expected numeric value for {cmd} on line {LineNum}");
                            break;
                        }
                        AddNumber(val);

                        break;
                    case 's':
                        AddString(val);
                        break;
                    case 'f':
                        AddFilename(val);
                        break;
                    case 'l':
                        {
                            var pos = (int)ScriptDataStream.Position;
                            LabelCalls.Add(new Tuple<string, int>(val, pos));
                            ScriptData.Write((uint)0x69696969);
                        }
                        break;
                    default:
                        Console.WriteLine($"Unable to properly handle command {cmd} on line {LineNum}");
                        break;
                }
            }

            return true;
        }

        static void EncryptTextStream(byte[] b)
        {
            //for certain definitions of encryption
            byte key = 0xC5;

            for (var i = 0; i < b.Length; i++)
            {
                b[i] ^= key;
                key += 0x5C;
            }
        }

        static void Process(string inputfile, string outputfolder)
        {
            //set up output stream
            ScriptDataStream = new MemoryStream();
            ScriptData = new BinaryWriter(ScriptDataStream);

            StringDataStream = new MemoryStream();
            StringData = new BinaryWriter(StringDataStream);

            FilenameDataStream = new MemoryStream();
            FilenameData = new BinaryWriter(FilenameDataStream);

            ScriptData.Write("PJADV_SF0001".ToCharArray());
            ScriptData.Write(0);
            ScriptData.Write((uint)0x62626262); //not sure what these are to be honest
            ScriptData.Write((uint)0x63636363);
            ScriptData.Write((uint)0x64646464);
            ScriptData.Write((uint)0x65656565);

            StringData.Write("PJADV_TF0001".ToCharArray());
            StringData.Write(0);

            FilenameData.Write("PJADV_FL0001".ToCharArray());
            FilenameData.Write(0);

            //read input stream
            var lines = File.ReadAllLines(inputfile);

            foreach (var line in lines)
            {
                var l = line.Trim();

                if (l.StartsWith("!"))
                    l = l.Substring(10).Trim();

                LineNum++;

                if (string.IsNullOrEmpty(l) || l.StartsWith("//"))
                    continue;

                if (l.StartsWith("#"))
                {
                    var labelname = l.Substring(1);
                    if (LabelPositions.ContainsKey(labelname))
                        Console.WriteLine($"Error! Cannot redefine label {labelname}");
                    else
                        LabelPositions.Add(labelname, (int)ScriptDataStream.Position);

                    continue;
                }

                var command = SplitLineParameters(l);
                var cmd = command[0];

                if (OutputCommand(command))
                    continue;

                if (cmd == "Unknown")
                {
                    var val = uint.Parse(command[1], NumberStyles.HexNumber);
                    ScriptData.Write(val);
                    continue;
                }

                //non matched command
                Console.WriteLine($"Non matched command type {cmd} on line {LineNum}");
            }

            foreach (var l in LabelCalls)
            {
                if (!LabelPositions.ContainsKey(l.Item1))
                {
                    Console.WriteLine($"Error: The label {l.Item1} is used, but never defined");
                }
                ScriptDataStream.Seek(l.Item2, SeekOrigin.Begin);
                ScriptData.Write(LabelPositions[l.Item1]);
            }

            FilenameDataStream.Seek(12, SeekOrigin.Begin);
            FilenameData.Write(FilenamePositions.Count);

            ScriptDataStream.Seek(12, SeekOrigin.Begin);
            ScriptData.Write(CommandCount);

            var textdata = StringDataStream.ToArray();
            EncryptTextStream(textdata);

            File.WriteAllBytes(Path.Combine(outputfolder, "scenario.dat"), ScriptDataStream.ToArray());
            File.WriteAllBytes(Path.Combine(outputfolder, "textdata_debug.dat"), StringDataStream.ToArray());
            File.WriteAllBytes(Path.Combine(outputfolder, "textdata.dat"), textdata);
            File.WriteAllBytes(Path.Combine(outputfolder, "filename.dat"), FilenameDataStream.ToArray());
        }

        static void Main(string[] args)
        {
            if (debugpaths)
            {
                SetHandlerData();
                Process(@"D:\Dropbox\Mangagamer\PrismArk\script.txt", @"D:\Dropbox\Mangagamer\PrismArk\output");

                Console.WriteLine("DONE");
                Console.ReadKey();
            }
            else
            {
                if (args.Length < 1)
                {
                    Console.WriteLine("Usage: PrismScriptMerge inputscriptname.txt");
                    return;
                }

                var scriptname = args[0];

                var path = Path.GetDirectoryName(scriptname);

                SetHandlerData();
                Process(scriptname, path);

                Console.WriteLine("Done!");
            }
        }
    }
}
