using MVVM_Color_Utilities.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace MVVM_Color_Utilities.Helpers
{
    internal class ColorDataContext : IDataContext<ColorModel>
    {
        private static readonly string projectPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName; //Get Path of ColorItems file
        private static readonly string colorsFilePath = projectPath + "/Resources/ColorItemsList.txt";
        private readonly ObservableCollection<ColorModel> source;

        public ColorDataContext()
        {
            this.source = JsonConvert.DeserializeObject<ObservableCollection<ColorModel>>(File.ReadAllText(colorsFilePath));
            this.source ??= new ObservableCollection<ColorModel>();
        }

        /// <summary>
        /// Returns an ObservableCollection containing ColorClass objects.
        /// </summary>
        IEnumerable<ColorModel> IDataContext<ColorModel>.Source => source;

        ObservableCollection<ColorModel> IDataContext<ColorModel>.Observable => source;

        public IDataContext<ColorModel> Add(ColorModel item)
        {
            source.Add(item);
            return this;
        }

        public IDataContext<ColorModel> InsertAt(int index, ColorModel item)
        {
            source.Insert(index, item);
            return this;
        }

        public IDataContext<ColorModel> RemoveAt(int index)
        {
            source.RemoveAt(index);
            return this;
        }

        public IDataContext<ColorModel> ReplaceAt(int index, ColorModel item)
        {
            source[index] = item;
            return this;
        }

        /// <summary>
        /// Saves current <see cref="ColorClassList"/> to <see cref="colorsFilePath"/>.
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            try
            {
                File.WriteAllText(colorsFilePath, JsonConvert.SerializeObject(source));
                return true;
            }
            catch { return false; }
        }
    }
}