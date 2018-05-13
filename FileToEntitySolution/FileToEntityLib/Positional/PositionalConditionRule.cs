using System.Collections.Generic;
using System.Linq;
using FileToEntityLib.Extensios;

namespace FileToEntityLib.Positional
{
    public class PositionalConditionRule : Rule, IPositionalConditionRule
    {
        public PositionalConditionRule() : base(PositionalRuleType.Conditional)
        {
            Actions = new List<IRule>();
        }

        public virtual ICollection<IRule> Actions { get; set; }

        public virtual CastType CastType { get; set; }

        public virtual OperatorType OperatorType { get; set; }

        public virtual int Size { get; set; }

        public virtual int StartPosition { get; set; }

        public virtual string Value { get; set; }

        public virtual IPositionalConditionRule By(int size)
        {
            Size = size;
            return this;
        }

        public virtual IPositionalConditionRule CastToType(CastType castType)
        {
            CastType = castType;
            return this;
        }

        public override object Clone()
        {
            return null;
        }

        public virtual IPositionalConditionRule Contains(string value)
        {
            OperatorType = OperatorType.Contains;
            Value = value;
            return this;
        }

        public virtual IPositionalConditionRule ContainsValue()
        {
            OperatorType = OperatorType.ContainsValue;
            return this;
        }

        public virtual IPositionalConditionRule IsEquals(string value)
        {
            OperatorType = OperatorType.Equal;
            Value = value;
            return this;
        }

        public virtual IPositionalConditionRule IsGreater(string value)
        {
            OperatorType = OperatorType.Greater;
            Value = value;
            return this;
        }

        public virtual IPositionalConditionRule IsGreaterOrEquals(string value)
        {
            OperatorType = OperatorType.GreaterOrEqual;
            Value = value;
            return this;
        }

        public virtual IPositionalConditionRule IsLesser(string value)
        {
            OperatorType = OperatorType.Lesser;
            Value = value;
            return this;
        }

        public virtual IPositionalConditionRule IsLesserOrEquals(string value)
        {
            OperatorType = OperatorType.LesserOrEqual;
            Value = value;
            return this;
        }

        public virtual IPositionalConditionRule IsMatch(string value)
        {
            OperatorType = OperatorType.IsMatch;
            Value = value;
            return this;
        }

        public virtual IPositionalConditionRule IsNotEquals(string value)
        {
            OperatorType = OperatorType.NotEquals;
            Value = value;
            return this;
        }

        public virtual IPositionalConditionRule NotContains(string value)
        {
            OperatorType = OperatorType.NotContains;
            Value = value;
            return this;
        }

        public virtual IPositionalConditionRule RegisterAction(IRule rule)
        {
            var max = Actions.Count == 0 ? 1 : Actions.Max(p => p.Order) + 1;
            rule.Order = max;
            Actions.Add(rule);
            rule.Parent = this;
            return this;
        }

        public virtual IPositionalConditionRule SetPosition(int start, int by)
        {
            StartPosition = start;
            Size = by;
            return this;
        }

        public override string ToString()
        {
            return $"{base.ToString()} com operador {OperatorType.GetDescription()} \"{Value}\", iniciando em {StartPosition} por {Size}";
        }
    }
}