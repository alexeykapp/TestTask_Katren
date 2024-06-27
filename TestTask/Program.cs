using System;
using System.Collections.Generic;
using System.Linq;

namespace TestTask
{
    public class Program
    {
        /// <summary>
        /// Программа принимает на входе 2 пути до файлов.
        /// Анализирует в первом файле кол-во вхождений каждой буквы (регистрозависимо). Например А, б, Б, Г и т.д.
        /// Анализирует во втором файле кол-во вхождений парных букв (не регистрозависимо). Например АА, Оо, еЕ, тт и т.д.
        /// По окончанию работы - выводит данную статистику на экран.
        /// </summary>
        /// <param name="args">Первый параметр - путь до первого файла.
        /// Второй параметр - путь до второго файла.</param>
        static void Main(string[] args)
        {
            Console.Write("Please enter the first file path: ");
            string filePath1 = Console.ReadLine();

            Console.Write("Please enter the second file path: ");
            string filePath2 = Console.ReadLine();

            if (string.IsNullOrEmpty(filePath1) || string.IsNullOrEmpty(filePath2))
            {
                Console.WriteLine("Both file paths are required");
                return;
            }

            using (IReadOnlyStream inputStream1 = GetInputStream(filePath1))
            {
                IList<LetterStats> singleLetterStats = FillSingleLetterStats(inputStream1);
                RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
                PrintStatistic(singleLetterStats);
            }

            using (IReadOnlyStream inputStream2 = GetInputStream(filePath2))
            {
                IList<LetterStats> doubleLetterStats = FillDoubleLetterStats(inputStream2);
                RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);
                PrintStatistic(doubleLetterStats);
            }

            Console.ReadKey();
        }

        /// <summary>
        /// Ф-ция возвращает экземпляр потока с уже загруженным файлом для последующего посимвольного чтения.
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        /// <returns>Поток для последующего чтения.</returns>
        private static IReadOnlyStream GetInputStream(string fileFullPath)
        {
            return new ReadOnlyStream(fileFullPath);
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения каждой буквы.
        /// Статистика РЕГИСТРОЗАВИСИМАЯ!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IList<LetterStats> FillSingleLetterStats(IReadOnlyStream stream)
        {
            var stats = new Dictionary<char, LetterStats>();

            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();
                if (c == ' ')
                    break;
                if (!stats.ContainsKey(c))
                {
                    stats.Add(c, new LetterStats
                    {
                        Letter = c.ToString(),
                        Count = 0
                    });
                }

                IncStatistic(stats[c]);
            }

            return new List<LetterStats>(stats.Values);
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения парных букв.
        /// В статистику должны попадать только пары из одинаковых букв, например АА, СС, УУ, ЕЕ и т.д.
        /// Статистика - НЕ регистрозависимая!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IList<LetterStats> FillDoubleLetterStats(IReadOnlyStream stream)
        {
            var stats = new Dictionary<string, LetterStats>();

            char previousChar = ' ';

            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();
                if (char.ToLower(c) == char.ToLower(previousChar))
                {
                    string pair = $"{previousChar}{c}";
                    if (!stats.ContainsKey(pair))
                    {
                        stats.Add(pair, new LetterStats
                        {
                            Letter = pair,
                            Count = 0
                        });
                    }

                    IncStatistic(stats[pair]);
                }

                previousChar = c;
            }

            return new List<LetterStats>(stats.Values);
        }

        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>
        private static void RemoveCharStatsByType(IList<LetterStats> letters, CharType charType)
        {
            switch (charType)
            {
                case CharType.Consonants:
                    letters.RemoveAll(stat => StringPairChecker.IsConsonantPair(stat.Letter));
                    break;
                case CharType.Vowel:
                    letters.RemoveAll(stat => StringPairChecker.IsVowelPair(stat.Letter));
                    break;
            }
        }

        /// <summary>
        /// Ф-ция выводит на экран полученную статистику в формате "{Буква} : {Кол-во}"
        /// Каждая буква - с новой строки.
        /// Выводить на экран необходимо предварительно отсортировав набор по алфавиту.
        /// В конце отдельная строчка с ИТОГО, содержащая в себе общее кол-во найденных букв/пар
        /// </summary>
        /// <param name="letters">Коллекция со статистикой</param>
        private static void PrintStatistic(IEnumerable<LetterStats> letters)
        {
            var sortedLetters = new List<LetterStats>(letters);
            sortedLetters.Sort();

            int total = 0;
            foreach (var stat in sortedLetters)
            {
                Console.WriteLine($"{stat.Letter} : {stat.Count}");
                total += stat.Count;
            }

            Console.WriteLine($"ИТОГО: {total}");
        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// </summary>
        /// <param name="letterStats"></param>
        private static void IncStatistic(LetterStats letterStats)
        {
            letterStats.Count++;
        }
    }
}