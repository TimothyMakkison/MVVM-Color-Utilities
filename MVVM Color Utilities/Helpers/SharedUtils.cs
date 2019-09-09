using System.IO;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using Newtonsoft.Json;
namespace MVVM_Color_Utilities.Helpers
{
    static class SharedUtils
    {
        #region Fields
        private readonly static string projectPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName; //Get Path of ColorItems file
        private readonly static string colorsFilePath = projectPath + "/Resources/ColorItemsList.txt";
        private static ObservableCollection<ColorClass> _colorClassList;
        #endregion

        #region Properties
        public static ObservableCollection<ColorClass> ColorClassList
        {
            get
            {
                if (_colorClassList == null)
                {
                    _colorClassList=
                    JsonConvert.DeserializeObject<ObservableCollection<ColorClass>>(File.ReadAllText(colorsFilePath));
                }
                return _colorClassList;
            }
        }

        public static int NextID
        {
            get
            {
                return ColorClassList.Count > 0 ? ColorClassList[0].ID + 1 : 0;
            }
        }
        #endregion

        #region Methods
        public static bool SaveColorsList()
        {
            try
            {
                File.WriteAllText(colorsFilePath, JsonConvert.SerializeObject(ColorClassList));
                return true;
            }
            catch { return false; }
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
                catch//Invalid hex defaults to white.
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
