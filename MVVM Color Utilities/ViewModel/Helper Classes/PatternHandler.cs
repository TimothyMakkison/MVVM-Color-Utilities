using System;
using System.Windows.Input;

namespace MVVM_Color_Utilities.ViewModel.Helper_Classes
{
    public static class PatternHandler
    {
        /// <summary>
        /// Singleton function that constructs an instance of an object when first called and then
        /// returns the object in subsequent calls.
        /// </summary>
        /// <typeparam name="T">Type of object.</typeparam>
        /// <param name="storage">Referable object storage.</param>
        /// <param name="func">Object constructor.</param>
        /// <returns>Object.</returns>
        public static T Singleton<T>(ref T storage, Func<T> func) => storage ??= func.Invoke();

        /// <summary>
        /// Singleton function that constructs an instance of an object when if empty is true.
        /// </summary>
        /// <typeparam name="T">Type of object.</typeparam>
        /// <param name="storage">Referable object storage.</param>
        /// <param name="empty">Boolean representing the state of storage.</param>
        /// <param name="func">Object constructor.</param>
        /// <returns>Object.</returns>
        public static T Singleton<T>(ref T storage, bool empty, Func<T> func)
        {
            if (empty)
                storage = func.Invoke();

            return storage;
        }

        /// <param name="func">Object constructor.</param>
        /// <returns>Object.</returns>

        /// <summary>
        /// Singleton function that constructs an instance of an <see cref="ICommand"/> object when first called and then
        /// returns the object in subsequent calls.
        /// </summary>
        /// <param name="storage">Referable object storage.</param>
        /// <param name="command">Delegate command to be assigned to storage.</param>
        /// <returns>Returns command.</returns>
        public static ICommand Singleton(ref ICommand storage, Action command)
            => storage ??= new RelayCommand(param => command());
    }
}