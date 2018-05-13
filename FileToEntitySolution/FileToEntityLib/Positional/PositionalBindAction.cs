using System;
using System.Linq.Expressions;
using FileToEntityLib.Extensios;

namespace FileToEntityLib.Positional
{
    public class PositionalBindAction : Rule, IPositionalBindAction
    {
        public PositionalBindAction() : base(PositionalRuleType.BindValue)
        {
        }

        public virtual string CacheName { get; set; }

        public virtual CustomMask CustomMask { get; set; }

        public virtual CustomType CustomType { get; set; }

        public virtual string Mask { get; set; }

        public virtual string PropertyToBind { get; set; }

        public virtual int Size { get; set; }
        public virtual int StartPosition { get; set; }
        public virtual string Type { get; set; }

        public virtual bool UseCache
        {
            get { return CacheName != null; }
        }

        public virtual bool UseCustomType
        {
            get { return CustomType != CustomType.None; }
        }

        public virtual string Value { get; set; }

        public virtual IPositionalBindAction Bind<T>(Expression<Func<T, object>> predicate)
        {
            Type = typeof(T).FullName;
            var expression = predicate as LambdaExpression;
            MemberExpression memberExpr = null;
            if (expression.Body.NodeType == ExpressionType.Convert)
            {
                memberExpr = ((UnaryExpression)expression.Body).Operand as MemberExpression;
            }
            else if (expression.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpr = expression.Body as MemberExpression;
            }
            PropertyToBind = memberExpr.Member.Name;
            return this;
        }

        public virtual IPositionalBindAction BindByCache<T>(Expression<Func<T, object>> predicate, string cacheName)
        {
            Type = typeof(T).FullName;
            var expression = predicate as LambdaExpression;
            MemberExpression memberExpr = null;
            if (expression.Body.NodeType == ExpressionType.Convert)
            {
                memberExpr = ((UnaryExpression)expression.Body).Operand as MemberExpression;
            }
            else if (expression.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpr = expression.Body as MemberExpression;
            }
            PropertyToBind = memberExpr.Member.Name;
            CacheName = cacheName;
            return this;
        }

        public override object Clone()
        {
            return new PositionalBindAction()
            {
                Value = Value,
                CacheName = CacheName,
                CustomMask = CustomMask,
                CustomType = CustomType,
                Mask = Mask,
                PropertyToBind = PropertyToBind,
                Size = Size,
                StartPosition = StartPosition,
                Type = Type,
                Order = Order,
                RuleType = RuleType,
                ExecutionStructure = ExecutionStructure
            };
        }

        public virtual IPositionalBindAction Set<T>(Expression<Func<T, object>> predicate, object value)
        {
            Type = typeof(T).FullName;
            var expression = predicate as LambdaExpression;
            MemberExpression memberExpr = null;
            if (expression.Body.NodeType == ExpressionType.Convert)
            {
                memberExpr = ((UnaryExpression)expression.Body).Operand as MemberExpression;
            }
            else if (expression.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpr = expression.Body as MemberExpression;
            }
            PropertyToBind = memberExpr.Member.Name;
            Value = value.ToString();
            return this;
        }

        public virtual IPositionalBindAction SetCustomMask(CustomMask mask)
        {
            CustomMask = mask;
            return this;
        }

        public virtual IPositionalBindAction SetCustomType(CustomType datatype)
        {
            CustomType = datatype;
            return this;
        }

        public virtual IPositionalBindAction SetMask(string mask)
        {
            Mask = mask;
            return this;
        }

        public virtual IPositionalBindAction SetPosition(int start, int by)
        {
            StartPosition = start;
            Size = by;
            return this;
        }

        public override string ToString()
        {
            var bindtype = UseCache ? $"usando cache {CacheName}" : Value != null ? $"o valor {Value}" : "";
            return $"{base.ToString()} no type {Type}.{PropertyToBind} {bindtype}, mascara {Mask}, customMask: {CustomMask.GetDescription()}, customType: {CustomType.GetDescription()}";
        }
    }
}