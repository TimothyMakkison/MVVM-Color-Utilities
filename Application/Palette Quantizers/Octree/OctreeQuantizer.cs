using Application.Helpers.DistanceCalculator;
using MoreLinq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Application.Palette_Quantizers.Octree
{
    public class OctreeQuantizer : IColorQuantizer
    {
        private List<OctreeNode> octreeNodes = new List<OctreeNode>();
        private readonly IDistanceCalculator distanceCalculator = new ManhattenDistance();

        public string Name => "Octree Quantizer";
        public List<Color> Palette { get; set; } = new List<Color>();

        /// <summary>
        /// Generates a color palette of a specified size from colors in an image.
        /// </summary>
        /// <param name="colorCount">Specifies size of palette</param>
        /// <param name="colorDictionary">Colors from image</param>
        /// <returns>Color palette</returns>
        public List<Color> GetPalette(int colorCount, ConcurrentDictionary<int, int> colorDictionary)
        {
            octreeNodes.Clear();

            //Create encompasing parent node.
            OctreeNode node = new OctreeNode(0, 0, 0, 256);

            //Iterate through each input color adding each to encompassing node
            foreach (KeyValuePair<int, int> item in colorDictionary)
            {
                Color color = Color.FromArgb(item.Key);
                node.AddRange(Enumerable.Repeat(color, item.Value));
            }
            octreeNodes.Add(node);

            // Divide the largest node until all leaves are found or required number of colors are found.
            for (int i = 0; i < colorCount && octreeNodes.Count < colorCount; i++)
            {
                // Find largest node and split
                // remove node and replace with children nodes
                OctreeNode largest = octreeNodes.MaxBy(x => x.Count).FirstOrDefault();
                octreeNodes.AddRange(largest.GetChildCubes());
                octreeNodes.Remove(largest);
            }

            //Trim excess items by keeping the largest nodes and discarding the smallest.
            if (octreeNodes.Count > colorCount)
            {
                var sorted = octreeNodes.OrderByDescending(x => x.Count);
                octreeNodes = sorted.Take(colorCount).ToList();
            }

            //Assign palette index
            for (int i = 0; i < octreeNodes.Count; i++)
            {
                octreeNodes[i].PaletteIndex = i;
            }

            return Palette = octreeNodes.Select(x => x.AverageColor)
                                        .ToList(); ;
        }

        /// <summary>
        /// Finds the palette index of a similar color.
        /// </summary>
        /// <param name="color">Target color</param>
        /// <returns>Index of similar color</returns>
        public int GetPaletteIndex(Color color)
        {
            //Gets index of containing node
            foreach (var node in octreeNodes)
            {
                if (node.InBounds(color))
                {
                    return node.PaletteIndex;
                }
            }

            //If a node doesnt contain the color then the closest color is used as a substitute
            return octreeNodes.MinBy(x => distanceCalculator.Distance(x.AverageColor, color))
                .FirstOrDefault().PaletteIndex;
        }
    }
}