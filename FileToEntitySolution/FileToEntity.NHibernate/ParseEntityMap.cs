using FileToEntityLib;

namespace FileToEntity.NHibernate
{
    public class ParseEntityMap : BaseEntityMap<ParseEntity>
    {
        public ParseEntityMap()
        {
            Table("PARSER_RESULT_ENTITY");

            References(p => p.RelatedResultEntry)
                .Column("PARSER_ENTRY_ID")
                .ForeignKey("PARSER_RESULT_ENTITY_ENTRY_ID");
        }
    }
}