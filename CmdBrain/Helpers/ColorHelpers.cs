using System.Drawing;

namespace No8.CmdBrain;

public static class ColorHelpers
{
    /// <summary>
    /// Adjust brightness of color by factor.
    /// Factor must be between -1.0 and 1.0
    /// </summary>
    public static Color AdjustBy(this Color color, float factor)
    {
        var red = (float)color.R;
        var green = (float)color.G;
        var blue = (float)color.B;

        if (factor < 0)
        {
            factor = 1 + factor;
            red *= factor;
            green *= factor;
            blue *= factor;
        }
        else
        {
            red = (255 - red) * factor + red;
            green = (255 - green) * factor + green;
            blue = (255 - blue) * factor + blue;
        }

        return Color.FromArgb(color.A, (int)red, (int)green, (int)blue);
    }

    public static ushort ToTerminalColor(this Color c)
    {
        ushort result = 0x00;

        if (c == Color.DarkRed) return 0x0004; // (ushort)Windows.CharacterAttributes.FgDarkRed;
        if (c == Color.DarkGreen) return 0x0002; // (ushort)Windows.CharacterAttributes.FgDarkGreen;
        if (c == Color.DarkBlue) return 0x0001; // (ushort)Windows.CharacterAttributes.FgDarkBlue;
        if (c == Color.DarkCyan) return 0x0003; // (ushort)Windows.CharacterAttributes.FgDarkCyan;
        if (c == Color.DarkMagenta) return 0x0005; // (ushort)Windows.CharacterAttributes.FgDarkMagenta;
        if (c == Color.Gold) return 0x0006; // (ushort)Windows.CharacterAttributes.FgDarkYellow;
        if (c == Color.Gray) return 0x0007; // (ushort)Windows.CharacterAttributes.FgGrey;
        if (c == Color.Red) return 0x000C; // (ushort)Windows.CharacterAttributes.FgRed;
        if (c == Color.Green) return 0x000A; // (ushort)Windows.CharacterAttributes.FgGreen;
        if (c == Color.Blue) return 0x0009; // (ushort)Windows.CharacterAttributes.FgBlue;
        if (c == Color.Cyan) return 0x000B; //  (ushort)Windows.CharacterAttributes.FgCyan;
        if (c == Color.Magenta) return 0x000D; // (ushort)Windows.CharacterAttributes.FgMagenta;
        if (c == Color.Yellow) return 0x000E; // (ushort)Windows.CharacterAttributes.FgYellow;
        if (c == Color.White) return 0x000F; // (ushort)Windows.CharacterAttributes.FgWhite;

        if (c.R >= 128) result += 0x04;
        if (c.G >= 128) result += 0x02;
        if (c.B >= 128) result += 0x01;
        if (c.R >= 250 || c.G >= 250 || c.B >= 250)
            result += 0x08; // intense

        return result;
    }

    public static Color BlendRGB(this Color c1, Color other, double t)
    {
        return Color.FromArgb(
            (int)(c1.R + t * (other.R - c1.R)),
            (int)(c1.G + t * (other.G - c1.G)),
            (int)(c1.B + t * (other.B - c1.B))
        );
    }

    public static Color BlendLAB(this Color c1, Color c2, double t)
    {
        var (l1, a1, b1) = ((Colorful)c1).AsLAB();
        var (l2, a2, b2) = ((Colorful)c2).AsLAB();
        
        return Colorful.CreateLAB(
            l1 + t * (l2 - l1),
            a1 + t * (a2 - a1),
            b1 + t * (b2 - b1));
    }

    public static Color BlendHSV(this Color c1, Color c2, double t)
    {
        var (h1, s1, v1) = ((Colorful)c1).AsHSV();
        var (h2, s2, v2) = ((Colorful)c2).AsHSV();

        // We know that h are both in [0-360]
        return Colorful.CreateHSV(
            Colorful.InterpAngle(h1, h2, t), 
            s1 + t * (s2 - s1), 
            v1 + t * (v2 - v1));
    }

    public static Color BlendHCL(this Color col1, Color col2, double t)
    {
        var (h1, c1, l1) = ((Colorful)col1).AsHCL();
        var (h2, c2, l2) = ((Colorful)col2).AsHCL();

        // We know that h are both in [0..360]
        return Colorful.CreateHCL(
            Colorful.InterpAngle(h1, h2, t), 
            c1 + t * (c2 - c1), 
            l1 + t * (l2 - l1))
            .Clamped();
    }

    public static Color BlendLUV(this Color c1, Color c2, double t)
    {
        var (l1, u1, v1) = ((Colorful)c1).AsLUV();
        var (l2, u2, v2) = ((Colorful)c2).AsLUV();
        return Colorful.CreateLUV(
            l1 + t * (l2 - l1),
            u1 + t * (u2 - u1),
            v1 + t * (v2 - v1));
    }

}