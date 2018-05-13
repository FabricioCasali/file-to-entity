using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FileToEntityLib.Extensios
{
    public static class TypeExtensions
    {
        public static PropertyInfo[] GetPublicProperties(this Type type)
        {
            if (type.IsInterface)
            {
                var propertyInfos = new List<PropertyInfo>();

                var considered = new List<Type>();
                var queue = new Queue<Type>();
                considered.Add(type);
                queue.Enqueue(type);
                while (queue.Count > 0)
                {
                    var subType = queue.Dequeue();
                    foreach (var subInterface in subType.GetInterfaces())
                    {
                        if (considered.Contains(subInterface)) continue;

                        considered.Add(subInterface);
                        queue.Enqueue(subInterface);
                    }

                    var typeProperties = subType.GetProperties(
                        BindingFlags.FlattenHierarchy
                        | BindingFlags.Public
                        | BindingFlags.Instance);

                    var newPropertyInfos = typeProperties
                        .Where(x => !propertyInfos.Contains(x));

                    propertyInfos.InsertRange(0, newPropertyInfos);
                }

                return propertyInfos.ToArray();
            }

            return type.GetProperties(BindingFlags.FlattenHierarchy
                                      | BindingFlags.Public | BindingFlags.Instance);
        }

        /// <summary>
        ///   Verifica se o objeto é uma coleção.
        /// </summary>
        /// <param name="o"> Objeto a verificar. </param>
        /// <returns> <i>true</i> se for uma coleção, <i>false</i> caso contrário. </returns>
        public static bool IsCollection(this object o)
        {
            return typeof(ICollection).IsAssignableFrom(o.GetType())
                   || typeof(ICollection<>).IsAssignableFrom(o.GetType());
        }

        /// <summary>
        ///   Verifica se o tipo é uma coleção.
        ///   <br /><b>OBS:</b> O método tem por objeto identificar se o objeto é um coleção (List, Collection, Enumerable, Set, etc.). Tipos de dados como String <b>não</b> retornarão positivo.
        /// </summary>
        /// <param name="propertyType"> Tipo do objeto a consultar. </param>
        /// <returns> <i>true</i> se for uma coleção, <i>false</i> caso contrário. </returns>
        public static bool IsCollection(this Type propertyType)
        {
            var collections = new List<Type> { typeof(IEnumerable<>), typeof(IEnumerable), typeof(HashSet<>) };
            if (propertyType == null || propertyType == typeof(string))
            {
                return false;
            }

            return propertyType.GetInterfaces().Any(type => collections.Any(c => type == c));
        }
    }
}