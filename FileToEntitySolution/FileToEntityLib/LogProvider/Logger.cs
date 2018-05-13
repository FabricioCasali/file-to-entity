using System;
using System.Collections.Generic;
using log4net;
using log4net.Repository;

namespace FileToEntityLib.LogProvider
{
    /// <summary>
    ///     Permite o registor de informações no log da aplicação.
    /// </summary>
    public static class Logger
    {
        /// <summary>
        ///     Nome do log default.
        /// </summary>
        private const string DefaultLogName = "file-to-entity";

        /// <summary>
        ///     Armazena o log default.
        /// </summary>
        private static ILog _default;

        /// <summary>
        ///     Lista de logs customizados criados na aplicação.
        /// </summary>
        private static IDictionary<string, ILoggerRepository> _loggers = new Dictionary<string, ILoggerRepository>();

        /// <summary>
        ///     Obtém o log padrão da aplicação.
        /// </summary>
        public static ILog Default
        {
            get
            {
                if (_default == null)
                {
                    _default = Log4NetFactory.Instance.GetLogger(DefaultLogName, Log4NetFactory.Instance.DefaultRepository);
                }
                return _default;
            }
        }

        public static void ChangeAppContext(this ILog log, string value)
        {
            ThreadContext.Properties["appContext"] = value;
        }

        /// <summary>
        ///     Verifica se existe um log com o nome proposto.
        /// </summary>
        /// <param name="identifier">Nome único do log.</param>
        /// <returns><code>true</code> caso exista, <code>false</code> caso contrário.</returns>
        public static bool CheckIfCustomLogExists(string identifier)
        {
            return LogManager.Exists(identifier) == null;
        }

        /// <summary>
        ///     Permite a criação de um log customizado.
        /// </summary>
        /// <param name="identifier">Nome único do log.</param>
        /// <param name="fileName">
        ///     Arquivo onde o log será salvo. O caminho pode ser relativo ("<code>./meuarquivo.log</code>")ou
        ///     absoluto ("<code>c:\meuarquivo.log</code>").
        /// </param>
        /// <exception cref="Exception">Já exista outro log criado com o mesmo nome.</exception>
        public static void CreateCustomLog(string identifier, string fileName)
        {
            var exists = LogManager.Exists(identifier);
            if (exists != null)
            {
                return;
            }
            var repository = Log4NetFactory.Instance.CreateRepository(identifier, fileName);
            Log4NetFactory.Instance.GetLogger(identifier, repository);
            _loggers.Add(identifier, repository);
        }

        /// <summary>
        ///     Obtém um log customizado previamente configurado.
        /// </summary>
        /// <param name="identifier">Nome único do log.</param>
        /// <returns>Log customizado.</returns>
        /// <exception cref="Exception">Caso log não exista.</exception>
        public static ILog CustomLog(string identifier)
        {
            if (!_loggers.ContainsKey(identifier))
            {
                throw new Exception("Não existe um log criado com o identificador " + identifier);
            }
            var loggerRepository = _loggers[identifier];
            return Log4NetFactory.Instance.GetLogger(identifier, loggerRepository);
        }

        /// <summary>
        ///     Encerra e desaloca da memória um serviço de log customizado.
        /// </summary>
        /// <param name="identifier">Nome único do log.</param>
        /// <exception cref="Exception">Caso não exista log com o identificador.</exception>
        public static void DestroyCustomLog(string identifier)
        {
            if (!CheckIfCustomLogExists(identifier))
            {
                throw new Exception("Não existe log com o nome " + identifier);
            }
            LogManager.ShutdownRepository(identifier);
        }
    }
}