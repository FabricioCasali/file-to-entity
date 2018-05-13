using System.Collections.Generic;

namespace FileToEntityLib

{
    public interface ICondition<out T> : IRule where T : IRule
    {
        /// <summary>
        ///     Lista de ações que serão executadas caso a regra seja cumprida
        /// </summary>
        /// <value>
        ///     Lista de ações
        /// </value>
        ICollection<IRule> Actions { get; set; }

        CastType CastType { get; set; }

        OperatorType OperatorType { get; set; }

        /// <summary>
        ///     Obtém ou atribui o valor da comparação.
        /// </summary>
        /// <value>
        ///     Valor.
        /// </value>
        string Value { get; set; }

        T CastToType(CastType castType);

        T Contains(string value);

        T ContainsValue();

        T IsEquals(string value);

        T IsGreater(string value);

        T IsGreaterOrEquals(string value);

        T IsLesser(string value);

        T IsLesserOrEquals(string value);

        /// <summary>
        ///     Verifica se o valor lido atende a expressão regular.
        /// </summary>
        /// <param name="value">Expressão regular</param>
        /// <returns></returns>
        T IsMatch(string value);

        T IsNotEquals(string value);

        T NotContains(string value);

        /// <summary>
        ///     Registra uma ação para ser executada quando a condição for verdadeira
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <returns></returns>
        T RegisterAction(IRule rule);
    }
}