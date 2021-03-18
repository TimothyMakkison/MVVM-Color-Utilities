using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;

namespace MVVM_Color_Utilities.Palette_Quantizers.Median_Cut
{
    public class MedianCutQuantizer : BaseColorQuantizer
    {
        //Created by Smart K8 at:
        //https://www.codeproject.com/Articles/66341/A-Simple-Yet-Quite-Powerful-Palette-Quantizer-in-C

        #region Fields

        private List<MedianCutCube> cubeList = new List<MedianCutCube>();

        #endregion Fields

        #region Properties

        public override string Name => "MedianCut Quantizer";

        public override List<Color> Palette { get; set; } = new List<Color>();

        #endregion Properties

        #region Methods

        private void SplitCubes(int colorCount)
        {
            // creates a holder for newly added cubes
            List<MedianCutCube> newCubes = new List<MedianCutCube>();

            foreach (MedianCutCube cube in cubeList)
            {
                // if another new cubes should be over the top; don't do it and just stop here
                if (newCubes.Count >= colorCount)
                {
                    break;
                }

                cube.SplitAtMedian(cube.ChannelIndex, out MedianCutCube newMedianCutCubeA,
                    out MedianCutCube newMedianCutCubeB);

                // adds newly created cubes to our list; but one by one and if there's enough cubes stops the process
                newCubes.Add(newMedianCutCubeA);
                if (newCubes.Count >= colorCount)
                {
                    break;
                }

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
        /// Generates Palette of same length or less than colorCount.
        /// </summary>
        /// <param name="colorCount"></param>
        /// <returns></returns>
        public override List<Color> GetPalette(int colorCount, ConcurrentDictionary<int, int> colorDictionary)
        {
            cubeList.Clear();
            cubeList.Add(new MedianCutCube(colorDictionary.Keys));
            Palette.Clear();

            if (colorDictionary.Count == 0)//Returns empty if it has nothing to sort through.
            {
                return Palette;
            }

            // finds the minimum iterations needed to achieve the cube count (color count) we need
            int iterationCount = 1;
            while ((1 << iterationCount) < colorCount) { iterationCount++; }//Equivalent of Log2(colorCount)

            for (int iteration = 0; iteration < iterationCount; iteration++)
            {
                SplitCubes(colorCount);
            }

            // initializes the result palette
            int paletteIndex = 0;

            // adds all the cubes' colors to the palette, and mark that cube with palette index for later use
            foreach (MedianCutCube cube in cubeList)
            {
                Palette.Add(cube.AverageColor);
                cube.PaletteIndex = paletteIndex++;
            }
            // returns the palette (should contain <= ColorCount colors)
            return Palette;
        }

        /// <summary>
        /// Returns index of the most similar color in Palette.
        /// </summary>
        /// <param name="color">Target Color</param>
        /// <returns></returns>
        public override int GetPaletteIndex(Color color)
        {
            //If palette isnt formed
            if (Palette.Count == 0)
            {
                return 0;
            }
            //Test every cube for whether it contains the target color
            foreach (MedianCutCube cube in cubeList)
            {
                if (cube.IsColorIn(color))
                {
                    return cube.PaletteIndex;
                }
            }
            //If palette doesnt include color then 0 is returned
            return 0;
        }

        #endregion Methods
    }
}