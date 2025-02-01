namespace Cherris;

public static class TypeParser
{
    public static Vector2 ParseVector(object list)
    {
        var listList = (List<object>)list;

        float x;
        float y;

        try
        {
            x = (float)Convert.ToDouble(listList[0]);
            y = (float)Convert.ToDouble(listList[1]);
        }
        catch (InvalidCastException e)
        {
            throw new ArgumentException("Vector components must be integers or string representations of integers.", e);
        }

        return new(x, y);
    }

    public static Color ParseColor(object list)
    {
        var listList = (List<object>)list;

        int r, g, b;

        try
        {
            r = Convert.ToInt32(listList[0]);
            g = Convert.ToInt32(listList[1]);
            b = Convert.ToInt32(listList[2]);
        }
        catch (InvalidCastException e)
        {
            throw new ArgumentException("Color components must be integers or string representations of integers.", e);
        }

        // Optional: Validate that the values are within 0-255
        if (r < 0 || r > 255 || g < 0 || g > 255 || b < 0 || b > 255)
        {
            throw new ArgumentException("Color components must be between 0 and 255.");
        }

        return new(r, g, b, 255);
    }
}