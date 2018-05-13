namespace FileToEntityLib
{
    /// <summary>
    ///     <see cref="IRule" /> que define uma <see cref="IAction" /> onde ir� ignorar o processamento das demais regras.
    /// </summary>
    public interface IIgnoreAction : IAction
    {
        /// <summary>
        ///     Obt�m ou atribui o <see cref="IgnoreScope" /> que ser� ignorado pela regra.
        /// </summary>
        IgnoreScope Scope { get; set; }
    }
}