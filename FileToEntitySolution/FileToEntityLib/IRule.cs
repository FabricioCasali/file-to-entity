using System;

namespace FileToEntityLib
{
    public interface IRule  :ICloneable
    {
        PositionalRuleType RuleType { get; set; }

        string ExecutionStructure { get; set; }

        int Order { get; set; }

        IRule Parent { get; set; }
    }
}