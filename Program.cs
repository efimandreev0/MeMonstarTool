using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;

namespace MeMonstar
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args[1].Contains(".txt"))
            {
                Rebuild(args[0], args[1]);
            }
            else
            {
                Extract(args[0]);
            }
        }
        public static void Extract(string file)
        {
            var reader = new BinaryReader(File.OpenRead(file));
            int count = reader.ReadInt32();
            int[] offsets = new int[count];
            string[] strings = new string[count];
            for (int i = 0; i < count; i++)
            {
                reader.BaseStream.Position += 4;
                offsets[i] = reader.ReadInt32();
            }
            for (int i = 0; i < count; i++)
            {
                reader.BaseStream.Position = offsets[i];
                strings[i] = Utils.ReadString(reader, Encoding.Unicode);
                strings[i] = strings[i].Replace("\\n", "<lf>");
            }
            File.WriteAllLines(file + ".txt", strings);
        }
        public static void Rebuild(string oldFile, string txtFile)
        {
            var writer = new BinaryWriter(File.OpenWrite(oldFile));
            string[] strings = File.ReadAllLines(txtFile);
            int[] newOffset = new int[strings.Length];
            writer.BaseStream.Position = 0x93C;
            for (int i = 0; i < strings.Length; i++)
            {
                newOffset[i] = (int)writer.BaseStream.Position;
                writer.Write(Encoding.Unicode.GetBytes(strings[i].Replace("<lf>","\\n")));
                writer.Write(new byte());
            }
            writer.BaseStream.Position = 0x0;
            writer.Write(strings.Length);
            for (int i = 0; i < strings.Length; i++)
            {
                writer.BaseStream.Position += 4;
                writer.Write(newOffset[i]);
            }
        }
    }
}
