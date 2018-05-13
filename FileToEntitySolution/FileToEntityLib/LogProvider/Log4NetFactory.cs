using System;
using System.Text;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository;
using log4net.Repository.Hierarchy;

namespace FileToEntityLib.LogProvider
{
    /// <summary>
    ///     Classe utilitária para configurar log de eventos do log4net
    /// </summary>
    public class Log4NetFactory
    {
        /// <summary>
        ///     Nome do repositório default.
        /// </summary>
        private const string DefaultRepositoryName = "ENGINE";

        /// <summary>
        ///     Pattern do arquivo de log.
        /// </summary>
        private const string LogPattern = "%date [%thread] %-5level [%property{appContext}] - %message%newline";

        /// <summary>
        ///     Singleton
        /// </summary>
        private static Log4NetFactory _instance;

        /// <summary>
        ///     Construtor privado para iniciar o singleton.
        /// </summary>
        private Log4NetFactory()
        {
            DefaultRepository = CreateRepository(DefaultRepositoryName, "./application.log");
        }

        /// <summary>
        ///     Obtém a instância única do gerador.
        /// </summary>
        public static Log4NetFactory Instance
        {
            get { return _instance ?? (_instance = new Log4NetFactory()); }
        }

        /// <summary>
        ///     Repositório default do log.
        /// </summary>
        public ILoggerRepository DefaultRepository { get; private set; }

        /// <summary>
        ///     Cria um novo repositório.
        /// </summary>
        /// <param name="repositoryName">Nome do repositório.</param>
        /// <param name="outputFile">Arquivo de saída do log.</param>
        /// <returns>Repositório criado.</returns>
        public ILoggerRepository CreateRepository(string repositoryName, string outputFile)
        {
            return CreateRepository(repositoryName, outputFile, null);
        }

        public ILoggerRepository CreateRepository(string repositoryName, string outputFile, string pattern)
        {
            Hierarchy repository = null;
            try
            {
                repository = (Hierarchy)LogManager.GetRepository(repositoryName);
            }
            catch (Exception)
            {
                repository = (Hierarchy)LogManager.CreateRepository(repositoryName);
            }
            var root = repository.Root;
            root.AddAppender(ConfigureRollingAppender(outputFile, pattern));
            root.AddAppender(ConfigureConsoleAppender(pattern));
            root.AddAppender(ConfigureTraceAppender(pattern));
            root.Level = Level.All;
            repository.Configured = true;
            ThreadContext.Properties["appContext"] = "Main";
            return repository;
        }

        /// <summary>
        ///     Obtém (ou cria, caso não exista) o logger de determinado repositório.
        /// </summary>
        /// <param name="name"> Nome do log.</param>
        /// <param name="repository">Repositório que irá armazenar o log</param>
        /// <returns>Logger configurado.</returns>
        public ILog GetLogger(string name, ILoggerRepository repository)
        {
            var log = LogManager.GetLogger(repository.Name, name);
            return log;
        }

        public void SetThreadContext(string context, string value)
        {
            ThreadContext.Properties[context] = value;
        }

        /// <summary>
        ///     Cria a pattern de compilação.
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns>Layout criado.</returns>
        private PatternLayout BuildPattern(string pattern)
        {
            var patternLayout = new PatternLayout();
            if (pattern == null)
            {
                patternLayout.ConversionPattern = LogPattern;
            }
            else
            {
                patternLayout.ConversionPattern = pattern;
            }
            patternLayout.ActivateOptions();
            return patternLayout;
        }

        /// <summary>
        ///     Configura o appender no console.
        /// </summary>
        /// <returns>Appender configurado.</returns>
        private AppenderSkeleton ConfigureConsoleAppender(string pattern)
        {
            var console = new ConsoleAppender();
            console.Layout = BuildPattern(pattern);
            console.Target = "Console.Out";
            console.ActivateOptions();
            return console;
        }

        /// <summary>
        ///     Configura o appender em arquivo.
        /// </summary>
        /// <param name="outputFile">Caminho onde o arquivo será salvo.</param>
        /// <param name="pattern"></param>
        /// <returns>Appender configurado.</returns>
        private AppenderSkeleton ConfigureRollingAppender(string outputFile, string pattern)
        {
            var roller = new RollingFileAppender();
            roller.Layout = BuildPattern(pattern);
            roller.AppendToFile = true;
            roller.PreserveLogFileNameExtension = true;
            roller.RollingStyle = RollingFileAppender.RollingMode.Composite;
            roller.MaxSizeRollBackups = 20;
            roller.CountDirection = 1;
            roller.MaximumFileSize = "50MB";
            roller.StaticLogFileName = false;
            roller.File = outputFile;
            roller.Encoding = Encoding.GetEncoding("ISO-8859-1");
            roller.LockingModel = new FileAppender.MinimalLock();
            roller.ActivateOptions();
            return roller;
        }

        /// <summary>
        ///     Configura o appender na console de desenvolvimento.
        /// </summary>
        /// <returns>Appender configurado.</returns>
        private AppenderSkeleton ConfigureTraceAppender(string pattern)
        {
            var trace = new TraceAppender();
            trace.Layout = BuildPattern(pattern);
            trace.ImmediateFlush = true;
            trace.ActivateOptions();
            return trace;
        }
    }
}