using System;

namespace FileToEntityLib.Annotation
{
    [AttributeUsage(AttributeTargets.Class)]
    public class BindableEntityAttribute : Attribute
    {
        public BindableEntityAttribute(string friendlyName)
        {
            FriendlyName = friendlyName;
        }

        public string FriendlyName { get; set; }
    }
}