using System;
using System.IO;
using System.Security;

namespace FileToEntityLib
{
    public class PositionalParser : ParserEngine
    {
        private int _currentLine;
        private string[] _lines;

        public PositionalParser(string filePath) : base(FileType.Positional)
        {
            FilePath = filePath;
        }

        /// <summary>
        ///     Método abstrato que é chamado para obter o próximo segmento de conteúdo para ser processado.
        /// </summary>
        /// <param name="entry">Parâmetro <c>output</c> com os dados da saída.</param>
        /// <param name="line">Parâmetro <c>output</c> com o índice da linha que esta sendo processado.</param>
        /// <returns><c>True</c> caso ainda tenha dados a processar, <c>false</c> caso contrário</returns>
        protected override bool GetNextSource(out string[] entry, out long line)
        {
            if (_lines == null)
            {
                throw new Exception("Sem dados");
            }
            line = _currentLine;
            line++;
            try
            {
                entry = new[] {_lines[_currentLine]};
                _currentLine++;
                return true;
            }
            catch (Exception)
            {
                entry = null;
                return false;
            }
        }

        /// <summary>
        ///     Método chamado antes do início do processamento do arquivo.
        /// </summary>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="SecurityException"></exception>
        protected override void Prepare()
        {
            var file = new FileInfo(FilePath);
            if (!file.Exists)
            {
                throw new ParserException($"Arquivo {FilePath} não encontrado");
            }
            FileSize = file.Length;
            _lines = File.ReadAllLines(FilePath, Encoding);
            _currentLine = 0;
        }
    }
}