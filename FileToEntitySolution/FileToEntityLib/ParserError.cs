namespace FileToEntityLib
{
    public class ParserError : BaseEntity
    {
        public virtual string ErrorMessage { get; set; }

        public virtual ParserResultEntry ParserResultEntry { get; set; }
    }
}