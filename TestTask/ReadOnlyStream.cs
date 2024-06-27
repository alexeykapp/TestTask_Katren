using System;
using System.IO;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private StreamReader _localStream;

        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            if (string.IsNullOrEmpty(fileFullPath))
            {
                throw new ArgumentException("File path cannot be null or empty", nameof(fileFullPath));
            }

            _localStream = new StreamReader(fileFullPath);
            IsEof = false;
        }

        /// <summary>
        /// Флаг окончания файла
        /// </summary>
        public bool IsEof { get; private set; }

        /// <summary>
        /// Ф-ция чтения следующего символа из потока.
        /// Если произведена попытка прочитать символ после достижения конца файла, метод 
        /// должен бросать соответствующее исключение
        /// </summary>
        /// <returns>Считанный символ.</returns>
        public char ReadNextChar()
        {
            while (true)
            {
                int nextChar = _localStream.Read();
                if (nextChar == -1)
                {
                    IsEof = true;
                    return ' ';
                }

                if (char.IsLetter((char)nextChar))
                {
                    return (char)nextChar;
                }
            }
        }

        /// <summary>
        /// Сбрасывает текущую позицию потока на начало
        /// </summary>
        public void ResetPositionToStart()
        {
            if (_localStream == null)
            {
                IsEof = true;
                return;
            }

            _localStream.BaseStream.Seek(0, SeekOrigin.Begin);
            _localStream.DiscardBufferedData();
            IsEof = false;
        }

        public void Dispose()
        {
            if (_localStream != null)
            {
                try
                {
                    _localStream.Dispose();
                }
                finally
                {
                    _localStream = null;
                }
            }
        }
    }
}