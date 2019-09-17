using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Windows;

namespace MVVM_Color_Utilities.Helpers
{
    public class ListColorClass
    {
        #region Constructor
        public ListColorClass(int id, string hex, string name)
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
        private readonly Regex _hexColorReg = new Regex("^#(?:(?:[0-9a-fA-F]{3}){1,2}|(?:[0-9a-fA-F]{4}){1,2})$");

        public SolidColorBrush SampleBrush
        {
            get
            {
                Color color;
                if (Hex == null)
                {
                    Hex = "";
                }

                if (_hexColorReg.IsMatch(Hex))
                {
                    color = (Color)ColorConverter.ConvertFromString(Hex);
                }
                else
                {
                    color = (Color)ColorConverter.ConvertFromString("#FFFF");
                    Hex = "#FFFF";
                }
                return new SolidColorBrush(color);
            }
        }
        #endregion
    }
}
