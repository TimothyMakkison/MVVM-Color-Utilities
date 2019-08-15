using MaterialDesignThemes.Wpf;
using MVVM_Color_Utilities.ViewModel.Helper_Classes;

namespace MVVM_Color_Utilities.ColorPicker_Tab
{

    class ColorPickerViewModel : ObservableObject, IPageViewModel
    {

        #region Properties
        public PackIconKind Icon
        {
            get
            {
                return PackIconKind.Paint;
            }
        }
        #endregion
    }
}
