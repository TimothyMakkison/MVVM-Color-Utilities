using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MVVM_Color_Utilities.Palette_Quantizers.Median_Cut
{
    class MedianCutQuantizer
    {
        #region Fields
        private List<MedianCutCube> cubeList = new List<MedianCutCube>();
        #endregion

        #region Constructor
        public MedianCutQuantizer(ICollection<Int32> colorList)
        {
            cubeList.Add(new MedianCutCube(colorList));
        }
        #endregion

        #region Methods

        public void SplitCubes(Int32 colorCount)
        {
            // creates a holder for newly added cubes
            List<MedianCutCube> newCubes = new List<MedianCutCube>();

            foreach (MedianCutCube cube in cubeList)
            {
                // if another new cubes should be over the top; don't do it and just stop here
                if (newCubes.Count >= colorCount) break;

                MedianCutCube newMedianCutCubeA, newMedianCutCubeB;

                cube.SplitAtMedian(cube.ChannelIndex, out newMedianCutCubeA, out newMedianCutCubeB);

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

        public List<Color> GetPalette(Int32 colorCount)
        {
            // finds the minimum iterations needed to achieve the cube count (color count) we need
            Int32 iterationCount = 1;
            while ((1 << iterationCount) < colorCount) { iterationCount++; }

            for (Int32 iteration = 0; iteration < iterationCount; iteration++)
            {
                SplitCubes(colorCount);
            }

            // initializes the result palette
            List<Color> result = new List<Color>();
            Int32 paletteIndex = 0;

            // adds all the cubes' colors to the palette, and mark that cube with palette index for later use
            foreach (MedianCutCube cube in cubeList)
            {
                result.Add(cube.AverageColor);
                cube.PaletteIndex = paletteIndex++;
            }

            // returns the palette (should contain <= ColorCount colors)
            return result;
        }
        #endregion
    }
}
