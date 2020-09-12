using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MVVM_Color_Utilities.Helpers
{
    internal interface IDataContext<T>
    {
        IEnumerable<T> Source { get; }
        ObservableCollection<T> Observable { get; }


        IDataContext<T> Add(T item);

        IDataContext<T> RemoveAt(int index);

        IDataContext<T> InsertAt(int index, T item);

        IDataContext<T> ReplaceAt(int index, T item);

        bool Save();
    }
}