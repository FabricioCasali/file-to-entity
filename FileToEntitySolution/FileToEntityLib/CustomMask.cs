namespace FileToEntityLib
{
    /// <summary>
    ///     Define as máscaras customizadas tratadas pela aplicação durante a conversão de dados.
    /// </summary>
    public enum CustomMask
    {
        /// <summary>
        ///     Nenhuma máscara
        /// </summary>
        None = 0,

        /// <summary>
        ///     Mês/ano: JANEIRO/2015
        /// </summary>
        CompleteMonthYear = 1,

        /// <summary>
        ///     Formato monetário
        /// </summary>
        Monetary = 2,

        /// <summary>
        ///     Formato do mês escrito como texto
        /// </summary>
        MonthAsText = 3
    }
}