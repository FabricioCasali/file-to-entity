namespace FileToEntityLib.Column
{
    public interface IColumnBindAction : IBindAction<IColumnBindAction>
    {
        int ColumnIndex { get; set; }

        /// <summary>
        ///     Determina a coluna do arquivo que será verificada.
        /// </summary>
        /// <param name="index">Índice da coluna.</param>
        /// <returns></returns>
        IColumnBindAction SetColumn(int index);
    }
}