using RPiRgbLEDMatrix;

namespace TrainBoard.Utilities;

public static class ColourConverter
{
    /// <summary>
    /// Converts a 32-bit integer representation of a color to an RgbColor struct.
    /// </summary>
    /// <param name="colorInt">The integer representation of the color.</param>
    /// <returns>An RgbColor struct containing the R, G, and B components.</returns>
    public static Color IntToRgb(int colorInt)
    {
        byte r = (byte)((colorInt >> 16) & 0xFF);
        byte g = (byte)((colorInt >> 8) & 0xFF);
        byte b = (byte)(colorInt & 0xFF);

        return new Color(r, g, b);
    }

    /// <summary>
    /// Converts an RgbColor struct to a single 32-bit integer representation.
    /// </summary>
    /// <param name="color">The RgbColor struct to convert.</param>
    /// <returns>A 32-bit integer representing the color.</returns>
    public static int RgbToInt(Color color)
    {
        return (color.R << 16) | (color.G << 8) | color.B;
    }
}