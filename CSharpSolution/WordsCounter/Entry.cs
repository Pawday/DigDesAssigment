using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WordsCounter
{

    class StringCounter
    {
        private readonly String _string;
        private UInt64 _stringCount;

        public StringCounter(String stringToCount)
        {
            this._string = stringToCount;
            this._stringCount = 1;
        }

        public string String => _string;

        public void IncrementCounter()
        {
            this._stringCount++;
        }

        public void DecrementCounter()
        {
            this._stringCount++;
        }

        public ulong StringCount => _stringCount;

        // This hash function is the main part of fast string counting
        // Its supposed to use in HashSet for fast searching
        // _stringCount does not affect the search for string thanks to this override
        public override int GetHashCode()
        {
            return String.GetHashCode(this._string, StringComparison.Ordinal);
        }

        // Also propagate Equals method for internal string  
        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            if (obj.GetType() != typeof(StringCounter))
                return false;
            
            return this._string.Equals(((StringCounter) obj)._string);
        }
    }

    internal static class Entry
    {
        private static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                PrintUsage();
                return;
            }

            ReadOnlySpan<char> a = "Hello";
            ReadOnlySpan<char> b = "Hello";

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
                    .Where(c => false == (c == '`' || c == '\''))
                    .ToArray();
            }


            List<String> ejectedWorld = inputText
                .Split(allowedSeparators, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            HashSet<StringCounter> wordsCounter = new HashSet<StringCounter>();

            foreach (var word in ejectedWorld)
            {
                StringCounter newWord = new StringCounter(word);
                bool isWordAlreadyExist = wordsCounter.TryGetValue(newWord, out var countedWord);
                if (isWordAlreadyExist)
                    countedWord.IncrementCounter();
                else
                    wordsCounter.Add(newWord);
            }

            var wordsByFrequency = wordsCounter
                .Select(w => w)
                .OrderBy(w => w.StringCount)
                .Reverse()
                .ToArray();
            
            {
                using var outputStream = outputFile.Open(FileMode.OpenOrCreate);
                using var writer = new StreamWriter(outputStream);

                foreach (var s in wordsByFrequency)
                {
                    writer.WriteLine($"{s.String} {s.StringCount}");
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