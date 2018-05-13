namespace FileToEntityLib
{
    public class IgnoreAction : Rule, IIgnoreAction
    {
        public IgnoreAction() : base(PositionalRuleType.Ignore)
        {
        }

        /// <summary>
        ///     Obtém ou atribui o <see cref="IgnoreScope" /> que será ignorado pela regra.
        /// </summary>
        public virtual IgnoreScope Scope { get; set; }

        public override object Clone()
        {
            return new IgnoreAction()
            {
                Scope = Scope,
                Order = Order,
                ExecutionStructure = ExecutionStructure,
                RuleType = RuleType
            };
        }
    }
}