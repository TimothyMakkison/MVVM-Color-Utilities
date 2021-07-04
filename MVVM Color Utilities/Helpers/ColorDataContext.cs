using MVVM_Color_Utilities.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace MVVM_Color_Utilities.Helpers
{
    internal class ColorDataContext : IDataContext<ColorModel>
    {
        private const string colorsFilePath = "Resources/ColorItemsList.txt";
        private readonly ObservableCollection<ColorModel> _source;

        public ColorDataContext()
        {
            var jsonString = File.ReadAllText(colorsFilePath);
            this._source = JsonConvert.DeserializeObject<ObservableCollection<ColorModel>>(jsonString);
            this._source ??= new ObservableCollection<ColorModel>();
        }

        /// <summary>
        /// Returns an ObservableCollection containing ColorClass objects.
        /// </summary>
        IEnumerable<ColorModel> IDataContext<ColorModel>.Source => _source;

        ObservableCollection<ColorModel> IDataContext<ColorModel>.Observable => _source;

        public IDataContext<ColorModel> Add(ColorModel item)
        {
            _source.Add(item);
            return this;
        }

        public IDataContext<ColorModel> InsertAt(int index, ColorModel item)
        {
            _source.Insert(index, item);
            return this;
        }

        public IDataContext<ColorModel> RemoveAt(int index)
        {
            _source.RemoveAt(index);
            return this;
        }

        public IDataContext<ColorModel> ReplaceAt(int index, ColorModel item)
        {
            _source[index] = item;
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
                File.WriteAllText(colorsFilePath, JsonConvert.SerializeObject(_source));
                return true;
            }
            catch { return false; }
        }
    }
}