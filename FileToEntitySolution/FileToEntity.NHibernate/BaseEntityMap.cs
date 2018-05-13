using FileToEntityLib;
using FluentNHibernate.Mapping;

namespace FileToEntity.NHibernate
{
    /// <summary>
    ///     Constrói a persistência automatizada do objeto <see cref="BaseEntity" />
    /// </summary>
    /// <typeparam name="TEntity"> </typeparam>
    public class BaseEntityMap<TEntity> : ClassMap<TEntity> where TEntity : BaseEntity
    {
        /// <summary>
        ///     Configura a persistência do objeto.
        /// </summary>
        public BaseEntityMap()
        {
            Id(c => c.Id)
                .Column("ID")
                .CustomType<long>()
                .GeneratedBy.Native()
                .UnsavedValue(-1);

            Map(p => p.CreatedBy)
                .Column("CREATED_BY")
                .Not.Nullable();

            Map(p => p.CreatedOn)
                .Column("CREATED_ON")
                .Not.Nullable();

            Map(p => p.ChangedBy)
                .Column("CHANGED_BY")
                .Nullable();

            Map(p => p.ChangedOn)
                .Column("CHANGED_ON")
                .Nullable();
        }
    }
}