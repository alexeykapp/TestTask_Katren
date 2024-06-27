using System;

namespace TestTask
{
    /// <summary>
    /// Статистика вхождения буквы/пары букв
    /// </summary>
    public class LetterStats : IComparable<LetterStats>
    {
        /// <summary>
        /// Буква/Пара букв для учёта статистики.
        /// </summary>
        public string Letter;

        /// <summary>
        /// Кол-во вхождений буквы/пары.
        /// </summary>
        public int Count;

        public int CompareTo(LetterStats letter)
        {
            char a = char.ToLower(Letter[0]);
            char b = char.ToLower(letter.Letter[0]);
            if (a == b)
            {
                return 0;
            }

            if (a < b)
            {
                return -1;
            }

            return 1;
        }
    }
}