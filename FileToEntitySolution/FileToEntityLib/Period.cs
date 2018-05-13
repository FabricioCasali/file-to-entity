using System;

namespace FileToEntityLib
{
    public class Period : IComparable<Period>
    {
        public int Month { get; set; }

        public int Year { get; set; }

        public int CompareTo(Period other)
        {
            throw new NotImplementedException();
        }
    }
}