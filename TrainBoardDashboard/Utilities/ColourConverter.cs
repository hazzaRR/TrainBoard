namespace TrainBoardDashboard.Utilities;

public static class ColourConverter
{
    /// <summary>
    /// Converts a hexadecimal color string (e.g., "#FF0080" or "FF0080") to a 32-bit integer.
    /// </summary>
    /// <param name="hex">The hexadecimal string representation of the color.</param>
    /// <returns>The integer representation of the color.</returns>
    /// <exception cref="ArgumentException">Thrown if the hex string is not in a valid format.</exception>
    public static int HexToInt(string hex)
    {
        if (string.IsNullOrEmpty(hex))
        {
            throw new ArgumentException("Hex string cannot be null or empty.");
        }
        string hexValue = hex.TrimStart('#');
        if (hexValue.Length != 6)
        {
            throw new ArgumentException("Invalid hex string format. Must be 6 characters long.");
        }

        try
        {
            return Convert.ToInt32(hexValue, 16);
        }
        catch (FormatException)
        {
            throw new ArgumentException("Invalid hexadecimal characters in the string.");
        }
    }

    /// <summary>
    /// Converts a 32-bit integer representation of a color to a hexadecimal string.
    /// </summary>
    /// <param name="colorInt">The integer representation of the color.</param>
    /// <returns>A 6-character hexadecimal string prefixed with '#'.</returns>
    public static string IntToHex(int colorInt)
    {
        return $"#{colorInt:X6}";
    }
}