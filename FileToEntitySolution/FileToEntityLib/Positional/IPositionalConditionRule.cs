namespace FileToEntityLib.Positional
{
    public interface IPositionalConditionRule : ICondition<IPositionalConditionRule>
    {
        /// <summary>
        /// Obtém ou atribui a posição inicial a ser considerada para a leitura de dados.
        /// </summary>
        /// <value>
        /// Posição inicial (ínicio em 1)
        /// </value>
        int StartPosition { get; set; }

        int Size { get; set; }

        IPositionalConditionRule SetPosition(int start, int by);
    }
}