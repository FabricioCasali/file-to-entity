namespace FileToEntityLib
{
    public abstract class ParseEntity : BaseEntity
    {
        public virtual ParserResultEntry RelatedResultEntry { get; set; }

        public virtual void Set(ParserResultEntry parserResultEntry)
        {
            RelatedResultEntry = parserResultEntry;
            if (!parserResultEntry.Data.Contains(this))
            {
                parserResultEntry.Add(this);
            }
        }
    }
}