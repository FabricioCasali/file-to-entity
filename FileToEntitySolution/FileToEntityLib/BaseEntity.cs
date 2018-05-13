using System;

namespace FileToEntityLib
{
    public class BaseEntity : IEntity
    {
        public BaseEntity()
        {
            Id = -1;
        }

        public virtual string ChangedBy { get; set; }
        public virtual DateTime? ChangedOn { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual DateTime CreatedOn { get; set; }
        public virtual long Id { get; set; }
        public virtual byte[] RowVersion { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is BaseEntity)) return false;
            var casted = (BaseEntity)obj;
            return casted.Id == Id;
        }

        public override int GetHashCode()
        {
            if (Id == -1)
            {
                return base.GetHashCode();
            }
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return $"{GetType().Name} ({Id}) ";
        }
    }
}