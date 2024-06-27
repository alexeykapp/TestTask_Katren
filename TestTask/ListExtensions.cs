using System;
using System.Collections.Generic;

namespace TestTask
{
    public static class ListExtensions
    {
        public static void RemoveAll(this IList<LetterStats> letters, Func<LetterStats, bool> predicate)
        {
            for (int i = 0; i < letters.Count; i++)
            {
                if (predicate(letters[i]))
                {
                    letters.RemoveAt(i);
                }
            }
        }
    }
}