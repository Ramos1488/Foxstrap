using System.Windows.Media;

namespace Foxstrap.Extensions
{
    public static class ColorExtensions
    {
        public static SolidColorBrush ToBrush(this Color color) =>
            new SolidColorBrush(color);

        public static Color WithAlpha(this Color color, byte alpha) =>
            Color.FromArgb(alpha, color.R, color.G, color.B);
    }
}
