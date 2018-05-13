using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace FileToEntityLib.Extensios
{
    public static class EnumerationExtensions
    {
        /// <summary>
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="enumerationValue"> </param>
        /// <returns> </returns>
        public static string GetDescription<T>(this T enumerationValue)
            where T : struct
        {
            var type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");
            }

            //Tries to find a DescriptionAttribute for a potential friendly name
            //for the enum
            var memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                var attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    //Pull out the description value
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }

            //If we have no description attribute, just return the ToString of the enum
            return enumerationValue.ToString();
        }

        public static IList<string> GetDescriptions<T>(this T e) where T : struct
        {
            return GetDescriptions<T>();
        }

        public static IList<string> GetDescriptions<T>() where T : struct
        {
            var result = new List<String>();
            var names = Enum.GetNames(typeof(T));
            foreach (var name in names)
            {
                T xx;
                var works = Enum.TryParse(name, false, out xx);
                if (works)
                {
                    var description = xx.GetDescription();
                    result.Add(description);
                }
            }
            return result;
        }

        public static T ValueOf<T>(this string value) where T : struct
        {
            var t = typeof(T);
            if (!t.IsEnum)
            {
                throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");
            }

            var enu = Activator.CreateInstance(t);

            var fieldsInfo = t.GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (var info in fieldsInfo)
            {
                var customAttributes = info.GetCustomAttributes(typeof(DescriptionAttribute), true);
                foreach (DescriptionAttribute attribute in customAttributes)
                {
                    if (attribute.Description.Equals(value))
                    {
                        var o = Enum.ToObject(t, info.GetValue(info));
                        return (T)o;
                    }
                }
            }
            return (T)enu;
        }
    }
}