using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FileToEntityLib.Extensios
{
    /// <summary>
    ///     A copy of the StringExtensions .NET class, originally written (I believe) by Andrew Peters.
    /// </summary>
    public static class StringExtensions
    {
        private static readonly List<Rule> _plurals = new List<Rule>();

        private static readonly List<Rule> _singulars = new List<Rule>();

        private static readonly List<string> _uncountables = new List<string>();

        #region Default Rules

        static StringExtensions()
        {
            AddPlural("$", "s");
            AddPlural("s$", "s");
            AddPlural("(ax|test)is$", "$1es");
            AddPlural("(octop|vir|alumn|fung)us$", "$1i");
            AddPlural("(alias|status)$", "$1es");
            AddPlural("(bu)s$", "$1ses");
            AddPlural("(buffal|tomat|volcan)o$", "$1oes");
            AddPlural("([ti])um$", "$1a");
            AddPlural("sis$", "ses");
            AddPlural("(?:([^f])fe|([lr])f)$", "$1$2ves");
            AddPlural("(hive)$", "$1s");
            AddPlural("([^aeiouy]|qu)y$", "$1ies");
            AddPlural("(x|ch|ss|sh)$", "$1es");
            AddPlural("(matr|vert|ind)ix|ex$", "$1ices");
            AddPlural("([m|l])ouse$", "$1ice");
            AddPlural("^(ox)$", "$1en");
            AddPlural("(quiz)$", "$1zes");

            AddSingular("s$", "");
            AddSingular("(n)ews$", "$1ews");
            AddSingular("([ti])a$", "$1um");
            AddSingular("((a)naly|(b)a|(d)iagno|(p)arenthe|(p)rogno|(s)ynop|(t)he)ses$", "$1$2sis");
            AddSingular("(^analy)ses$", "$1sis");
            AddSingular("([^f])ves$", "$1fe");
            AddSingular("(hive)s$", "$1");
            AddSingular("(tive)s$", "$1");
            AddSingular("([lr])ves$", "$1f");
            AddSingular("([^aeiouy]|qu)ies$", "$1y");
            AddSingular("(s)eries$", "$1eries");
            AddSingular("(m)ovies$", "$1ovie");
            AddSingular("(x|ch|ss|sh)es$", "$1");
            AddSingular("([m|l])ice$", "$1ouse");
            AddSingular("(bus)es$", "$1");
            AddSingular("(o)es$", "$1");
            AddSingular("(shoe)s$", "$1");
            AddSingular("(cris|ax|test)es$", "$1is");
            AddSingular("(octop|vir|alumn|fung)i$", "$1us");
            AddSingular("(alias|status)es$", "$1");
            AddSingular("^(ox)en", "$1");
            AddSingular("(vert|ind)ices$", "$1ex");
            AddSingular("(matr)ices$", "$1ix");
            AddSingular("(quiz)zes$", "$1");

            AddIrregular("person", "people");
            AddIrregular("man", "men");
            AddIrregular("child", "children");
            AddIrregular("sex", "sexes");
            AddIrregular("move", "moves");
            AddIrregular("goose", "geese");
            AddIrregular("alumna", "alumnae");

            AddUncountable("equipment");
            AddUncountable("information");
            AddUncountable("rice");
            AddUncountable("money");
            AddUncountable("species");
            AddUncountable("series");
            AddUncountable("fish");
            AddUncountable("sheep");
            AddUncountable("deer");
            AddUncountable("aircraft");
        }

        #endregion Default Rules

        public static string Abbreviate(this string word, int maxLength)
        {
            var x = word.Split("[\\s]", StringSplitOptions.RemoveEmptyEntries);
            while (NeedToAbbreviate(Concat(x), maxLength))
            {
                var len = Concat(x).Length;
                var dif = len - maxLength;
                if (x.Length > 3)
                {
                    for (var i = 1; i < x.Length - 1; i++)
                    {
                        if (x[i].Length > 3)
                        {
                            if (x[i].Length > dif)
                            {
                                dif = x[i].Length - dif;
                            }
                            else
                            {
                                dif = 1;
                            }
                            x[i] = x[i].Substring(0, dif);
                            break;
                        }
                    }
                }
                else if (x.Length == 2)
                {
                    x[1] = x[1].Substring(0, 1);
                }
                if (x.Length == 1 || Concat(x).Length == len)
                {
                    throw new Exception(string.Format("Não é possível abreviar ({0}).", word));
                }
            }
            return Concat(x);
        }

        public static string Camelize(this string lowercaseAndUnderscoredWord)
        {
            return Uncapitalize(Pascalize(lowercaseAndUnderscoredWord));
        }

        public static string Capitalize(this string word)
        {
            return word.Substring(0, 1).ToUpper() + word.Substring(1).ToLower();
        }

        public static bool ContainsIgnoreCase(this string source, string target)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return string.IsNullOrWhiteSpace(target);
            }
            if (string.IsNullOrWhiteSpace(target))
            {
                return false;
            }
            return source.ToUpper().Contains(target.ToUpper());
        }

        public static string Dasherize(this string underscoredWord)
        {
            return underscoredWord.Replace('_', '-');
        }

        public static bool EqualsIgnoreCase(this string a, string b)
        {
            return string.Equals(a, b, StringComparison.InvariantCultureIgnoreCase);
        }

        public static string Humanize(this string lowercaseAndUnderscoredWord)
        {
            return Capitalize(Regex.Replace(lowercaseAndUnderscoredWord, @"_", " "));
        }

        public static bool IsMatch(this string input, string pattern)
        {
            return Regex.IsMatch(input, pattern);
        }

        public static bool IsMatch(this string input, string pattern, RegexOptions options)
        {
            return Regex.IsMatch(input, pattern, options);
        }

        public static string Ordinalize(this string numberString)
        {
            return Ordanize(int.Parse(numberString), numberString);
        }

        public static string Ordinalize(this int number)
        {
            return Ordanize(number, number.ToString());
        }

        public static string Pascalize(this string lowercaseAndUnderscoredWord)
        {
            return Regex.Replace(lowercaseAndUnderscoredWord, "(?:^|_)(.)",
                delegate (Match match) { return match.Groups[1].Value.ToUpper(); });
        }

        public static string Pluralize(this string word)
        {
            return ApplyRules(_plurals, word);
        }

        public static string RandomString(int size)
        {
            var builder = new StringBuilder();
            var random = new Random();
            char ch;
            for (var i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            return builder.ToString();
        }

        public static string RandomString(this string x, int size)
        {
            var builder = new StringBuilder();
            var random = new Random();
            char ch;
            for (var i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }

        /// <summary>
        ///     Remove todas as ocorrencias do caractere de Carriage Return (\r).
        /// </summary>
        /// <param name="input"> . </param>
        /// <returns> . </returns>
        public static string RemoveCarriageReturn(this string input)
        {
            return input.Replace("\r", "");
        }

        /// <summary>
        ///     Substitui todas as ocorrências do comando Cr (carriage return, chr(13)) para Lf (line feed, chr(10)).
        /// </summary>
        /// <param name="input"> valor que será substituído. </param>
        /// <returns> valor sem ocorrências do comando carriage return (\\r) </returns>
        public static string ReplaceCrLfToLf(this string input)
        {
            return input.Replace("\r\n", "\n");
        }

        public static string Singularize(this string word)
        {
            return ApplyRules(_singulars, word);
        }

        public static string[] Split(this string value, string pattern, StringSplitOptions opt)
        {
            var result = Regex.Split(value, pattern);
            if (opt == StringSplitOptions.RemoveEmptyEntries)
            {
                string[] n = { };
                foreach (var s in result)
                {
                    if (!string.IsNullOrWhiteSpace(s))
                    {
                        var tmp = new string[n.Length + 1];
                        Array.Copy(n, tmp, n.Length);
                        tmp[tmp.Length - 1] = s;
                        n = tmp;
                    }
                }
                result = n;
            }
            return result;
        }

        public static string[] Split(this string value, string pattern)
        {
            return Split(value, pattern, StringSplitOptions.None);
        }

        public static string Titleize(this string word)
        {
            return Regex.Replace(Humanize(Underscore(word)), @"\b([a-z])",
                delegate (Match match) { return match.Captures[0].Value.ToUpper(); });
        }

        public static string[] Tokenize(this string val)
        {
            var match = Regex.Match(val, @"[^\s""']+|""([^""]*)""|'([^']*)'");
            var result = new List<string>();
            while (match.Success)
            {
                result.Add(match.Groups[0].Value.TrimSpecialChars());
                match = match.NextMatch();
            }
            return result.ToArray();
        }

        public static string TrimSpecialChars(this string value)
        {
            var specialChars = new[] { '\r', '\n', '\t' };
            return value.TrimStart(specialChars).TrimEnd(specialChars);
        }

        /// <summary>
        ///     Trunca o conteúdo de uma string para o tamanho desejado.
        /// </summary>
        /// <param name="value"> Valor a ser truncado. </param>
        /// <param name="length"> Tamanho máximo da string. </param>
        /// <returns> Valor truncado. </returns>
        public static string Truncate(this string value, int length)
        {
            if (value.Length > length)
            {
                return value.Substring(0, length);
            }
            return value;
        }

        public static string Uncapitalize(this string word)
        {
            return word.Substring(0, 1).ToLower() + word.Substring(1);
        }

        public static string Underscore(this string pascalCasedWord)
        {
            return Regex.Replace(
                Regex.Replace(
                    Regex.Replace(pascalCasedWord, @"([A-Z]+)([A-Z][a-z])", "$1_$2"), @"([a-z\d])([A-Z])",
                    "$1_$2"), @"[-\s]", "_").ToLower();
        }

        private static void AddIrregular(string singular, string plural)
        {
            AddPlural("(" + singular[0] + ")" + singular.Substring(1) + "$", "$1" + plural.Substring(1));
            AddSingular("(" + plural[0] + ")" + plural.Substring(1) + "$", "$1" + singular.Substring(1));
        }

        private static void AddPlural(string rule, string replacement)
        {
            _plurals.Add(new Rule(rule, replacement));
        }

        private static void AddSingular(string rule, string replacement)
        {
            _singulars.Add(new Rule(rule, replacement));
        }

        private static void AddUncountable(string word)
        {
            _uncountables.Add(word.ToLower());
        }

        private static string ApplyRules(List<Rule> rules, string word)
        {
            var result = word;

            if (!_uncountables.Contains(word.ToLower()))
            {
                for (var i = rules.Count - 1; i >= 0; i--)
                {
                    if ((result = rules[i].Apply(word)) != null)
                    {
                        break;
                    }
                }
            }

            return result;
        }

        private static string Concat(string[] tokens)
        {
            return tokens.Aggregate((s, s1) => s + " " + s1);
        }

        private static bool NeedToAbbreviate(string word, int maxLength)
        {
            var tokens = word.Split("[\\s]", StringSplitOptions.RemoveEmptyEntries);
            return (tokens.Aggregate((s, s1) => s + " " + s1).Length > maxLength);
        }

        private static string Ordanize(int number, string numberString)
        {
            var nMod100 = number % 100;

            if (nMod100 >= 11 && nMod100 <= 13)
            {
                return numberString + "th";
            }

            switch (number % 10)
            {
                case 1:
                    return numberString + "st";

                case 2:
                    return numberString + "nd";

                case 3:
                    return numberString + "rd";

                default:
                    return numberString + "th";
            }
        }

        #region Nested type: Rule

        private class Rule
        {
            private readonly Regex _regex;
            private readonly string _replacement;

            public Rule(string pattern, string replacement)
            {
                _regex = new Regex(pattern, RegexOptions.IgnoreCase);
                _replacement = replacement;
            }

            public string Apply(string word)
            {
                if (!_regex.IsMatch(word))
                {
                    return null;
                }

                return _regex.Replace(word, _replacement);
            }
        }

        #endregion Nested type: Rule
    }
}