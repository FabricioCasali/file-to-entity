using System;

namespace FileToEntityLib.Positional
{
    public class ColumnRegisterCache : Rule, IColumnRegisterCache
    {
        public ColumnRegisterCache() : base(PositionalRuleType.Cache)
        {
        }

        public string CacheName { get; set; }
        public string CacheValue { get; set; }

        public IColumnRegisterCache Register(string name)
        {
            CacheName = name;
            return this;
        }

        public int ColumnIndex { get; set; }

        public IColumnRegisterCache SetColumn(int index)
        {
            ColumnIndex = index;
            return this;
        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }
    }
}