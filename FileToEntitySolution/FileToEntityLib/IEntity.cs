using System;

namespace FileToEntityLib
{
    public interface IEntity
    {
        /// <summary>
        /// Obtém ou atribui o login do usuário que realizou a última alteração na entidade
        /// </summary>
        string ChangedBy { get; set; }

        /// <summary>
        /// Obtém ou atribui a data da última alteração na entidade
        /// </summary>
        DateTime? ChangedOn { get; set; }

        /// <summary>
        /// Obtém ou atribui o login do usuário que criou a entidade
        /// </summary>
        string CreatedBy { get; set; }

        /// <summary>
        /// Obtém ou atribui a data de criação da entidade
        /// </summary>
        DateTime CreatedOn { get; set; }

        /// <summary>
        /// Obtém ou atribui o identificador único da entidade. Este campo é gerado automaticamente pela aplicação quando o objeto é persistido, de acordo com os critérios da estratégia de persistência.
        /// </summary>
        long Id { get; set; }
    }
}