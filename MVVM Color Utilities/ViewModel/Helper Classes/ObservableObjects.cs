using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

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

        #region Singleton
        protected T Singleton<T>(ref T storage, Func<T> func)
        {
            if (storage == null)
            {
                storage = func.Invoke();
            }
            return storage;
        }
        protected T Singleton<T>(ref T storage, bool empty, Func<T> func)
        {
            if (empty)
            {
                storage = func.Invoke();
            }
            return storage;
        }
        protected ICommand CommandSingleton(ref ICommand storage, Action command)
        {
            if (storage == null)
            {
                storage = new RelayCommand(param => command());
            }
            return storage;
        }
        #endregion
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return false;

            storage = value;
            OnPropertyChanged(propertyName);
            System.Diagnostics.Debug.WriteLine(storage);

            return true;
        }
    }
}
