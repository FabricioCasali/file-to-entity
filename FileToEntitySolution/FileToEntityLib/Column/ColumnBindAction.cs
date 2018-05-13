using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

namespace FileToEntityLib.Column
{
    public class ColumnBindAction : Rule, IColumnBindAction
    {
        public ColumnBindAction() : base(PositionalRuleType.BindValue)
        {
        }

        [MaxLength(50)]
        public string CacheName { get; set; }

        [Required]
        public int ColumnIndex { get; set; }

        public CustomMask CustomMask { get; set; }

        public CustomType CustomType { get; set; }

        [MaxLength(80)]
        public string Mask { get; set; }

        [MaxLength(80)]
        [Required]
        public string PropertyToBind { get; set; }

        [MaxLength(200)]
        [Required]
        public string Type { get; set; }

        [NotMapped]
        public bool UseCache
        {
            get { return CacheName == null; }
        }

        [NotMapped]
        public bool UseCustomType
        {
            get { return CustomType != CustomType.None; }
        }

        [Required]
        [MaxLength(2000)]
        public object Value { get; set; }

        public IColumnBindAction Bind<T>(Expression<Func<T, object>> predicate)
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

        public IColumnBindAction BindByCache<T>(Expression<Func<T, object>> predicate, string cacheName)
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
            throw new NotImplementedException();
        }

        public IColumnBindAction Set<T>(Expression<Func<T, object>> predicate, object value)
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
            Value = value;
            return this;
        }

        /// <summary>
        ///     Determina a coluna do arquivo que será verificada.
        /// </summary>
        /// <param name="index">Índice da coluna.</param>
        /// <returns></returns>
        public IColumnBindAction SetColumn(int index)
        {
            ColumnIndex = index;
            return this;
        }

        public IColumnBindAction SetCustomMask(CustomMask mask)
        {
            CustomMask = mask;
            return this;
        }

        public IColumnBindAction SetCustomType(CustomType datatype)
        {
            CustomType = datatype;
            return this;
        }

        public IColumnBindAction SetMask(string mask)
        {
            Mask = mask;
            return this;
        }
    }
}