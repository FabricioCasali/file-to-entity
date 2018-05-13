using System;
using System.Linq.Expressions;

namespace FileToEntityLib
{
    public interface IBindAction<X> : IAction where X : IAction
    {
        X Bind<T>(Expression<Func<T, object>> predicate);

        /// <summary>
        ///     Realiza o bind de uma propriedade com um valor registrado em um cache
        /// </summary>
        /// <typeparam name="T"><see cref="Type" /> do objeto que será vinculado</typeparam>
        /// <param name="predicate">Propriedade do objeto a ser vinculado</param>
        /// <param name="cacheName">Nome do cache a ser utilizado</param>
        /// <returns></returns>
        X BindByCache<T>(Expression<Func<T, object>> predicate, string cacheName);

        X Set<T>(Expression<Func<T, object>> predicate, object value);

        X SetCustomMask(CustomMask mask);

        X SetCustomType(CustomType datatype);

        X SetMask(string mask);
    }
}