using System;
using System.Collections.Generic;
using System.IO;
using Excel;

namespace FileToEntityLib
{
    public class ExcelParser : ParserEngine
    {
        private IDictionary<long, string[]> _allLines;

        private long _currentLine;

        private string _filePath;

        public ExcelParser(string filePath) : base(FileType.Excel)
        {
            _filePath = filePath;
            _allLines = new Dictionary<long, string[]>();
        }

        protected override bool GetNextSource(out string[] entry, out long line)
        {
            _currentLine++;
            if (!_allLines.ContainsKey(_currentLine))
            {
                entry = null;
                line = 0;
                return false;
            }
            entry = _allLines[_currentLine];
            line = _currentLine;
            return true;
        }

        protected override void Prepare()
        {
            Log.Debug($"preparando leitura do arquivo {_filePath}. Verificando se arquivo existe");
            var fileInfo = new FileInfo(_filePath);
            if (!fileInfo.Exists)
            {
                Log.Error($"arquivo não existe: {fileInfo.Name}");
                throw new FileNotFoundException($"Arquivo {_filePath} não existe ou não pode ser acessado.");
            }
            Log.Debug($"arquivo encontrado, abrindo");
            var stream = new FileStream(_filePath, FileMode.Open);
            IExcelDataReader reader;
            Log.Debug($"verificando extensao do arquivo para determinar como o mesmo deve ser aberto: {fileInfo.Extension}");
            if (fileInfo.Extension.Equals(".xls", StringComparison.CurrentCultureIgnoreCase))
            {
                Log.Debug("abrindo arquivo em formato binario");
                reader = ExcelReaderFactory.CreateBinaryReader(stream);
            }
            else
            {
                Log.Debug("abrindo arquivo em formato xml");
                reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            }
            var line = 0;
            while (reader.Read())
            {
                line++;
                var lineArray = new string[reader.FieldCount];
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    lineArray[i] = reader.GetString(i);
                }
                _allLines.Add(line, lineArray);
            }
            reader.Close();
            _currentLine = 0;
        }
    }
}