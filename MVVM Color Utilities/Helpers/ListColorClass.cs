using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Windows;
using System;

namespace MVVM_Color_Utilities.Helpers
{
    public class ListColorClass
    {
        #region Fields
        private SolidColorBrush _sampleBrush;
        private string _hex;
        private readonly Regex _hexColorReg = new Regex("^#(?:(?:[0-9a-fA-F]{3}){1,2}|(?:[0-9a-fA-F]{4}){1,2})$");
        #endregion

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
        public string Hex
        {
            get
            {
                return _hex;
            }
            set
            {
                _hex = value;
                SampleBrush = null;
            }
        }
        public string Name { get; set; }

        public SolidColorBrush SampleBrush
        {
            get
            {
                if(_sampleBrush == null)
                {
                    _sampleBrush = HexToBrush();
                }
                return _sampleBrush;
            }
            set
            {
                _sampleBrush = value;
            }
        }
        private SolidColorBrush HexToBrush()
        {
            Color color;
            if (Hex == null)
            {
                Hex = "";
            }

            if (_hexColorReg.IsMatch(Hex))
            {
                color = (Color)ColorConverter.ConvertFromString(Hex);
                string location = (color.R>>2 << 12 | color.G>>2 << 6 | color.B >>2).ToString();

                string string10 = (color.R << 16 | color.G << 8 | color.B).ToString();
                int base10 = Int32.Parse(string10);
                int gridIndex = (base10 & 0xFF0000)>>18 << 12 | (base10 & 0xFF00) >> 10<< 6 
                    | (base10 & 0xFF) >> 2;
                //int back = (locationTest & 0x400000)
                int backToBase = (gridIndex & 0x3F000) << 6 | (gridIndex & 0xCF0) << 4 | (gridIndex & 0x3F) << 2;

                System.Diagnostics.Debug.WriteLine(((gridIndex & 0x3F000) >> 10).ToString()
                  + "," + color.R.ToString());
                System.Diagnostics.Debug.WriteLine(((gridIndex & 0xFC0) >> 4).ToString()
                   + "," + color.G.ToString());
                System.Diagnostics.Debug.WriteLine(((gridIndex & 0x3F) << 2).ToString() 
                  + "," + color.B.ToString());
               
                System.Diagnostics.Debug.WriteLine("base:"+backToBase);

                System.Diagnostics.Debug.WriteLine("original:"+string10+", location:"+location+", "+gridIndex);
             }
            else
            {
                color = (Color)ColorConverter.ConvertFromString("#FFFF");
                Hex = "#FFFF";
            }
            return new SolidColorBrush(color);
        }
        #endregion
    }
}
