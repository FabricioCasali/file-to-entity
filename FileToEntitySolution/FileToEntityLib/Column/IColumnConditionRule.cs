namespace FileToEntityLib.Column
{
    public interface IColumnConditionRule : ICondition<IColumnConditionRule>
    {
        int ArrayPosition { get; set; }

        /// <summary>
        /// Índice da posição do array.
        /// </summary>
        /// <param name="position">Posição do array.</param>
        /// <returns></returns>
        IColumnConditionRule ArrayItem(int position);
    }
}