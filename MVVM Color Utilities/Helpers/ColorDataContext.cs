using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;

namespace MVVM_Color_Utilities.Helpers
{
    //TODO create a dedicated database handler and add database
    internal class ColorDataContext
    {
        private static readonly string projectPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName; //Get Path of ColorItems file
        private static readonly string colorsFilePath = projectPath + "/Resources/ColorItemsList.txt";
        private ObservableCollection<ListColorClass> colorClassList;

        /// <summary>
        /// Returns an ObservableCollection containing ColorClass objects.
        /// </summary>
        public ObservableCollection<ListColorClass> ColorClassList => colorClassList ??=
                    JsonConvert.DeserializeObject<ObservableCollection<ListColorClass>>(File.ReadAllText(colorsFilePath));

        /// <summary>
        /// Returns the next viable ID.
        /// </summary>
        public int NextID => ColorClassList.Count + 1;

        /// <summary>
        /// Saves current <see cref="ColorClassList"/> to <see cref="colorsFilePath"/>.
        /// </summary>
        /// <returns></returns>
        public bool SaveColorsList()
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
        public bool AddColorItem(int index, string hexString, string nameString)
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
        public bool EditColorItem(int index, string hexString, string nameString)
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
        public bool DeleteColorItem(int index)
        {
            if (ColorClassList.Count > index && ColorClassList.Count > 0)
            {
                ColorClassList.RemoveAt(index);
                return SaveColorsList();
            }
            return false;
        }
    }
}