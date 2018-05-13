using System;
using FileToEntityLib.Extensios;

namespace FileToEntityLib
{
    public abstract class Rule : BaseEntity, IRule, IComparable<Rule>
    {
        protected Rule(PositionalRuleType ruleType)
        {
            RuleType = ruleType;
        }

        protected Rule()
        {
        }

        public virtual string ExecutionStructure { get; set; }

        public virtual int Order { get; set; }

        public virtual IRule Parent { get; set; }

        public virtual PositionalRuleType RuleType { get; set; }

        public abstract object Clone();

        /// <summary>
        ///     Compares the current instance with another object of the same type and returns an integer that indicates whether
        ///     the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <returns>
        ///     A value that indicates the relative order of the objects being compared. The return value has these meanings:
        ///     Value Meaning Less than zero This instance precedes <paramref name="other" /> in the sort order.  Zero This
        ///     instance occurs in the same position in the sort order as <paramref name="other" />. Greater than zero This
        ///     instance follows <paramref name="other" /> in the sort order.
        /// </returns>
        /// <param name="other">An object to compare with this instance. </param>
        public virtual int CompareTo(Rule other)
        {
            if (other == null) return 1;
            else if (other.Order.Equals(Order)) return 0;
            else if (other.Order > Order) return -1;
            else return +1;
        }

        public virtual bool ShouldSerializeParent()
        {
            return false;
        }

        public override string ToString()
        {
            var struc = string.IsNullOrWhiteSpace(ExecutionStructure) ? "" : ExecutionStructure + " > ";
            return $"{struc}{RuleType.GetDescription()} ({Id})";
        }
    }
}