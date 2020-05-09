using System.Text.RegularExpressions;
using System.Windows.Media;

namespace MVVM_Color_Utilities.Helpers
{
    public class ListColorClass
    {
        #region Fields
        private string hex="";
        private readonly Regex hexColorReg = new Regex("^#(?:(?:[0-9a-fA-F]{3}){1,2}|(?:[0-9a-fA-F]{4}){1,2})$");
        #endregion

        #region Constructor
        /// <summary>
        /// Constructs an instance of <see cref="ListColorClass"/> using its ID, color in hex format and given name.
        /// </summary>
        /// <param name="id">Id of color.</param>
        /// <param name="hex">Hexadecimal form of color.</param>
        /// <param name="name">Name of color.</param>
        public ListColorClass(int id, string hex, string name)
        {
            ID = id;
            Name = name;
            Hex = hex;
        }
        #endregion

        #region Properties
        /// <summary>
        /// ID value of ColorItem.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Color in hex format.
        /// </summary>
        public string Hex
        {
            get => hex;
            set
            {
                hex = hexColorReg.IsMatch(value) ? value : "#FFFF";
                SampleBrush = HexToBrush(hex);
            }
        }
        /// <summary>
        /// Name of color.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Brush form of color.
        /// </summary>
        public SolidColorBrush SampleBrush { get; set; }

        #endregion

        #region Methods
        /// <summary>
        /// Converts valid hex color into <see cref="SolidColorBrush"/> format.
        /// </summary>
        /// <param name="hexString">Input hexstring.</param>
        /// <returns>Color in <see cref="SolidColorBrush"/> format.</returns>
        private SolidColorBrush HexToBrush(string hexString)
        {
            Color color = hexColorReg.IsMatch(hexString)
                ? (Color)ColorConverter.ConvertFromString(Hex)
                : Color.FromRgb(255,255,255);
            return new SolidColorBrush(color);
        }
        #endregion
    }
}
