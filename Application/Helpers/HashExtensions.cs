using System.Collections.Generic;

namespace Application.Helpers
{
    public static class HashExtensions
    {
        public static int GetSequenceHash<T>(this IEnumerable<T> source)
        {
            unchecked
            {
                int hash = 19;
                foreach (var item in source)
                {
                    hash += item.GetHashCode();
                }
                return hash;
            }
        }
    }
}