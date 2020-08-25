using MVVM_Color_Utilities.Helpers;
using MVVM_Color_Utilities.ViewModel.Helper_Classes;
using System.Drawing;
using System.Windows.Input;

namespace MVVM_Color_Utilities.ImageAnalyzer_Tab
{
    internal class IAColorClass
    {
        #region Fields

        private ICommand saveColorCommand;

        #endregion Fields

        #region Constructor

        public IAColorClass(Color color)
        {
            Color = ColorUtils.ColorToBrush(color);
            Color.Freeze();
            ColorHex = ColorUtils.ColorToHex(color);
        }

        #endregion Constructor

        #region Properties

        public System.Windows.Media.SolidColorBrush Color { get; set; }
        public string ColorHex { get; set; }

        #endregion Properties

        #region Commands

        public ICommand SaveColorCommand => saveColorCommand ??= new RelayCommand(param => SaveColorMethod());

        #endregion Commands

        #region Methods

        /// <summary>
        /// Saves color to shared ColorList.
        /// </summary>
        /// <returns></returns>
        private bool SaveColorMethod()
        {
            try
            {
                int ID = SharedUtils.NextID;
                SharedUtils.ColorClassList.Insert(0, new Helpers.ListColorClass(ID, ColorHex, "Color " + ID.ToString()));
                SharedUtils.SaveColorsList();
                return true;
            }
            catch { return false; }
        }

        #endregion Methods
    }
}