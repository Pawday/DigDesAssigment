using System;
using System.Collections.Generic;
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

            char[] allowedSeparators;
            
            {
                var allowedSeparatorsAsSet = new HashSet<char>();

                foreach (var c in inputText)
                {
                    allowedSeparatorsAsSet.Add(c);
                }
                allowedSeparators = allowedSeparatorsAsSet
                    .Select(c => c)
                    .Where(c => false == Char.IsLetter(c))
                    .Where(c => false == Char.IsNumber(c))
                    .Where(c => false == (c == '`' || c == '\'' || c == '\''))
                    .ToArray();
            }


            List<String> ejectedWorld = inputText
                .Split(allowedSeparators, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            {
                using var outputStream = outputFile.Open(FileMode.OpenOrCreate);
                using var writer = new StreamWriter(outputStream);

                foreach (var s in ejectedWorld)
                {
                    writer.WriteLine(s);
                }
            }
            
            
            
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("(executable_name) <input file> <output file>");
        }
    }
}