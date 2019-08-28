using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;
using MVVM_Color_Utilities.ViewModel.Helper_Classes;
using System.Windows.Media;
using Newtonsoft.Json;

namespace MVVM_Color_Utilities.ColorsList_Tab
{
    public class ColorListModel
    {
        #region Fields
        private readonly static string projectPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName; //Get Path of ColorItems file
        private readonly static string colorsFilePath = projectPath + "/Resources/ColorItemsList.txt";
        #endregion

        #region Properties
        public ObservableCollection<ColorClass> ColorClassList { get; }
        =JsonConvert.DeserializeObject<ObservableCollection<ColorClass>>(File.ReadAllText(colorsFilePath));

        public int NextID
        {
            get
            {
                return ColorClassList.Count > 0 ? ColorClassList[0].ID + 1 : 0;
            }
        }
        #endregion

        #region Methods
        private void SaveColorsList()
        {
            try
            {
                File.WriteAllText(colorsFilePath, JsonConvert.SerializeObject(ColorClassList));
            }
            catch { }
        }
        public void AddColorItem(int index,string hexString, string nameString)
        {
            if(ColorClassList.Count > index || ColorClassList.Count==0)
            {
                ColorClassList.Insert(0, new ColorClass(NextID, hexString, nameString));
                SaveColorsList();
            }
        }
        public void EditColorItem(int index,string hexString, string nameString)
        {
            if (ColorClassList.Count > index && ColorClassList.Count > 0)
            {
                ColorClassList[index] = new ColorClass(NextID, hexString, nameString);
                SaveColorsList();
            }
        }
        public void DeleteColorItem(int index)
        {
            if (ColorClassList.Count > index &&ColorClassList.Count>0)
            {
                ColorClassList.RemoveAt(index);
                SaveColorsList();
            }
        }
        #endregion
    }
    #region Color Class
    public class ColorClass
    {
        #region Constructor
        public ColorClass(int id, string hex, string name)
        {
            ID = id;
            Name = name;
            Hex = hex;
        }
        #endregion

        #region Properties
        public int ID { get; set; }
        public string Hex { get; set; }
        public string Name { get; set; }

        public SolidColorBrush SampleBrush
        {
            get
            {
                Color color;
                try
                {
                     color = (Color)ColorConverter.ConvertFromString(Hex);
                }
                catch
                {
                    color = (Color)ColorConverter.ConvertFromString("#FFFF");
                    Hex = "#FFFF";
                }
                return new SolidColorBrush(color);
            } 
        }
        #endregion
    }
    #endregion
}
