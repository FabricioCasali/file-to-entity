using FileToEntityLib;

namespace FileToEntity.NHibernate
{
    public class ParserErrorMap : BaseEntityMap<ParserError>
    {
        public ParserErrorMap()
        {
            Table("PARSER_ERROR");

            Map(p => p.ErrorMessage)
                .Column("ERROR_MESSAGE")
                .Not.Nullable()
                .Length(2000);

            References(p => p.ParserResultEntry)
                .Column("PARSER_RESULT_ENTRY_ID")
                .ForeignKey("FK_PARSER_ERROR_ENTRY")
                .Not.Nullable();
        }
    }
}