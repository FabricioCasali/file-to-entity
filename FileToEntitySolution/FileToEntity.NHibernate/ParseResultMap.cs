using FileToEntityLib;

namespace FileToEntity.NHibernate
{
    public class ParseResultMap : BaseEntityMap<ParseResult>
    {
        public ParseResultMap()
        {
            Table("PARSER_RESULT");

            Map(p => p.RegistersInFile)
                .Column("REGISTERS_IN_FILE")
                .Not.Nullable();

            Map(p => p.Size)
                .Column("SIZE")
                .Not.Nullable();

            Map(p => p.FileName)
                .Column("FILE_NAME")
                .Not.Nullable();

            HasMany(p => p.Entries)
                .Cascade.AllDeleteOrphan()
                .Inverse();
        }
    }
}