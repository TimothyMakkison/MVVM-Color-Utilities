using System;
using System.Collections.ObjectModel;
using System.Windows;
using MVVM_Color_Utilities.Helpers;

namespace MVVM_Color_Utilities.ColorsList_Tab
{
    public class ColorListModel
    {
        #region Properties
        public ObservableCollection<ColorClass> ColorClassList
        {
            get
            {
                return SharedUtils.ColorClassList;
            }
        }
        #endregion

        #region Methods
        public void AddColorItem(int index,string hexString, string nameString)
        {
            if(SharedUtils.ColorClassList.Count > index || SharedUtils.ColorClassList.Count==0)
            {
                SharedUtils.ColorClassList.Insert(0, new ColorClass(SharedUtils.NextID, hexString, nameString));
                SharedUtils.SaveColorsList();
            }
        }
        public bool EditColorItem(int index,string hexString, string nameString)
        {
            if (SharedUtils.ColorClassList.Count > index && SharedUtils.ColorClassList.Count > 0)
            {
                SharedUtils.ColorClassList[index] = new ColorClass(SharedUtils.NextID, hexString, nameString);
                return SharedUtils.SaveColorsList() ? true : false;
            }
            else
                return false;
        }
        public bool DeleteColorItem(int index)
        {
            if (SharedUtils.ColorClassList.Count > index && SharedUtils.ColorClassList.Count>0)
            {
                SharedUtils.ColorClassList.RemoveAt(index);
                return SharedUtils.SaveColorsList() ? true:false;
            }
            return false;
        }
        #endregion
    }
}
