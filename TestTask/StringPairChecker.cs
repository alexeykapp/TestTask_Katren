using System.Collections.Generic;
using System.Linq;

namespace TestTask
{
    public static class StringPairChecker
    {
        private static readonly HashSet<char> vowels = new HashSet<char>() { 'а', 'е', 'ё', 'и', 'о', 'у', 'ы', 'э', 'ю', 'я' };

        public static bool IsVowelPair(string str)
        {
            return str.All(c => vowels.Contains(char.ToLower(c)));
        }

        public static bool IsConsonantPair(string str)
        {
            return str.All(c => !vowels.Contains(char.ToLower(c)));
        }
    }
}