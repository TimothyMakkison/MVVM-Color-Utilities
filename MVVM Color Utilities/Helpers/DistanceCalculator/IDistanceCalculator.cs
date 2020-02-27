using System.Drawing;

namespace MVVM_Color_Utilities.Helpers.DistanceCalculator
{
    public interface IDistanceCalculator
    {
        int Distance(Color a, Color b);
    }
}
