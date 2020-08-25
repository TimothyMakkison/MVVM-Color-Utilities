using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace MVVM_Color_Utilities.Palette_Quantizers.Octree
{
    public class OctreeNode : List<Color>
    {
        #region Fields

        private Color averageColor;

        private readonly int redLowerBound, redUpperBound;
        private readonly int greenLowerBound, greenUpperBound;
        private readonly int blueLowerBound, blueUpperBound;

        #endregion Fields

        #region Constructor

        public OctreeNode(int redLowerBound, int greenLowerBound, int blueLowerBound, int size)
        {
            this.redLowerBound = redLowerBound;
            this.greenLowerBound = greenLowerBound;
            this.blueLowerBound = blueLowerBound;
            Size = size;

            int distance = Size - 1;
            redUpperBound = redLowerBound + distance;
            greenUpperBound = greenLowerBound + distance;
            blueUpperBound = blueLowerBound + distance;
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Size of node.
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// Palette color index of this node.
        /// </summary>
        public int PaletteIndex { get; set; }

        /// <summary>
        /// Average color of node, found by averaging every color item.
        /// </summary>
        public Color AverageColor
        {
            get
            {
                if (averageColor == default)
                {
                    GetAverageColor();
                }
                return averageColor;
            }
        }

        #endregion Properties

        #region Method

        #region InBounds

        /// <summary>
        /// Returns boolean value of whether node contains color.
        /// </summary>
        /// <param name="color">Target color</param>
        /// <returns></returns>
        public bool InBounds(Color color) => color.R >= redLowerBound && color.R <= redUpperBound
                && color.G >= greenLowerBound && color.G <= greenUpperBound
                && color.B >= blueLowerBound && color.B <= blueUpperBound;

        #endregion InBounds

        #region Split

        /// <summary>
        /// Creates 8 sub nodes from this node.
        /// </summary>
        /// <returns>All non leaf nodes</returns>
        public IEnumerable<OctreeNode> GetChildCubes()
        {
            int halfSize = Size / 2;

            //Subdivides this node into 8 nodes of varying positions.
            OctreeNode[] nodes = new OctreeNode[8]
            {
                new OctreeNode(redLowerBound, greenLowerBound, blueLowerBound, halfSize),
                new OctreeNode(redLowerBound, greenLowerBound, blueLowerBound + halfSize, halfSize),
                new OctreeNode(redLowerBound, greenLowerBound + halfSize, blueLowerBound, halfSize),
                new OctreeNode(redLowerBound, greenLowerBound + halfSize, blueLowerBound + halfSize, halfSize),

                new OctreeNode(redLowerBound + halfSize, greenLowerBound, blueLowerBound, halfSize),
                new OctreeNode(redLowerBound + halfSize, greenLowerBound, blueLowerBound + halfSize, halfSize),
                new OctreeNode(redLowerBound + halfSize, greenLowerBound + halfSize, blueLowerBound, halfSize),
                new OctreeNode(redLowerBound + halfSize, greenLowerBound + halfSize, blueLowerBound + halfSize, halfSize),
            };

            //Adds each color to its corresponding cube.
            foreach (var color in this)
            {
                int index = 0;

                index += color.R >= redLowerBound && color.R < redLowerBound + halfSize ? 0 : 4;
                index += color.G >= greenLowerBound && color.G < greenLowerBound + halfSize ? 0 : 2;
                index += color.B >= blueLowerBound && color.B < blueLowerBound + halfSize ? 0 : 1;

                nodes[index].Add(color);
            }

            //Find all non empty nodes
            return nodes.Where(x => x.Count() > 0);
        }

        #endregion Split

        #region Get average color

        /// <summary>
        /// Calculates the average color of this node.
        /// </summary>
        private Color GetAverageColor()
        {
            if (Count == 0)
            {
                return Color.FromArgb(0, 0, 0);
            }
            else
            {
                int red = 0;
                int green = 0;
                int blue = 0;

                foreach (var color in this)
                {
                    red += color.R;
                    green += color.G;
                    blue += color.B;
                }

                //Calculates average color channels from sum of values, returns zero if
                red /= Count;
                green /= Count;
                blue /= Count;

                return averageColor = Color.FromArgb(255, red, green, blue);
            }
        }

        #endregion Get average color

        #region Override

        public override string ToString() => $"Octree node of size: {Size}, index: {PaletteIndex} and count {Count}" +
                $"\n Color: {AverageColor}" +
                $"\n Red: {redLowerBound} - {redUpperBound}," +
                $" Green: {greenLowerBound} - {greenUpperBound}," +
                $" Blue: {blueLowerBound} - {blueUpperBound}";

        #endregion Override

        #endregion Method
    }
}