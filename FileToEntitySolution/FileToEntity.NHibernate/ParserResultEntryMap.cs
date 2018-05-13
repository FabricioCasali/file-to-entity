using FileToEntityLib;

namespace FileToEntity.NHibernate
{
    public class ParserResultEntryMap : BaseEntityMap<ParserResultEntry>
    {
        public ParserResultEntryMap()
        {
            Table("PARSER_RESULT_ENTRY");

            Map(p => p.Register)
                .Column("REGISTER")
                .Not.Nullable();

            References(p => p.ParseResult)
                .Column("PARSER_RESULT_ID")
                .Not.Nullable()
                .ForeignKey("FK_PARSER_ENTITY_RESULT_ID");

            HasMany(p => p.Errors)
                .Cascade.AllDeleteOrphan();

            HasMany(p => p.Data)
                .Cascade.AllDeleteOrphan();
        }
    }
}