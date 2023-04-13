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

            char[] discoveredWhitespaces;
            
            {
                var discoveredWhitespacesList = new HashSet<char>();

                foreach (var c in inputText)
                {
                    discoveredWhitespacesList.Add(c);
                }
                discoveredWhitespaces = discoveredWhitespacesList
                    .Select(c => c)
                    .Where(c => false == Char.IsLetter(c))
                    .Where(c => false == Char.IsNumber(c))
                    .Where(c => false == (c == '`' || c == '\'' || c == '\''))
                    .ToArray();
            }


            List<String> ejectedWorld = inputText
                .Split(discoveredWhitespaces, StringSplitOptions.RemoveEmptyEntries)
                .Select(EjectWord)
                .Select(s => new Tuple<String, int>(s,s.Length))
                .Where(pair => pair.Item2 != 0)
                .Select(pair => pair.Item1.Contains('\n') ? new Tuple<String,int>("",666) :  pair)
                .Select(pair => pair.Item1)
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

        // Examples: <input> -> <output>
        //
        // "__WordWithPrefix" -> "WordWithPrefix"
        // "WordWithSuffix_?" -> "WordWithSuffix"
        // "__WordWithSuffixAndPrefix?__" -> "WordWithSuffixAndPrefix"
        // "123432" -> "123432"
        // "_midl'aworda'modimo?" -> "midl'aworda'modimo"
        // "_==withNumeric123--3" -> "withNumeric123"
        // "_a_" -> "a"
        // "Scherer".[3]" -> "Scherer".[3"
        //
        //
        //from http://az.lib.ru/t/tolstoj_lew_nikolaewich/text_0040.shtml
        // "Qu'a-t-on" -> "Qu'a-t-on" 
        // "дело?...После" -> "дело?...После" 
        //
        // "Lui;que" -> "Lui;que"
        // "ridicule.Et" -> "ridicule.Et"
        // "стыдно?Да" -> "стыдно?Да"
        // "усталый(несмотря" -> "усталый(несмотря"
        //
        // "вчег'а" -> "вчег'а"
        // "дег'нул" -> "дег'нул"
        // "Сквег'но" -> "Сквег'но"
        // "Пг`авда" -> "Пг`авда"
        private static String EjectWord(String input)
        {
            if (input.Length == 0)
                return "";

            var stringLen = input.Length;
            
            int stringStartTrimOffset = 0;
            int stringEndTrimOffset = 0;
            bool wordStartFound = false;
            bool wordEndFound = false;

            for (var index = 0; index < stringLen && false == (wordStartFound && wordEndFound); ++index)
            {
                var frontChar = input[index];
                var endChar = input[stringLen - index - 1];

                if (false == Char.IsLetter(frontChar) && false == Char.IsDigit(frontChar) && false == wordStartFound)
                    ++stringStartTrimOffset;
                else
                    wordStartFound = true;
                
                if (false == Char.IsLetter(endChar) && false == Char.IsDigit(endChar) && false == wordEndFound)
                    ++stringEndTrimOffset;
                else
                    wordEndFound = true;
            }

            if (!wordStartFound && !wordEndFound)
                return "";

            var wordLen = (stringLen - stringEndTrimOffset) - stringStartTrimOffset;

            var returnVal = input.Substring(stringStartTrimOffset, wordLen);
            
            return returnVal;
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("(executable_name) <input file> <output file>");
        }
    }
}