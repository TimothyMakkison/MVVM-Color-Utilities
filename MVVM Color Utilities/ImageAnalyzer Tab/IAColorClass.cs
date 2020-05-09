using MVVM_Color_Utilities.ViewModel.Helper_Classes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows;
using MVVM_Color_Utilities.Helpers;
using System.Windows.Input;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Reflection;

namespace MVVM_Color_Utilities.ImageAnalyzer_Tab
{
    class IAColorClass
    {
        #region Fields
        private ICommand _saveColorCommand;
        #endregion

        #region Constructor
        public IAColorClass(Color color)
        {
            Color = ColorUtils.ColorToBrush(color);
            Color.Freeze();
            ColorHex = ColorUtils.ColorToHex(color);
        }
        #endregion

        #region Properties
        public System.Windows.Media.SolidColorBrush Color { get; set; }
        public string ColorHex { get; set; }
        #endregion

        #region Commands
        public ICommand SaveColorCommand
        {
            get
            {
                if (_saveColorCommand == null)
                {
                    _saveColorCommand = new RelayCommand(param => SaveColorMethod());
                }
                return _saveColorCommand;
            }
        }
        #endregion

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
        #endregion
    }
}
