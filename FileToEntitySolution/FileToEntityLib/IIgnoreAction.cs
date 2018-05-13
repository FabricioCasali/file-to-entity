namespace FileToEntityLib
{
    /// <summary>
    ///     <see cref="IRule" /> que define uma <see cref="IAction" /> onde irá ignorar o processamento das demais regras.
    /// </summary>
    public interface IIgnoreAction : IAction
    {
        /// <summary>
        ///     Obtém ou atribui o <see cref="IgnoreScope" /> que será ignorado pela regra.
        /// </summary>
        IgnoreScope Scope { get; set; }
    }
}