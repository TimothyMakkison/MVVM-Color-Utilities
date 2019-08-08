using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVVM_Color_Utilities.ViewModel.Helper_Classes;
using MaterialDesignThemes.Wpf;


namespace MVVM_Color_Utilities.ColorPicker_Tab
{
    class ColorPickerViewModel : ObservableObject, IPageViewModel
    {
        public PackIconKind Icon
        {
            get
            {
                return PackIconKind.Paint;
            }
        }
    }
}
