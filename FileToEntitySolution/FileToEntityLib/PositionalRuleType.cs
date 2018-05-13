using FileToEntityLib.Positional;

namespace FileToEntityLib
{
    /// <summary>
    /// Define os possíveis tipos de regras.
    /// </summary>
    public enum PositionalRuleType
    {
        None = 0,

        /// <summary>
        /// Regra condicional. <seealso cref="IPositionalConditionRule"/>
        /// </summary>
        Conditional = 1,

        /// <summary>
        /// Regra de atribuição de valores. <seealso cref="IPositionalBindAction"/>
        /// </summary>
        BindValue = 2,

        /// <summary>
        /// Regra de ignorar registro. <seealso cref="IIgnoreAction"/>
        /// </summary>
        Ignore = 3,

        /// <summary>
        /// Regra para registrar cache de uma variável. <seealso cref="IPositionalRegisterCacheAction"/>
        /// </summary>
        Cache = 4
    }
}