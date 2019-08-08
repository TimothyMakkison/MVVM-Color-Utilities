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
    class ColorListModel
    {
        #region Fields
        private ObservableCollection<ColorClass> _colorsClassList;

        private readonly static string projectPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName; //Get Path of ColorItems file
        private readonly static string colorsFilePath = projectPath + "/Resources/ColorItemsList.txt";
        #endregion

        #region Properties
        public ObservableCollection<ColorClass> ColorClassList
        {
            get
            {
                if (_colorsClassList == null)
                {
                    _colorsClassList = JsonConvert.DeserializeObject<ObservableCollection<ColorClass>>(File.ReadAllText(colorsFilePath));
                }
                //MessageBox.Show("Getting");
                return _colorsClassList;
            }
        }

        public int NextID
        {
            get
            {
                return ColorClassList.Count > 0 ? ColorClassList[0].ID + 1 : 0;
            }
        }
        #endregion

        #region Methods

        public void SaveColorsList()
        {
            try
            {
                //MessageBox.Show("Saving");
                File.WriteAllText(colorsFilePath, JsonConvert.SerializeObject(ColorClassList));
            }
            catch { }
        }
        #endregion
    }

    public class ColorClass
    {
        public int ID { get; set; }
        public string Hex { get; set; }
        public string Name { get; set; }

        private Color _color = (Color)ColorConverter.ConvertFromString("#FFFF");

        public SolidColorBrush SampleBrush
        {
            get { return new SolidColorBrush(Color); } //Will accept 3,4,6,8
        }
        private Color Color
        {
            get
            {
                try
                {
                    return _color = (Color)ColorConverter.ConvertFromString(Hex);
                }
                catch
                {
                    Hex = "#FFFF";
                    return _color;
                }
            }
        }
        public ColorClass(int id, string hex, string name)
        {
            ID = id;
            Name = name;
            Hex = hex;
        }
        public override string ToString() // overrides any ToString methods and returns string
        {
            return ColorLineFormat();
        }
        public string ColorLineFormat()
        {
            return ID + "," + Hex + "," + Name;
        }
    }
}
