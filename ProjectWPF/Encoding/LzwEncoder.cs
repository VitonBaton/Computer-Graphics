using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ProjectWPF.Encoding
{
    public static class LzwEncoder
    {

        public static IEnumerable<string> GenerateStartDictionary(string text)
        {
            var set = new SortedSet<char>();
            set.UnionWith(text);
            return set.Select(symbol => symbol.ToString());
        }
        
        public static string Encode(string text, IEnumerable<string> startTable)
        {
            var result = new StringBuilder();

            //var set = new SortedSet<char>();
            //set.UnionWith(text);
            //var table = set.Select(symbol => symbol.ToString()).ToList();

            var table = startTable.Select(c => c).ToList();
    
            var currentString = string.Empty;
            var nextString = new StringBuilder();

            foreach (var symbol in text)
            {
                nextString.Append(symbol);

                if (table.Contains(nextString.ToString()))
                {
                    currentString = nextString.ToString();
                }
                else
                {
                    result.Append(table.IndexOf(currentString) + " ");
                    table.Add(nextString.ToString());
                    nextString.Clear();
                    nextString.Append(symbol);
                    currentString = symbol.ToString();
                }
            }

            result.Append(table.IndexOf(currentString));
            return result.ToString();
        }

        public static string Decode(string text, IEnumerable<string> startTable)
        {
            var result = new StringBuilder();

            var table = startTable.Select(c => c).ToList();
    
            var currentString = string.Empty;
            var nextString = new StringBuilder();

            var indexes = text.Split(' ').Select(s => int.Parse(s));

            var lastIndex = -1;
            foreach (var index in indexes)
            {
                try
                {
                    nextString.Append(index == table.Count ? nextString.ToString()[0] : table[index][0]);

                    if (table.Contains(nextString.ToString()))
                    {
                        currentString = nextString.ToString();
                    }
                    else
                    {
                        result.Append(currentString);
                        table.Add(nextString.ToString());
                        nextString.Clear();
                        nextString.Append(table[index]);
                        currentString = nextString.ToString();
                    }

                    lastIndex = index;
                }
                catch (ArgumentOutOfRangeException)
                {
                    throw new ArgumentOutOfRangeException("Ошибка в коде или словаре");
                }
            }

            result.Append(table[lastIndex]);
            return result.ToString();
        }
    }
}