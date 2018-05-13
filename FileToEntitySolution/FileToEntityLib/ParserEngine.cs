using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using FileToEntityLib.Column;
using FileToEntityLib.Extensios;
using FileToEntityLib.LogProvider;
using FileToEntityLib.Positional;
using log4net;

namespace FileToEntityLib
{
    public abstract class ParserEngine
    {
        protected ILog Log;
        private IDictionary<string, string> _cacheDictionary;
        private FileType _fileType;
        private IDictionary<string, Type> _typeDictionary;

        protected internal ParserEngine(FileType fileType)
        {
            _fileType = fileType;
            _typeDictionary = new Dictionary<string, Type>();
            _cacheDictionary = new Dictionary<string, string>();
            Rules = new List<IRule>();
            Encoding = Encoding.Default;
            var fileGuid = Guid.NewGuid();
            var loggerName = string.Format($"_parse_engine_{fileGuid}");
            Logger.CreateCustomLog(loggerName, $"./file-to-entity-{fileGuid}.log");
            Log = Logger.CustomLog(loggerName);
        }

        public string FilePath { get; set; }

        public long FileSize { get; set; }

        /// <summary>
        ///     Obtém o tipo do arquivo que esta sendo processado.
        /// </summary>
        /// <value>
        ///     <see cref="FileToEntityLib.FileType" /> do arquivo processado.
        /// </value>
        public FileType FileType
        {
            get { return _fileType; }
        }

        public ICollection<IRule> Rules { get; set; }

        /// <summary>
        ///     Obtém ou atribui o encoding utilizado para a leitura do arquivo. Default
        ///     <see cref="System.Text.Encoding.Default" />
        /// </summary>
        /// <value>
        ///     <see cref="System.Text.Encoding" /> a ser utilizado
        /// </value>
        protected Encoding Encoding { get; set; }

        public int AddRule(IRule rule)
        {
            rule.Order = Rules.Count == 0 ? 1 : Rules.Max(p => p.Order) + 1;
            Rules.Add(rule);
            return rule.Order;
        }

        /// <summary>
        ///     Obtém uma lista contendo todos os objetos de regra existentes na estrutura.
        /// </summary>
        /// <param name="i">Nível da hierarquia a consultar</param>
        /// <returns>Lista de <see cref="Rule" /> encontrados no grafo</returns>
        public ICollection<Rule> GetAllRules(int i)
        {
            return GetAllRules(Rules.OrderBy(p => p.Order).ToList(), i, 0);
        }

        /// <summary>
        ///     Obtém uma lista contendo todos os objetos de regra existentes na estrutura.
        /// </summary>
        /// <returns>Lista de <see cref="Rule" /> encontrados no grafo</returns>
        public ICollection<Rule> GetAllRules()
        {
            return GetAllRules(Rules.OrderBy(p => p.Order).ToList(), 9999, 0);
        }

        public ParseResult Parse()
        {
            Contract.Requires(FilePath != null);
            var start = DateTime.Now;
            Log.InfoFormat("Iniciando processamento do arquivo");

            // Chama rotina para ler o arquivo e montar em memoria o array, que irá disponibilizar os registros.
            Prepare();
            string[] content = null;
            long register = 0;

            var parseResult = new ParseResult
            {
                FileName = FilePath,
                Size = FileSize
            };

            EnumerateProcessTree();

            while (GetNextSource(out content, out register))
            {
                Log.ChangeAppContext("MAIN");
                Log.Debug(
                    $"obtido registro {register} para processar: \"{content.Aggregate((s, s1) => s + "||" + (string.IsNullOrWhiteSpace(s1) ? "NULL" : s1))}\"");
                ICollection<ParseEntity> createdObjectsList = new List<ParseEntity>();
                foreach (var rule in Rules)
                {
                    try
                    {
                        var abort = ProcessRule(rule, content, createdObjectsList, register);
                        if (abort)
                        {
                            Log.Debug("processamento das demais regras abortado.");
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        var entry = new ParserResultEntry();
                        entry.Errors.Add(
                            new ParserError
                            {
                                ErrorMessage = e.Message,
                                ParserResultEntry = entry
                            });
                        Log.Error(
                            $"adicionando registro de erro de processamento ao objeto de saída: {entry.Errors.FirstOrDefault()}");
                        entry.ParseResult = parseResult;
                        parseResult.Entries.Add(entry);
                    }
                }
                if (createdObjectsList.Count > 0)
                {
                    Log.Debug(
                        $"regras geraram {createdObjectsList.Count} objetos, registrando objetos na lista de saída da aplicação");
                    foreach (var o in createdObjectsList)
                    {
                        Log.Debug($"objeto gerado: {o}");
                    }
                    var entry = new ParserResultEntry();
                    foreach (var parseEntity in createdObjectsList)
                    {
                        entry.Add(parseEntity);
                    }
                    ;
                    entry.Register = register;
                    entry.ParseResult = parseResult;
                    parseResult.Entries.Add(entry);
                }
                Log.Debug($"{new String(':', 100)}");
            }

            parseResult.RegistersInFile = register;

            Log.Info(
                $"processamento do arquivo concluído em {DateTime.Now - start} ms. obtido {parseResult.Entries.Count} resultados");
            return parseResult;
        }

        /// <summary>
        ///     Método abstrato que é chamado para obter o próximo segmento de conteúdo para ser processado.
        /// </summary>
        /// <param name="entry">Parâmetro <c>output</c> com os dados da saída.</param>
        /// <param name="line">Parâmetro <c>output</c> com o índice da linha que esta sendo processado.</param>
        /// <returns><c>True</c> caso ainda tenha dados a processar, <c>false</c> caso contrário</returns>
        protected abstract bool GetNextSource(out string[] entry, out long line);

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
        /// <exception cref="ParserException"></exception>
        protected abstract void Prepare();

        private static ICollection<Rule> GetAllRules(List<IRule> rules, int i, int current)
        {
            var result = new List<Rule>();
            current++;
            if (current > i)
            {
                return result;
            }
            foreach (var rule in rules)
            {
                result.Add((Rule)rule);
                if (rule is ICondition<IRule>)
                {
                    var entries = ((ICondition<Rule>)rule).Actions.ToList();
                    result.AddRange(GetAllRules(entries, i, current));
                }
            }
            return result;
        }

        private static int GetMonthNumber(string monthDescription)
        {
            monthDescription = monthDescription.Trim().ToUpper();
            if (monthDescription.Equals("JANEIRO") || monthDescription.Equals("JAN"))
                return 1;
            if (monthDescription.Equals("FEVEREIRO") || monthDescription.Equals("FEV"))
                return 2;
            if (monthDescription.Equals("MARÇO") || monthDescription.Equals("MAR") || monthDescription.Equals("MARCO"))
                return 3;
            if (monthDescription.Equals("ABRIL") || monthDescription.Equals("ABR"))
                return 4;
            if (monthDescription.Equals("MAIO") || monthDescription.Equals("MAI"))
                return 5;
            if (monthDescription.Equals("JUNHO") || monthDescription.Equals("JUN"))
                return 6;
            if (monthDescription.Equals("JULHO") || monthDescription.Equals("JUL"))
                return 7;
            if (monthDescription.Equals("AGOSTO") || monthDescription.Equals("AGO"))
                return 8;
            if (monthDescription.Equals("SETEMBRO") || monthDescription.Equals("SET"))
                return 9;
            if (monthDescription.Equals("OUTUBRO") || monthDescription.Equals("OUT"))
                return 10;
            if (monthDescription.Equals("NOVEMBRO") || monthDescription.Equals("NOV"))
                return 11;
            if (monthDescription.Equals("DEZEMBRO") || monthDescription.Equals("DEZ"))
                return 12;
            return 0;
        }

        private static bool ProcessComparissonCriterias(OperatorType operatorType, ICondition<IRule> condition,
                    string value)
        {
            if (condition.CastType == CastType.Long)
            {
                long sourceValue, targetValue = 0;
                try
                {
                    sourceValue = Convert.ToInt64(value);
                }
                catch (Exception)
                {
                    throw new FormatException($"Valor de origem {value} não pode ser convertido para um valor numérico");
                }
                try
                {
                    targetValue = Convert.ToInt64(condition.Value);
                }
                catch (Exception)
                {
                    throw new FormatException($"Valor de origem {value} não pode ser convertido para um valor numérico");
                }
                switch (operatorType)
                {
                    case OperatorType.Greater:
                        return sourceValue > targetValue;

                    case OperatorType.GreaterOrEqual:
                        return sourceValue >= targetValue;

                    case OperatorType.Lesser:
                        return sourceValue < targetValue;

                    case OperatorType.LesserOrEqual:
                        return sourceValue <= targetValue;
                }
            }
            else
            {
                var arrayItems = new[] { value, condition.Value };
                Array.Sort(arrayItems);
                switch (operatorType)
                {
                    case OperatorType.Greater:
                    case OperatorType.GreaterOrEqual:
                        return arrayItems[0] == value;

                    case OperatorType.Lesser:
                    case OperatorType.LesserOrEqual:
                    default:
                        return arrayItems[1] == value;
                }
            }
            //Nunca deve chegar neste ponto
            return false;
        }

        /// <summary>
        ///     Constrói a árvore para processamento das regras.
        /// </summary>
        /// <param name="level">Nível atual na lista</param>
        /// <param name="rules">Regras a processar neste nível</param>
        /// <param name="father">Item "pai"</param>
        /// <returns>String contendo a representação deste nível da árvore.</returns>
        private string BuildProcessTree(int level, IList<IRule> rules, IRule father)
        {
            var tree = new StringBuilder("");
            for (var index = 0; index < rules.Count; index++)
            {
                var rule = rules[index];
                var d = "";
                if (level == 0)
                {
                    d = "*";
                }
                else if (level - 1 == 0)
                {
                    d = "|" + new string('_', level * 4);
                }
                else
                {
                    for (var i = 0; i < level - 1; i++)
                    {
                        d += "|" + new string('_', 4);
                    }
                }
                var currentStructure = string.Format("{0:00}", rule.Order);
                if (father != null)
                {
                    currentStructure = $"{father.ExecutionStructure}.{currentStructure}";
                }
                rule.ExecutionStructure = currentStructure;
                tree = tree.Append($"{d}{rule}\n");
                if (rule is ICondition<IRule> && ((ICondition<IRule>)rule).Actions.Count > 0)
                {
                    var condition = rule as ICondition<IRule>;
                    tree =
                        tree.Append(BuildProcessTree(level + 1, condition.Actions.OrderBy(p => p.Order).ToList(), rule));
                }
            }
            return tree.ToString();
        }

        private void EnumerateProcessTree()
        {
            Log.Debug("\n" + BuildProcessTree(0, Rules.OrderBy(p => p.Order).ToList(), null));
        }

        /// <summary>
        ///     Formata o valor de acordo com o tipo do dado.
        /// </summary>
        /// <param name="valueDatatype">Tipo do dado.</param>
        /// <param name="substring">Valor a ser formatado.</param>
        /// <param name="value">Máscara a ser aplicada no momento do format.</param>
        /// <param name="customMask">Máscara customizada a ser aplicada.<seealso cref="CustomMask" /></param>
        /// <returns></returns>
        private object FormatValue(Type valueDatatype, string value, string valueMask, CustomMask customMask)
        {
            object data = null;
            if (valueDatatype.IsAssignableFrom(typeof(DateTime)))
            {
                data = DateTime.ParseExact(value, valueMask, CultureInfo.CurrentCulture);
            }
            else if (valueDatatype.IsEnum)
            {
                var enumValue = valueDatatype.GetEnumValues();
                foreach (var x in enumValue)
                {
                    if (x.ToString().Equals(value))
                    {
                        return x;
                    }
                }
            }
            else if (valueDatatype.IsAssignableFrom(typeof(Period)))
            {
                if (customMask == CustomMask.CompleteMonthYear)
                {
                    data = new Period
                    {
                        Month = GetMonthNumber(value.Split('/')[0]),
                        Year = Convert.ToInt32(value.Split('/')[1])
                    };
                }
            }
            else if (valueDatatype.IsAssignableFrom(typeof(int)))
            {
                try
                {
                    if (customMask == CustomMask.MonthAsText)
                    {
                        data = GetMonthNumber(value);
                    }
                    else
                    {
                        data = Convert.ToInt32(value);
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"erro durante a conversao do objeto {value} para o type {valueDatatype}");
                }
            }
            else if (valueDatatype.IsAssignableFrom(typeof(long)))
            {
                data = Convert.ToInt64(value);
            }
            else if (valueDatatype.IsAssignableFrom(typeof(bool)))
            {
                data = value.EqualsIgnoreCase("TRUE") ||
                       value.EqualsIgnoreCase("T") ||
                       value.EqualsIgnoreCase("S") ||
                       value.EqualsIgnoreCase("SIM") ||
                       value.EqualsIgnoreCase("YES") ||
                       value.EqualsIgnoreCase("Y") ||
                       value.EqualsIgnoreCase("1");
            }
            else if (valueDatatype.IsAssignableFrom(typeof(decimal)))
            {
                if (customMask == CustomMask.Monetary)
                {
                    var provider = new NumberFormatInfo
                    {
                        NumberDecimalDigits = 6,
                        NumberDecimalSeparator = ",",
                        NumberGroupSeparator = "."
                    };
                    data = Convert.ToDecimal(Convert.ToDecimal(value, provider));
                }
                else
                {
                    data = Convert.ToDecimal(Convert.ToDecimal(value).ToString(valueMask));
                }
            }
            else
            {
                data = value;
            }
            return data;
        }

        /// <summary>
        ///     Procura em todas as libs carregadas no <see cref="AppDomain.CurrentDomain" />
        ///     <see cref="Type" /> com o nome passado por parâmetro
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <returns></returns>
        private Type GetTypeFromName(string typeName)
        {
            Type genericType = null;
            if (_typeDictionary.ContainsKey(typeName))
            {
                _typeDictionary.TryGetValue(typeName, out genericType);
                return genericType;
            }
            Log.Debug(
                $"type {typeName} ainda não é conhecido, procurando o type nas bibliotecas disponíveis na aplicação.");
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                genericType = (from p in assembly.GetTypes()
                               where p.FullName.Equals(typeName)
                               select p).SingleOrDefault();
                if (genericType != null)
                {
                    Log.Debug($"Type foi encontrado na assembly {assembly.FullName}");
                    break;
                }
            }
            if (genericType == null)
            {
                throw new ParserException(
                    $"Type {typeName} não foi encontrado nas libs carregadas no contexto da aplicação");
            }
            Log.Debug($"adicionando type {typeName} ao cache de types");
            _typeDictionary.Add(typeName, genericType);
            return genericType;
        }

        private bool IsMatch(IRule rule, long line, OperatorType operatorType, string value, ICondition<IRule> condition)
        {
            var isMatch = false;
            switch (operatorType)
            {
                case OperatorType.IsMatch:
                    isMatch = ProcessRegex(condition.Value, value, line, rule);
                    break;

                case OperatorType.Equal:
                    isMatch = string.Equals(value, condition.Value, StringComparison.CurrentCultureIgnoreCase);
                    break;

                case OperatorType.Contains:
                    isMatch = value.ContainsIgnoreCase(condition.Value);
                    break;

                case OperatorType.NotContains:
                    isMatch = !value.ContainsIgnoreCase(condition.Value);
                    break;

                case OperatorType.ContainsValue:
                    isMatch = !string.IsNullOrWhiteSpace(value);
                    break;

                case OperatorType.Greater:
                case OperatorType.GreaterOrEqual:
                case OperatorType.Lesser:
                case OperatorType.LesserOrEqual:
                    isMatch = ProcessComparissonCriterias(operatorType, condition, value);
                    break;
            }
            Log.Debug($"atende a regra: {isMatch}");
            return isMatch;
        }

        /// <summary>
        ///     Processes the bind rule.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <param name="content">The content.</param>
        /// <param name="createdObjectList">The createdObjectList.</param>
        /// <exception cref="System.Exception">
        ///     Não foi possível determinar o type da ação
        ///     or
        ///     Nao existe valor em cache com o nome {nome do cache}
        /// </exception>
        private void ProcessBindRule(IRule rule, string[] content, ICollection<ParseEntity> createdObjectList)
        {
            Log.Debug("Iniciando processando de regra bind");
            // Procura o type genérico utilizado como parâmetro.
            var genericType = GetTypeFromName(rule.GetType().GetProperty("Type").GetValue(rule, null).ToString());
            Type constructed = null;
            if (rule is IPositionalBindAction)
            {
                constructed = typeof(IPositionalBindAction);
            }
            else if (rule is IColumnBindAction)
            {
                constructed = typeof(IColumnBindAction);
            }
            if (constructed == null)
            {
                throw new Exception($"Não foi possível determinar o type da ação");
            }
            Log.Debug("obtendo lista de propriedades do objeto");
            var allProperties = constructed.GetPublicProperties();
            Log.Debug($"objeto possui {allProperties.Length} propriedades");

            Log.Debug("lendo propriedades");
            var propertyBind =
                allProperties.SingleOrDefault(
                    p => p.Name.Equals(PropertyUtil<ColumnBindAction>.GetPropertyName(q => q.PropertyToBind)));
            var propertyCustomMask =
                allProperties.SingleOrDefault(
                    p => p.Name.Equals(PropertyUtil<ColumnBindAction>.GetPropertyName(q => q.CustomMask)));
            var propertyMask =
                allProperties.SingleOrDefault(
                    p => p.Name.Equals(PropertyUtil<ColumnBindAction>.GetPropertyName(q => q.Mask)));
            var propertyValue =
                allProperties.SingleOrDefault(
                    p => p.Name.Equals(PropertyUtil<ColumnBindAction>.GetPropertyName(q => q.Value)));

            Log.Debug("lendo valores das propriedades");
            var valueBind = (string)propertyBind.GetValue(rule, null);
            Log.Debug($"bind: {valueBind}");
            var valueCustomMask = (CustomMask)Convert.ToInt32(propertyCustomMask.GetValue(rule, null));
            Log.Debug($"customMask: {valueCustomMask}");
            var valueMask = (string)propertyMask.GetValue(rule, null);
            Log.Debug($"mask: {valueMask}");
            var valueValue = propertyValue.GetValue(rule, null);
            Log.Debug($"valor: {valueValue}");

            var property = genericType.GetProperty(valueBind, new Type[] { });
            var propertyDatatype = property.GetGetMethod().ReturnType;

            object instance = null;
            instance = createdObjectList.FirstOrDefault(p => p != null && p.GetType().IsEquivalentTo(genericType));
            if (instance == null)
            {
                Log.Debug("ainda nao existe instancia do objeto, criando nova instancia");
                instance = Activator.CreateInstance(genericType);
            }
            else
            {
                Log.Debug("ja existe uma instancia do objeto criada, reaproveitando");
            }
            object objConverted = null;

            if (valueValue != null)
            {
                Log.Debug($"atribuindo valor fixo \"{valueValue}\", convertendo objeto para o formato correto");
                valueValue = FormatValue(propertyDatatype, (string)valueValue, valueMask, valueCustomMask);
                property.SetValue(instance, valueValue);
            }
            else
            {
                // Se valueValue for null, indica que não foi definido um valor padrão para o campo, então irá ler o valor do arquivo conforme configurado no layout.
                var propertyUseCache =
                    allProperties.SingleOrDefault(
                        p => p.Name.Equals(PropertyUtil<ColumnBindAction>.GetPropertyName(q => q.UseCache)));
                var valueUseCache = (bool)propertyUseCache.GetValue(rule, null);
                Log.Debug($"deve usar cache: {valueUseCache}");
                if (valueUseCache)
                {
                    Log.Debug($"configurado para usar valor salvo em cache, procurando valor.");
                    var propertyCacheName =
                        allProperties.SingleOrDefault(
                            p => p.Name.Equals(PropertyUtil<ColumnBindAction>.GetPropertyName(q => q.CacheName)));
                    var valueCacheName = (string)propertyCacheName.GetValue(rule, null);
                    if (!_cacheDictionary.ContainsKey(valueCacheName))
                    {
                        throw new Exception($"Nao existe valor em cache com o nome {valueCacheName}");
                    }
                    var value = _cacheDictionary[valueCacheName];
                    objConverted = FormatValue(propertyDatatype, value, valueMask, valueCustomMask);
                    property.SetValue(instance, objConverted);
                }
                else
                {
                    if (rule is IPositionalBindAction)
                    {
                        var propertySize =
                            allProperties.SingleOrDefault(
                                p => p.Name.Equals(PropertyUtil<PositionalBindAction>.GetPropertyName(q => q.Size)));
                        var propertyStart =
                            allProperties.SingleOrDefault(
                                p =>
                                    p.Name.Equals(
                                        PropertyUtil<PositionalBindAction>.GetPropertyName(q => q.StartPosition)));
                        var valueSize = (int)propertySize.GetValue(rule, null);
                        var valueStart = (int)propertyStart.GetValue(rule, null);
                        var value = content[0].Substring(valueStart - 1, valueSize);
                        objConverted = FormatValue(propertyDatatype, value, valueMask, valueCustomMask);
                        property.SetValue(instance, objConverted);
                    }
                    else if (rule is IColumnBindAction)
                    {
                        var propertyArrayIndex =
                            constructed.GetProperty(PropertyUtil<ColumnBindAction>.GetPropertyName(q => q.ColumnIndex));
                        var valueArrayIndex = (int)propertyArrayIndex.GetValue(rule, null);
                        var value = content[valueArrayIndex + 1];
                        objConverted = FormatValue(propertyDatatype, value, valueMask, valueCustomMask);
                        property.SetValue(instance, objConverted);
                    }
                }
            }
            Log.Debug($"atribuído valor {property.GetValue(instance, null)} a propriedade {property.Name}");
            if (!createdObjectList.Contains(instance)) createdObjectList.Add((ParseEntity)instance);
        }

        /// <summary>
        ///     Processes the cache rule.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <param name="content">The content.</param>
        /// <param name="inst">The createdObjectList.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">$Valor do cache não pode ser nulo</exception>
        private bool ProcessCacheRule(IRule rule, string[] content, ICollection<ParseEntity> inst)
        {
            Type constructed = null;
            if (rule is IPositionalRegisterCacheAction)
            {
                constructed = typeof(IPositionalRegisterCacheAction);
            }
            else if (rule is IColumnRegisterCache)
            {
                constructed = typeof(IColumnRegisterCache);
            }

            var allProperties = constructed.GetPublicProperties();
            var propertyCacheName = allProperties.SingleOrDefault(p => p.Name.Equals("CacheName"));
            var propertyCacheValue = allProperties.SingleOrDefault(p => p.Name.Equals("CacheValue"));
            var valueCacheName = (string)propertyCacheName.GetValue(rule, null);
            var valueCacheValue = (string)propertyCacheValue.GetValue(rule, null);

            string value = null;

            // Se cache value for null, indica que o valor deverá ser definido através de uma posição do arquivo.
            if (valueCacheValue == null)
            {
                if (rule is IPositionalRegisterCacheAction)
                {
                    var propertySize = allProperties.SingleOrDefault(p => p.Name.Equals("Size"));
                    var propertyStartPosition = allProperties.SingleOrDefault(p => p.Name.Equals("StartPosition"));
                    var valueSize = (int)propertySize.GetValue(rule, null);
                    var valueStartPosition = (int)propertyStartPosition.GetValue(rule, null);
                    value = content[0].Substring(valueStartPosition - 1, valueSize);
                }
                else if (rule is IColumnRegisterCache)
                {
                    var propertyColumnIndex = allProperties.SingleOrDefault(p => p.Name.Equals("ColumnIndex"));
                    var valueColumnIndex = (int)propertyColumnIndex.GetValue(rule, null);
                    value = content[valueColumnIndex + 1];
                }
            }
            else
            {
                value = valueCacheValue;
            }
            if (value == null)
            {
                throw new Exception("Valor do cache nao pode ser nulo");
            }
            if (_cacheDictionary.ContainsKey(valueCacheName))
            {
                _cacheDictionary[valueCacheName] = value;
            }
            else
            {
                _cacheDictionary.Add(valueCacheName, value);
            }
            return false;
        }

        //TODO transferir para classe utilitária
        /// <summary>
        ///     Processa uma regra condicional, invocando recursivamente o método <see cref="ProcessRule" /> para cada uma das
        ///     <see cref="ICondition{T}.Actions" /> relacionadas.
        /// </summary>
        /// <param name="rule">Regra a ser processada.</param>
        /// <param name="content">Conteúdo do registro de dados a ser processado.</param>
        /// <param name="createdObjectList">Objetos gerados durante o processamento da árvore de regras..</param>
        /// <param name="line"></param>
        /// <returns>
        ///     Indicador se deve abortar o processamento de regras. Se <c>true</c> indica que deve <b>parar</b> o
        ///     processamento, se <c>false</c> continua processando mais regras.
        /// </returns>
        private bool ProcessConditionalRule(IRule rule, string[] content, ICollection<ParseEntity> createdObjectList,
            long line)
        {
            Log.Debug("iniciando processamento da regra condicional");
            string value = null;
            var operatorType = OperatorType.Equal;
            if (rule is IPositionalConditionRule)
            {
                Log.Debug($"regra é posicional, convertendo objeto");
                var positionalConditionRule = (IPositionalConditionRule)rule;
                operatorType = positionalConditionRule.OperatorType;
                if (positionalConditionRule.OperatorType == OperatorType.IsMatch)
                {
                    Log.Debug("regra usa regex, verificando se foi atribuído posição inicial e final para o teste");
                    if (positionalConditionRule.StartPosition != 0 &&
                        positionalConditionRule.Size != 0)
                    {
                        Log.Debug(
                            $"extraindo string na posição {positionalConditionRule.StartPosition} por {positionalConditionRule.Size}");
                        value = content[0].Substring(positionalConditionRule.StartPosition - 1,
                            positionalConditionRule.Size);
                        Log.Debug($"string a trabalhar: \"{value}\"");
                    }
                    else
                    {
                        value = content[0];
                        Log.Debug($"regra não usa posições, string a trabalhar: \"{value}\"");
                    }
                }
                else
                {
                    value = content[0].Substring(positionalConditionRule.StartPosition - 1, positionalConditionRule.Size);
                    Log.Debug($"regra não usa regex, string a trabalhar: \"{value}\"");
                }
            }
            else if (rule is IColumnConditionRule)
            {
                Log.Debug($"regra é de colunas, obtendo valor da coluna");
                var columnConditionRule = (IColumnConditionRule)rule;
                value = content[columnConditionRule.ArrayPosition];
                operatorType = columnConditionRule.OperatorType;
            }
            var condition = (ICondition<IRule>)rule;
            // Verifica se atende as condições
            Log.Debug(
                $"verificando se string \"{(string.IsNullOrWhiteSpace(value) ? "NULL" : value)}\" atende o critério \"{condition.Value}\"");
            var isMatch = IsMatch(rule, line, operatorType, value, condition);
            if (!isMatch)
            {
                Log.Debug("valor não atende a regra, abortando processamento da regra");
                return false;
            }
            Log.Debug($"valor atende a condição, {condition.Actions.Count} ações a executar");
            foreach (var action in condition.Actions)
            {
                Log.Debug($"processando regra {action}");
                var abort = ProcessRule(action, content, createdObjectList, line);
                if (abort)
                {
                    Log.Debug("abortando execução das demais regras.");
                    return true;
                }
                Log.ChangeAppContext("");
            }
            return isMatch;
        }

        private bool ProcessRegex(string regexString, string value, long line, IRule rule)
        {
            Regex regex = null;
            try
            {
                // A Regex tem o máximo de 10 segundos para ser processada.
                regex = new Regex(regexString, RegexOptions.None, new TimeSpan(0, 0, 0, 10));
            }
            catch (Exception e)
            {
                throw new ParserException($"Falha ao criar regex {regexString}.", e, line, rule);
            }
            return regex.IsMatch(value);
        }

        /// <summary>
        ///     Processa a uma determinada regra para cada linha do arquivo.
        /// </summary>
        /// <param name="rule">Regra a ser processada.</param>
        /// <param name="content">Linha do arquivo a ser processada.</param>
        /// <param name="createdObjectList">Lista de objetos criados pelo processamento das demais regras.</param>
        /// <param name="line">Linha ou posição que esta sendo processada</param>
        /// <returns>
        ///     Indicador se deve abortar o processamento de regras. Se <c>true</c> indica que deve <b>parar</b> o
        ///     processamento, se <c>false</c> continua processando mais regras.
        /// </returns>
        private bool ProcessRule(IRule rule, string[] content, ICollection<ParseEntity> createdObjectList, long line)
        {
            Log.ChangeAppContext($"{line}:{rule.ExecutionStructure}");
            Log.Debug($"processando regra {rule}");
            switch (rule.RuleType)
            {
                case PositionalRuleType.Conditional:
                    return ProcessConditionalRule(rule, content, createdObjectList, line);

                case PositionalRuleType.BindValue:
                    ProcessBindRule(rule, content, createdObjectList);
                    return false;

                case PositionalRuleType.Cache:
                    return ProcessCacheRule(rule, content, createdObjectList);

                case PositionalRuleType.Ignore:
                    return true;
            }
            return false;
        }
    }
}