﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Collections.Concurrent;
using System.Windows;

namespace MVVM_Color_Utilities.Palette_Quantizers.Median_Cut
{
    public class MedianCutQuantizer :BaseColorQuantizer
    {
        #region Fields
        private List<MedianCutCube> cubeList = new List<MedianCutCube>();
        private ICollection<Int32> colorList = new List<Int32>();
        private List<Color> _palette = new List<Color>();
        #endregion

        #region Constructor
        public MedianCutQuantizer(ConcurrentDictionary<int,int> colorDictionary)
        {
            this.colorList= colorDictionary.Keys;
        }
        public MedianCutQuantizer() { }
        #endregion

        #region Properties
        public override string Name { get; }= "MedianCutQuantizer";

        public override List<Color> Palette
        {
            get
            {
                return _palette;
            }
            set
            {
                _palette = value;
            }
        }
        public override int GetColorCount 
        {
            get
            {
                return Palette.Count;
            }
        }
        #endregion

        #region Methods
        public override void SetColorList(ConcurrentDictionary<int, int> colorDictionary)
        {
            this.colorList = colorDictionary.Keys;
        }
        public void SplitCubes(Int32 colorCount)
        {
            // creates a holder for newly added cubes
            List<MedianCutCube> newCubes = new List<MedianCutCube>();

            foreach (MedianCutCube cube in cubeList)
            {
                // if another new cubes should be over the top; don't do it and just stop here
                if (newCubes.Count >= colorCount) break;

                cube.SplitAtMedian(cube.ChannelIndex, out MedianCutCube newMedianCutCubeA
                    , out MedianCutCube newMedianCutCubeB);

                // adds newly created cubes to our list; but one by one and if there's enough cubes stops the process
                newCubes.Add(newMedianCutCubeA);
                if (newCubes.Count >= colorCount) break;
                newCubes.Add(newMedianCutCubeB);
            }

            // clears the old cubes
            cubeList = new List<MedianCutCube>();

            // adds the new cubes to the official cube list
            foreach (MedianCutCube medianCutCube in newCubes)
            {
                cubeList.Add(medianCutCube);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="colorCount"></param>
        /// <returns></returns>
        public override List<Color> GetPalette(Int32 colorCount)
        {
            cubeList.Clear();
            cubeList.Add(new MedianCutCube(colorList));

            if (colorList.Count == 0)//Returns empty if it has nothing to sort through.
            {
                return Palette;
            }
            Palette.Clear();

            // finds the minimum iterations needed to achieve the cube count (color count) we need
            Int32 iterationCount = 1;
            while ((1 << iterationCount) < colorCount) { iterationCount++; }

            for (Int32 iteration = 0; iteration < iterationCount; iteration++)
            {
                SplitCubes(colorCount);
            }

            // initializes the result palette
            Int32 paletteIndex = 0;

            // adds all the cubes' colors to the palette, and mark that cube with palette index for later use
            foreach (MedianCutCube cube in cubeList)
            {
                Palette.Add(cube.AverageColor);
                cube.PaletteIndex = paletteIndex++;
            }
            // returns the palette (should contain <= ColorCount colors)
            return Palette;
        }

        public override Int32 GetPaletteIndex(Color color)
        {
            //If palette doesnt include color then -1 is returned
            //If palette isnt formed
            if (Palette.Count == 0)
            {
                return 0;
            }
            //Test every cube for whether it contains the target color
            foreach(MedianCutCube cube in cubeList)
            {
                if (cube.IsColorIn(color))
                {
                    //MessageBox.Show(cube.)
                    return cube.PaletteIndex;
                }
            }
            return 0;
        }
        #endregion
    }
}
