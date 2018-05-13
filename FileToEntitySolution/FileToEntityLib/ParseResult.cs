using System.Collections.Generic;
using System.Linq;

namespace FileToEntityLib
{
    public class ParseResult : BaseEntity
    {
        public ParseResult()
        {
            Entries = new HashSet<ParserResultEntry>();
        }

        public virtual ISet<ParserResultEntry> Entries { get; set; }

        public virtual string FileName { get; set; }

        public virtual bool HasErrors
        {
            get { return Entries.Any(p => p.HasError); }
        }

        public virtual long RegistersInFile { get; set; }

        public virtual long Size { get; set; }
    }
}