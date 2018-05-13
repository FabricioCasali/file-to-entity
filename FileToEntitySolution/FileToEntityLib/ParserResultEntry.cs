using System.Collections.Generic;
using System.Linq;

namespace FileToEntityLib
{
    public class ParserResultEntry : BaseEntity
    {
        public ParserResultEntry()
        {
            Data = new HashSet<ParseEntity>();
            Errors = new HashSet<ParserError>();
        }

        /// <summary>
        ///     Objetos gerados a partir da leitura da linha
        /// </summary>
        public virtual ISet<ParseEntity> Data { get; set; }


        public virtual ISet<ParserError> Errors { get; set; }


        /// <summary>
        ///     Obtém se houve erros durante a extração da linha.
        /// </summary>
        public virtual bool HasError
        {
            get { return Errors.Any(); }
        }


        public virtual ParseResult ParseResult { get; set; }


        /// <summary>
        ///     Obtém ou atribui linha do arquivo que representa o dado
        /// </summary>
        public virtual long Register { get; set; }

        public virtual void Add(ParseEntity parseEntity)
        {            
            Data.Add(parseEntity);
            if (parseEntity.RelatedResultEntry != this)
            {
                parseEntity.Set(this);
            }
        }
    }
}