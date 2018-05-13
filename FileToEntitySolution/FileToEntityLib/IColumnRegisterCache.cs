namespace FileToEntityLib
{
    public interface IColumnRegisterCache : IRegisterCache<IColumnRegisterCache>
    {
        /// <summary>
        /// Obtém ou atribui o índice a ser utilizado
        /// </summary>
        /// <value>
        /// Índice da coluna
        /// </value>
        int ColumnIndex { get; set; }

        /// <summary>
        /// Determina a coluna do arquivo que será utilizada.
        /// </summary>
        /// <param name="index">Índice da coluna.</param>
        /// <returns></returns>
        IColumnRegisterCache SetColumn(int index);
    }
}