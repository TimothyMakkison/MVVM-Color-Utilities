using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace MVVM_Color_Utilities.Helpers
{
    public static class Guard
    {
        #region Objects
        /// <summary>
        /// Checks if an object is null.
        /// </summary>
        /// <param name="argument">Object</param>
        /// <returns>Returns true if null.</returns>
        public static bool IsNull(this object argument) => argument == null;
        /// <summary>
        /// Checks if an object is null, if true then writes to debug.
        /// </summary>
        /// <param name="argument">Object</param>
        /// <param name="argumentName">Object name</param>
        /// <returns>Returns true if null.</returns>
        public static bool IsNull(this object argument, string argumentName)
        {
            if (argument==null)
            {
                String message = string.Format("Object {0} is  null", argumentName);
                Debug.WriteLine(message);
                return true;
            }
            return false;
        }
        #endregion

        #region Enumerable
        /// <summary>
        /// Checks if type is null or empty.
        /// </summary>
        /// <typeparam name="T">Extended type</typeparam>
        /// <param name="enumerable">Type name</param>
        /// <returns>Returns true if type if null or empty</returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                return true;
            }
            if (enumerable is ICollection<T> collection)
            {
                return collection.Count < 1;
            }
            return !enumerable.Any();
        }
        /// <summary>
        /// Checks if type is null or empty, if true then writes a message to debug.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable">Extended type</param>
        /// <param name="typeName">Type Name</param>
        /// <returns>Returns true if value is null or empty</returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable, string typeName)
        {
            if (enumerable.IsNullOrEmpty())
            {
                String message = string.Format("Type {0} is either empty or null", typeName);
                Debug.WriteLine(message);
                return true;
            }
            return false;
        }
        #endregion

        #region Int
        /// <summary>
        /// Checks if integer is equal to 0.
        /// </summary>
        /// <param name="integer">Integer</param>
        /// <returns></returns>
        public static bool IsEmpty(this int integer)
        {
            return integer ==0;
        }
        /// <summary>
        /// Checks if integer is equal to 0, if true then writes to debug.
        /// </summary>
        /// <param name="integer">Integer</param>
        /// <param name="integerName">IntegerName</param>
        /// <returns></returns>
        public static bool IsEmpty(this int integer,string integerName)
        {
            if(integer == 0)
            {
                Debug.WriteLine("Integer {0} is empty", integerName);
                return true;
            }
            return false;
        }
        #endregion
    }
}