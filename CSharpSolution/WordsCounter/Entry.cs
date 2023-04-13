using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace WordsCounter
{
    internal static class Entry
    {
        private static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                PrintUsage();
                return;
            }

            var inputFile = new FileInfo(args[0]);
            var outputFile = new FileInfo(args[1]);
            
            String inputText;

            {
                using var inputStream = inputFile.Open(FileMode.Open);
                using var utf8InputReader = new StreamReader(inputStream, Encoding.UTF8);
                inputText = utf8InputReader.ReadToEnd();
            }

            {
                using var outputStream = outputFile.Open(FileMode.OpenOrCreate);
                StreamWriter writer = new StreamWriter(outputStream);

                inputText
                    .Split('\n')
                    .SelectMany(s => s.Split(new char[] {'\r','\n', ' '}, StringSplitOptions.RemoveEmptyEntries))
                    .Select(SubstituteWord)
                    .Select(s => new Tuple<String, int>(s,s.Length))
                    .Where(pair => pair.Item2 != 0)
                    .Select(pair => pair.Item1.Contains('\n') ? new Tuple<String,int>("",666) :  pair)
                    .ToList()
                    .ForEach(s => writer.WriteLine("{" + s.Item1 + " | " + s.Item2 + "}"));
                    // .ForEach(s => writer.WriteLine(s.Item1));
                    

                writer.Flush();
            }
        }

        private static String SubstituteWord(String input)
        {
            if (input.Length == 0)
                return input;
            
            var stringLen = input.Length;
            
            int stringStartTrim = 0;
            int stringEndTrim = 0;
            bool wordStartFound = false;
            bool wordEndFound = false;

            for (var index = 0; index < stringLen / 2 && false == (wordStartFound && wordEndFound); ++index)
            {
                if (false == Char.IsLetter(input[index]) && false == wordStartFound)
                    ++stringStartTrim;
                else
                    wordStartFound = true;
                
                if (false == Char.IsLetter(input[stringLen - index - 1]) && false == wordEndFound)
                    ++stringEndTrim;
                else
                    wordEndFound = true;
            }

            return input.Substring(stringStartTrim, stringLen - stringEndTrim - stringStartTrim);
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("(executable_name) <input file> <output file>");
        }
    }
}