using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MVVM_Color_Utilities.ViewModel.Helper_Classes
{
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var handler = PropertyChanged;
            if (PropertyChanged != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        protected T Singleteon<T>(ref T storage, Func<T> func)
        {
            if(storage == null)
            {
                storage = func.Invoke();
            }
            return storage;
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
