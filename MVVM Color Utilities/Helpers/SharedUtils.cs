using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;

namespace MVVM_Color_Utilities.Helpers
{
    //TODO create a dedicated database handler and add database
    internal static class SharedUtils
    {
        #region Fields

        private readonly static string projectPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName; //Get Path of ColorItems file
        private readonly static string colorsFilePath = projectPath + "/Resources/ColorItemsList.txt";
        private static ObservableCollection<ListColorClass> _colorClassList;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Returns an ObservableCollection containing ColorClass objects.
        /// </summary>
        public static ObservableCollection<ListColorClass> ColorClassList
        {
            get
            {
                return _colorClassList ??= 
                    JsonConvert.DeserializeObject<ObservableCollection<ListColorClass>>(File.ReadAllText(colorsFilePath));
            }
        }

        /// <summary>
        /// Returns the next viable ID.
        /// </summary>
        public static int NextID => ColorClassList.Count > 0 ? ColorClassList[0].ID + 1 : 0;

        #endregion Properties

        #region Methods

        /// <summary>
        /// Saves current <see cref="ColorClassList"/> to <see cref="colorsFilePath"/>.
        /// </summary>
        /// <returns></returns>
        public static bool SaveColorsList()
        {
            try
            {
                File.WriteAllText(colorsFilePath, JsonConvert.SerializeObject(ColorClassList));
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// Adds new item to ColorClassList.
        /// </summary>
        /// <param name="index">Insert location</param>
        /// <param name="hexString">Hex of object</param>
        /// <param name="nameString">Name of object</param>
        /// <returns>Returns success of operation</returns>
        public static bool AddColorItem(int index, string hexString, string nameString)
        {
            index = MathUtils.Clamp(0, ColorClassList.Count, index);
            ColorClassList.Insert(index, new ListColorClass(NextID, hexString, nameString));
            return SaveColorsList();
        }

        /// <summary>
        /// Edits item from ColorClassList at given position.
        /// </summary>
        /// <param name="index">Index of item</param>
        /// <param name="hexString">New hex</param>
        /// <param name="nameString">New name</param>
        /// <returns>Returns success of operation</returns>
        public static bool EditColorItem(int index, string hexString, string nameString)
        {
            if (ColorClassList.Count > index && ColorClassList.Count > 0)
            {
                ColorClassList[index] = new ListColorClass(NextID, hexString, nameString);
                return SaveColorsList();
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Deletes item from ColorClassList at given position.
        /// </summary>
        /// <param name="index">Position of item</param>
        /// <returns>Returns success of operation</returns>
        public static bool DeleteColorItem(int index)
        {
            if (ColorClassList.Count > index && ColorClassList.Count > 0)
            {
                ColorClassList.RemoveAt(index);
                return SaveColorsList();
            }
            return false;
        }

        #endregion Methods
    }
}