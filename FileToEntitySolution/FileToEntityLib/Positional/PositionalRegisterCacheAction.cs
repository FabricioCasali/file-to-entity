using System;

namespace FileToEntityLib.Positional
{
    public class PositionalRegisterCacheAction : Rule, IPositionalRegisterCacheAction
    {
        public PositionalRegisterCacheAction() : base(PositionalRuleType.Cache)
        {
        }

        public virtual string CacheName { get; set; }

        public virtual string CacheValue { get; set; }

        public virtual int Size { get; set; }

        public virtual int StartPosition { get; set; }

        public override object Clone()
        {
            throw new NotImplementedException();
        }

        public virtual IPositionalRegisterCacheAction Register(string name)
        {
            CacheName = name;
            return this;
        }

        public virtual IPositionalRegisterCacheAction SetPosition(int start, int @by)
        {
            StartPosition = start;
            Size = by;
            return this;
        }

        public override string ToString()
        {
            return $"{base.ToString()} com nome {CacheName}, valor {CacheValue}, posição {StartPosition} por {Size}";
        }
    }
}