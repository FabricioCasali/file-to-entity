using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace FileToEntityLib.Column
{
    public class ColumnConditionRule : Rule, IColumnConditionRule
    {
        public ColumnConditionRule() : base(PositionalRuleType.Conditional)
        {
            Actions = new List<IRule>();
        }

        public ICollection<IRule> Actions { get; set; }

        [Required]
        public int ArrayPosition { get; set; }

        [Required]
        public CastType CastType { get; set; }

        [Required]
        public OperatorType OperatorType { get; set; }

        [MaxLength(2000)]
        [Required]
        public string Value { get; set; }

        public IColumnConditionRule ArrayItem(int position)
        {
            ArrayPosition = position;
            return this;
        }

        public IColumnConditionRule CastToType(CastType castType)
        {
            CastType = castType;
            return this;
        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }

        public IColumnConditionRule Contains(string value)
        {
            OperatorType = OperatorType.Contains;
            Value = value;
            return this;
        }

        public IColumnConditionRule ContainsValue()
        {
            OperatorType = OperatorType.ContainsValue;
            return this;
        }

        public IColumnConditionRule IsEquals(string value)
        {
            OperatorType = OperatorType.Equal;
            Value = value;
            return this;
        }

        public IColumnConditionRule IsGreater(string value)
        {
            OperatorType = OperatorType.Greater;
            Value = value;
            return this;
        }

        public IColumnConditionRule IsGreaterOrEquals(string value)
        {
            OperatorType = OperatorType.GreaterOrEqual;
            Value = value;
            return this;
        }

        public IColumnConditionRule IsLesser(string value)
        {
            OperatorType = OperatorType.Lesser;
            Value = value;
            return this;
        }

        public IColumnConditionRule IsLesserOrEquals(string value)
        {
            OperatorType = OperatorType.LesserOrEqual;
            Value = value;
            return this;
        }

        public IColumnConditionRule IsMatch(string value)
        {
            OperatorType = OperatorType.IsMatch;
            Value = value;
            return this;
        }

        public IColumnConditionRule IsNotEquals(string value)
        {
            OperatorType = OperatorType.NotEquals;
            Value = value;
            return this;
        }

        public IColumnConditionRule NotContains(string value)
        {
            OperatorType = OperatorType.NotContains;
            Value = value;
            return this;
        }

        public IColumnConditionRule RegisterAction(IRule rule)
        {
            var max = Actions.Count == 0 ? 1 : Actions.Max(p => p.Order) + 1;
            rule.Order = max;
            Actions.Add(rule);
            rule.Parent = this;
            return this;
        }
    }
}