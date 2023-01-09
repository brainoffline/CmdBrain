using System.Drawing;

namespace No8.CmdBrain;

// A color is stored internally using sRGB (standard RGB) values in the range 0-1
public readonly struct Colorful
{
    public static readonly Colorful Empty = new(0, 0, 0);

    // This is the tolerance used when comparing colors using AlmostEqualRgb.
    private const double Delta = 1.0 / 255.0;

    // Default reference white points
    public static readonly double[] D65 = { 0.95047, 1.00000, 1.08883 };
    public static readonly double[] D50 = { 0.96422, 1.00000, 0.82521 };

    public readonly double R;
    public readonly double G;
    public readonly double B;

    /// <summary>
    ///     Create color with RGB values between 0.0 and 1.0
    /// </summary>
    /// <remarks>value are not enforced to allow for other color spaces</remarks>
    public Colorful(double r, double g, double b)
    {
        R = r;
        G = g;
        B = b;
    }

    /// <summary>
    ///     Create color with RGB values between 0 and 255
    /// </summary>
    public Colorful(byte r, byte g, byte b)
    {
        R = Clamp01(r / 255.0);
        G = Clamp01(g / 255.0);
        B = Clamp01(b / 255.0);
    }


    /// <summary>
    ///     Return RGBA as uint [0..65535]
    /// </summary>
    public (uint r, uint g, uint b, uint a) AsRGBA()
    {
        return (
            (uint)(R * 65535.0 + 0.5),
            (uint)(G * 65535.0 + 0.5),
            (uint)(B * 65535.0 + 0.5),
            0xFFFF);
    }

    /// <summary>
    ///     Explicitly convert between Color and Colorful
    /// </summary>
    public static implicit operator Colorful(Color color)
    {
        (double r, double g, double b, double a) = (color.R, color.G, color.B, color.A);

        if (a.IsZero())
            return Empty;
        if (a.Is(255))
            return new(color.R, color.G, color.B);

        // Since Colorful is alpha pre-multiplied, we need to divide the
        // RGB values by alpha again in order to get back the original RGB.
        r *= 0xffff;
        r /= a;
        g *= 0xffff;
        g /= a;
        b *= 0xffff;
        b /= a;

        return new((r / 65535.0), (g / 65535.0), (b / 65535.0));
    }

    /// <summary>
    ///     Explicitly convert between Color and Colorful
    /// </summary>
    public static implicit operator Color(Colorful color)
    {
        var (r, g, b) = color.AsRGB255();
        return Color.FromArgb(r, g, b);
    }

    /// <summary>
    ///     Return RGB as [0..255]
    /// </summary>
    public (byte r, byte g, byte b) AsRGB255()
    {
        return (
            (byte)(R * 255.0 + 0.5),
            (byte)(G * 255.0 + 0.5),
            (byte)(B * 255.0 + 0.5)
        );
    }

    /// <summary>
    ///     Validated color is in RGB color space
    /// </summary>
    public bool IsValid()
    {
        return 0.0 <= R && R <= 1.0 &&
               0.0 <= G && G <= 1.0 &&
               0.0 <= B && B <= 1.0;
    }

    internal static double Clamp01(double v) => Math.Clamp(v, 0.0, 1.0);

    /// <summary>
    ///     Ensures RGB stay between [0.0-1.0]
    /// </summary>
    public Colorful Clamped()
    {
        return new(
            Clamp01(R),
            Clamp01(G),
            Clamp01(B)
        );
    }

    internal static double Square(double value)
    {
        return value * value;
    }

    internal static double Cube(double value)
    {
        return value * value * value;
    }

    /// <summary>
    ///     RGB Distance to other color.
    ///     Not the best measure. LAB color space is a better measure
    /// </summary>
    public double DistanceRGB(Colorful other)
    {
        return Math.Sqrt(Square(R - other.R) + Square(G - other.G) + Square(B - other.B));
    }

    /// <summary>
    ///     RGB distance to other color in Linear RGB color space
    ///     Maybe useful for dithering
    /// </summary>
    public double DistanceLinearRGB(Colorful other)
    {
        var (r1, g1, b1) = AsLinearRGB();
        var (r2, g2, b2) = other.AsLinearRGB();
        return Math.Sqrt(Square(r1 - r2) + Square(g1 - g2) + Square(b1 - b2));
    }

    /// <summary>
    ///     RGB distance to other color using Riemersma algorithm
    /// </summary>
    /// <remarks>
    ///     https://www.compuphase.com/cmetric.htm
    ///     https://github.com/lucasb-eyer/go-colorful/issues/52
    /// </remarks>
    public double DistanceRiemersma(Colorful other)
    {
        var rAvg = (R + other.R) / 2.0;

        // Deltas
        var dR = R - other.R;
        var dG = G - other.G;
        var dB = B - other.B;

        return Math.Sqrt((2.0 + rAvg) * dR * dR + 4.0 * dG * dG + (2.0 + (1.0 - rAvg)) * dB * dB);
    }

    /// <summary>
    ///     Compare two Colorful value with approximate resolution
    /// </summary>
    public bool AlmostEqualRGB(Colorful other)
    {
        return Math.Abs(R - other.R) +
            Math.Abs(G - other.G) +
            Math.Abs(B - other.B) < 3 * Delta;
    }

    /// <summary>
    ///     Blend between two colors at a particular distance [0.0-1.0] using RGB color space
    /// </summary>
    /// <remarks>
    ///     BlendLAB, BlendLUV or BlendHCL may give better results
    /// </remarks>
    public Colorful BlendRGB(Colorful other, double t)
    {
        return new(
            R + t * (other.R - R),
            G + t * (other.G - G),
            B + t * (other.B - B)
        );
    }

    // Used by Hxx color spaces for interpolating between two angles in [0-360].
    internal static double InterpAngle(double a0, double a1, double t)
    {
        // Based on the answer here: http://stackoverflow.com/a/14498790/2366315
        // With potential proof that it works here: http://Math.stackexchange.com/a/2144499
        var delta = (((a1 - a0 % 360.0) + 540) % 360.0) - 180.0;
        return (a0 + t * delta + 360.0) % 360.0;
    }

    //--- HSV ---------------------------------------------------------------------------------

    /// <summary>
    ///     Returns color as HSV values
    ///     Hue is between [0-360]
    ///     Saturation is between [0.0-1.0]
    ///     Value is between [0.0-1.0]
    /// </summary>
    /// <remarks>
    ///     from http://en.wikipedia.org/wiki/HSL_and_HSV
    /// </remarks>
    public (double h, double s, double v) AsHSV()
    {
        var min = Math.Min(Math.Min(R, G), B);
        var v = Math.Max(Math.Max(R, G), B);
        var c = v - min;

        var s = 0.0;
        if (v.IsNotZero())
            s = c / v;

        var h = 0.0;
        if (min.IsNot(v))
        {
            if (v.Is(R))
                h = ((G - B) / c % 6.0);
            if (v.Is(G))
                h = (B - R) / c + 2.0;
            if (v.Is(B))
                h = (R - G) / c + 4.0;

            h *= 60.0;
            if (h < 0.0)
                h += 360.0;
        }

        return (h, s, v);
    }

    /// <summary>
    ///     Creates new Colorful using HSV color space
    ///     Hue is between [0-360]
    ///     Saturation is between [0.0-1.0]
    ///     Value is between [0.0-1.0]
    /// </summary>
    public static Colorful CreateHSV(double h, double s, double v)
    {
        var hp = h / 60.0;
        var c = v * s;
        var x = c * (1.0 - Math.Abs((hp % 2.0) - 1.0));

        var m = v - c;
        var r = 0.0;
        var g = 0.0;
        var b = 0.0;

        if (hp.Within(0.0, 1.0))
        {
            r = c;
            g = x;
        }
        else if (hp.Within(1.0, 2.0))
        {
            r = x;
            g = c;
        }
        else if (hp.Within(2.0, 3.0))
        {
            g = c;
            b = x;
        }
        else if (hp.Within(3.0, 4.0))
        {
            g = x;
            b = c;
        }
        else if (hp.Within(4.0, 5.0))
        {
            r = x;
            b = c;
        }
        else if (hp.Within(5.0, 6.0))
        {
            r = c;
            b = x;
        }

        return new(
            m + r,
            m + g,
            m + b
        );
    }

    /// <summary>
    ///     Blend between two colors at a particular distance [0.0-1.0] using HSV color space
    /// </summary>
    /// <remarks>
    ///     BlendLAB, BlendLUV or BlendHCL may give better results
    /// </remarks>

    public Colorful BlendHSV(Colorful other, double t)
    {
        var (h1, s1, v1) = AsHSV();
        var (h2, s2, v2) = other.AsHSV();

        // We know that h are both in [0..360]
        return CreateHSV(InterpAngle(h1, h2, t), s1 + t * (s2 - s1), v1 + t * (v2 - v1));
    }

    //--- HSL ---------------------------------------------------------------------------------

    /// <summary>
    ///     Returns color is HSL color space
    ///     Hue is between [0-360]
    ///     Saturation is between [0.0-1.0]
    ///     Luminance is between [0.0-1.0]
    /// </summary>
    public (double h, double s, double l) AsHSL()
    {
        var min = Math.Min(Math.Min(R, G), B);
        var max = Math.Max(Math.Max(R, G), B);

        var l = (max + min) / 2;
        double h, s;

        if (min.Is(max))
        {
            s = 0;
            h = 0;
        }
        else
        {
            if (l < 0.5)
                s = (max - min) / (max + min);
            else
                s = (max - min) / (2.0 - max - min);

            if (max.Is(R))
                h = (G - B) / (max - min);
            else if (max.Is(G))
                h = 2.0 + (B - R) / (max - min);
            else
                h = 4.0 + (R - G) / (max - min);

            h *= 60;
            if (h < 0)
                h += 360;
        }

        return (h, s, l);
    }

    /// <summary>
    ///     Creates new Colorful using HSV color space
    ///     Hue is between [0-360]
    ///     Saturation is between [0.0-1.0]
    ///     Luminance is between [0.0-1.0]
    /// </summary>
    public static Colorful CreateHSL(double h, double s, double l)
    {
        if (s.IsZero())
            return new(l, l, l);

        double r, g, b;
        double t1;

        if (l < 0.5)
            t1 = l * (1.0 + s);
        else
            t1 = l + s - l * s;

        var t2 = 2.0 * l - t1;
        h /= 360;
        var tr = h + 1.0 / 3.0;
        var tg = h;
        var tb = h - 1.0 / 3.0;

        if (tr < 0)
            tr++;
        if (tr > 1.0)
            tr--;
        if (tg < 0)
            tg++;
        if (tg > 1.0)
            tg--;
        if (tb < 0)
            tb++;
        if (tb > 1.0)
            tb--;

        // Red
        if (6.0 * tr < 1.0)
            r = t2 + (t1 - t2) * 6 * tr;
        else if (2.0 * tr < 1.0)
            r = t1;
        else if (3.0 * tr < 2.0)
            r = t2 + (t1 - t2) * (2.0 / 3.0 - tr) * 6.0;
        else
            r = t2;

        // Green
        if (6.0 * tg < 1.0)
            g = t2 + (t1 - t2) * 6 * tg;
        else if (2.0 * tg < 1.0)
            g = t1;
        else if (3.0 * tg < 2.0)
            g = t2 + (t1 - t2) * (2.0 / 3.0 - tg) * 6.0;
        else
            g = t2;

        // Blue
        if (6.0 * tb < 1.0)
            b = t2 + (t1 - t2) * 6 * tb;
        else if (2.0 * tb < 1.0)
            b = t1;
        else if (3.0 * tb < 2.0)
            b = t2 + (t1 - t2) * (2.0 / 3.0 - tb) * 6.0;
        else
            b = t2;

        return new(r, g, b);
    }

    //--- Hex ---------------------------------------------------------------------------------

    /// <summary>
    ///     Return color using html format [#ff00bb]
    /// </summary>
    public string AsHex()
    {
        var (r, g, b) = AsRGB255();
        return $"#{r:x2}{g:x2}{b:x2}";
    }

    /// <summary>
    ///     Create Colorful by parsing html format [#RRGGBB, #rgb, color name]
    /// </summary>
    public static (Colorful, bool error) CreateHex(string htmlColor)
    {
        var color = ColorTranslator.FromHtml(htmlColor);
        if (color == Color.Empty)
            return (Empty, true);

        return (color, false);
    }

    //--- Linear RGB ---------------------------------------------------------------------------------
    // http://www.sjbrown.co.uk/2004/05/14/gamma-correct-rendering/
    // http://www.brucelindbloom.com/Eqn_RGB_to_XYZ.html

    internal static double Linearize(double value)
    {
        if (value <= 0.04045)
            return value / 12.92;

        return Math.Pow((value + 0.055) / 1.055, 2.4);
    }

    /// <summary>
    ///     Return RGB using Linear RGB color space
    /// </summary>
    public (double r, double g, double b) AsLinearRGB()
    {
        return (
            Linearize(R),
            Linearize(G),
            Linearize(B)
            );
    }

    /// <summary>
    ///     Taylor approximation of linear RGB
    /// </summary>
    internal static double LinearizeFast(double v)
    {
        var v1 = v - 0.5;
        var v2 = v1 * v1;
        var v3 = v2 * v1;
        var v4 = v2 * v2;
        //v5  = v3*v2

        return -0.248750514614486 + 0.925583310193438 * v + 1.16740237321695 * v2 + 0.280457026598666 * v3 -
               0.0757991963780179 * v4; //+ 0.0437040411548932*v5
    }

    /// <summary>
    ///     Return RGB using Linear RGB color space (faster and nearly as accurate as LinearRGB)
    /// </summary>
    public (double r, double g, double b) AsFastLinearRGB()
    {
        var r = LinearizeFast(R);
        var g = LinearizeFast(G);
        var b = LinearizeFast(B);
        return (r, g, b);
    }

    internal static double Delinearize(double v)
    {
        if (v <= 0.0031308)
            return 12.92 * v;

        return 1.055 * Math.Pow(v, 1.0 / 2.4) - 0.055;
    }

    /// <summary>
    ///     Create Colorful using LinearRGB color space
    /// </summary>
    /// <remarks>
    ///     http://www.sjbrown.co.uk/2004/05/14/gamma-correct-rendering/
    /// </remarks>
    public static Colorful AsLinearRGB(double r, double g, double b)
    {
        return new(
            Delinearize(r),
            Delinearize(g),
            Delinearize(b));
    }

    internal static double DelinearizeFast(double v)
    {
        // This function (fractional root) is much harder to make linear, so we need to split.
        if (v > 0.2)
        {
            var v1 = v - 0.6;
            var v2 = v1 * v1;
            var v3 = v2 * v1;
            var v4 = v2 * v2;
            var v5 = v3 * v2;
            return 0.442430344268235 + 0.592178981271708 * v - 0.287864782562636 * v2 + 0.253214392068985 * v3 -
                0.272557158129811 * v4 + 0.325554383321718 * v5;
        }
        if (v > 0.03)
        {
            var v1 = v - 0.115;
            var v2 = v1 * v1;
            var v3 = v2 * v1;
            var v4 = v2 * v2;
            var v5 = v3 * v2;
            return 0.194915592891669 + 1.55227076330229 * v - 3.93691860257828 * v2 + 18.0679839248761 * v3 -
                101.468750302746 * v4 + 632.341487393927 * v5;
        }
        else
        {
            var v1 = v - 0.015;
            var v2 = v1 * v1;
            var v3 = v2 * v1;
            var v4 = v2 * v2;
            var v5 = v3 * v2;
            // You can clearly see from the involved constants that the low-end is highly nonlinear.
            return 0.0519565234928877 + 5.09316778537561 * v - 99.0338180489702 * v2 + 3484.52322764895 * v3 -
                150028.083412663 * v4 + 7168008.42971613 * v5;
        }
    }

    /// <summary>
    ///     Create Colorful using FastLinearRGB color space
    /// </summary>
    public static Colorful CreateFastLinearRGB(double r, double g, double b)
    {
        return new(
            DelinearizeFast(r),
            DelinearizeFast(g),
            DelinearizeFast(b)
        );
    }

    /// <summary>
    ///     Converts from CIE XYZ-space to Linear RGB color space
    /// </summary>
    public static (double r, double g, double b) ConvertXYZToLinearRGB(double x, double y, double z)
    {
        var r = 3.2409699419045214 * x - 1.5373831775700935 * y - 0.49861076029300328 * z;
        var g = -0.96924363628087983 * x + 1.8759675015077207 * y + 0.041555057407175613 * z;
        var b = 0.055630079696993609 * x - 0.20397695888897657 * y + 1.0569715142428786 * z;
        return (r, g, b);
    }

    /// <summary>
    ///     Converts from Linear RGB to CIE XYZ-space color space
    /// </summary>
    public static (double x, double y, double z) ConvertLinearRGBToXYZ(double r, double g, double b)
    {
        var x = 0.41239079926595948 * r + 0.35758433938387796 * g + 0.18048078840183429 * b;
        var y = 0.21263900587151036 * r + 0.71516867876775593 * g + 0.072192315360733715 * b;
        var z = 0.019330818715591851 * r + 0.11919477979462599 * g + 0.95053215224966058 * b;
        return (x, y, z);
    }

    /// <summary>
    ///     Blend between two colors at a particular distance [0.0-1.0] using Linear RGB color space
    ///     Does not product dark colors near the center
    /// </summary>
    /// <remarks>
    ///     BlendLAB, BlendLUV or BlendHCL may give better results
    /// </remarks>
    public Colorful BlendLinearRGB(Colorful other, double t)
    {
        var (r1, g1, b1) = AsLinearRGB();
        var (r2, g2, b2) = other.AsLinearRGB();
        return AsLinearRGB(
            r1 + t * (r2 - r1),
            g1 + t * (g2 - g1),
            b1 + t * (b2 - b1)
        );
    }


    //--- XYZ ---------------------------------------------------------------------------------
    // http://www.sjbrown.co.uk/2004/05/14/gamma-correct-rendering/

    /// <summary>
    ///     Return color using XYZ color space
    /// </summary>
    public (double x, double y, double z) AsXYZ()
    {
        var (r, g, b) = AsLinearRGB();
        return ConvertLinearRGBToXYZ(r, g, b);
    }

    public static Colorful CreateXYZ(double x, double y, double z)
    {
        var (r, g, b) = ConvertXYZToLinearRGB(x, y, z);
        return AsLinearRGB(r, g, b);
    }

    //--- XYY ---------------------------------------------------------------------------------
    // http://www.brucelindbloom.com/Eqn_XYZ_to_xyY.html

    /// <summary>
    ///     Convert color inb XYZ color space to XYY color space
    /// </summary>
    public static (double x, double y, double outY) ConvertXYZToXYY(double x, double y, double z)
    {
        return ConvertXYZToXYYWhiteRef(x, y, z, D65);
    }

    /// <summary>
    ///     Convert XYZ color to XYY color using White reference
    /// </summary>
    public static (double x, double y, double yOut) ConvertXYZToXYYWhiteRef(double x, double y, double z, double[] whiteRef)
    {
        var yOut = y;
        var n = x + y + z;
        if (Math.Abs(n) < 1e-14)
        {
            // When we have black, Bruce Lindbloom recommends to use the reference white's chromacity for x and y.
            x = whiteRef[0] / (whiteRef[0] + whiteRef[1] + whiteRef[2]);
            y = whiteRef[1] / (whiteRef[0] + whiteRef[1] + whiteRef[2]);
        }
        else
        {
            x = x / n;
            y = y / n;
        }

        return (x, y, yOut);
    }

    /// <summary>
    ///     Convert XYY color to XYZ color space
    /// </summary>
    public static (double x, double yOut, double z) ConvertXYYToXYZ(double xx, double yy, double y2)
    {
        var yOut = y2;
        double x, z;

        if (-1e-14 < yy && yy < 1e-14)
        {
            x = 0.0;
            z = 0.0;
        }
        else
        {
            x = y2 / yy * xx;
            z = y2 / yy * (1.0 - xx - yy);
        }

        return (x, yOut, z);
    }

    /// <summary>
    ///     Returns color in XYY color space
    /// </summary>
    public (double x, double y, double Y) AsXYY()
    {
        var (x, y, z) = AsXYZ();
        return ConvertXYZToXYY(x, y, z);
    }

    /// <summary>
    ///     Return color using XYY color space with specified white reference
    /// </summary>
    /// <remarks>(Note that the reference white is only used for black input.)</remarks>
    public (double x, double y, double Y) AsXYYWhiteRef(double[] whiteRef)
    {
        var (x, y, z) = AsXYZ();
        return ConvertXYZToXYYWhiteRef(x, y, z, whiteRef);
    }

    /// <summary>
    ///     Create color using CIE xyY color space
    /// </summary>
    public static Colorful CreateXYY(double x, double y, double y2)
    {
        var (xx, yy, zz) = ConvertXYYToXYZ(x, y, y2);
        return CreateXYZ(xx, yy, zz);
    }

    //--- LAB ---------------------------------------------------------------------------------
    // http://en.wikipedia.org/wiki/Lab_color_space#CIELAB-CIEXYZ_conversions
    // For L*a*b*, we need to L*a*b*<->XYZ->RGB and the first one is device dependent.

    internal static double lab_f(double t)
    {
        if (t > 6.0 / 29.0 * 6.0 / 29.0 * 6.0 / 29.0)
            return Math.Cbrt(t);

        return t / 3.0 * 29.0 / 6.0 * 29.0 / 6.0 + 4.0 / 29.0;
    }

    /// <summary>
    ///     Convert XYZ color to LAB color
    /// </summary>
    public static (double l, double a, double b) ConvertXYZToLAB(double x, double y, double z)
    {
        // Use D65 white as reference point by default.
        // http://www.fredmiranda.com/forum/topic/1035332
        // http://en.wikipedia.org/wiki/Standard_illuminant
        return ConvertXYZToLABWhiteRef(x, y, z, D65);
    }

    /// <summary>
    ///     Convert XYZ color to LAB color using white reference
    /// </summary>
    public static (double l, double a, double b) ConvertXYZToLABWhiteRef(double x, double y, double z, double[] whiteRef)
    {
        var fy = lab_f(y / whiteRef[1]);
        var l = 1.16 * fy - 0.16;
        var a = 5.0 * (lab_f(x / whiteRef[0]) - fy);
        var b = 2.0 * (fy - lab_f(z / whiteRef[2]));
        return (l, a, b);
    }

    internal static double lab_finv(double t)
    {
        if (t > 6.0 / 29.0)
            return t * t * t;

        return 3.0 * 6.0 / 29.0 * 6.0 / 29.0 * (t - 4.0 / 29.0);
    }

    /// <summary>
    ///     Convert LAB color to XYZ color
    /// </summary>
    public static (double x, double y, double z) ConvertLABToXYZ(double l, double a, double b)
    {
        // D65 white (see above).
        return ConvertLABToXYZWhiteRef(l, a, b, D65);
    }

    /// <summary>
    ///     Convert LAB color to XYZ color using white reference
    /// </summary>
    public static (double x, double y, double z) ConvertLABToXYZWhiteRef(double l, double a, double b, double[] whiteRef)
    {
        var l2 = (l + 0.16) / 1.16;
        var x = whiteRef[0] * lab_finv(l2 + a / 5.0);
        var y = whiteRef[1] * lab_finv(l2);
        var z = whiteRef[2] * lab_finv(l2 - b / 2.0);
        return (x, y, z);
    }

    // Converts the given color to CIE L*a*b* space using D65 as reference white.

    /// <summary>
    ///     Return color using LAB color space
    /// </summary>
    public (double l, double a, double b) AsLAB()
    {
        var (x, y, z) = AsXYZ();
        return ConvertXYZToLAB(x, y, z);
    }

    /// <summary>
    ///     Return color from LAB color space and reference White
    /// </summary>
    public (double l, double a, double b) AsLABWhiteRef(double[] whiteRef)
    {
        var (x, y, z) = AsXYZ();
        return ConvertXYZToLABWhiteRef(x, y, z, whiteRef);
    }

    // Generates a color by using data given in CIE L*a*b* space using D65 as reference white.
    // WARNING: many combinations of `l`, `a`, and `b` values do not have corresponding
    // valid RGB values, check the FAQ in the README if you're unsure.
    /// <summary>
    ///     Create color using CIE L*A*B* color space
    /// </summary>
    public static Colorful CreateLAB(double l, double a, double b)
    {
        var (x, y, z) = ConvertLABToXYZ(l, a, b);
        return CreateXYZ(x, y, z);
    }

    /// <summary>
    ///     Create color using CIE L*A*B* color space using white reference
    /// </summary>
    public static Colorful CreateLABWhiteRef(double l, double a, double b, double[] whiteRef)
    {
        var (x, y, z) = ConvertLABToXYZWhiteRef(l, a, b, whiteRef);
        return CreateXYZ(x, y, z);
    }

    /// <summary>
    ///     LAB Distance to other color.
    /// </summary>
    public double DistanceLAB(Colorful other)
    {
        var (l1, a1, b1) = AsLAB();
        var (l2, a2, b2) = other.AsLAB();
        return Math.Sqrt(Square(l1 - l2) + Square(a1 - a2) + Square(b1 - b2));
    }

    /// <summary>
    ///     IE76 Distance to other color (same as DistanceLAB)
    /// </summary>
    public double DistanceCIE76(Colorful other)
    {
        return DistanceLAB(other);
    }

    /// <summary>
    ///     CIE94 Distance to other color
    /// </summary>
    public double DistanceCIE94(Colorful cr)
    {
        var (l1, a1, b1) = AsLAB();
        var (l2, a2, b2) = cr.AsLAB();

        // NOTE: Since all those formulas expect L,A,B values 100x larger than we
        //       have them in this library, we either need to adjust all constants
        //       in the formula, or convert the ranges of L,A,B before, and then
        //       scale the distances down again. The latter is less error-prone.
        (l1, a1, b1) = (l1 * 100.0, a1 * 100.0, b1 * 100.0);
        (l2, a2, b2) = (l2 * 100.0, a2 * 100.0, b2 * 100.0);

        var kl = 1.0; // 2.0 for textiles
        var kc = 1.0;
        var kh = 1.0;
        var k1 = 0.045; // 0.048 for textiles
        var k2 = 0.015; // 0.014 for textiles.

        var deltaL = l1 - l2;
        var c1 = Math.Sqrt(Square(a1) + Square(b1));
        var c2 = Math.Sqrt(Square(a2) + Square(b2));
        var deltaCab = c1 - c2;

        // Not taking Sqrt here for stability, and it's unnecessary.
        var deltaHab2 = Square(a1 - a2) + Square(b1 - b2) - Square(deltaCab);
        var sl = 1.0;
        var sc = 1.0 + k1 * c1;
        var sh = 1.0 + k2 * c1;

        var vL2 = Square(deltaL / (kl * sl));
        var vC2 = Square(deltaCab / (kc * sc));
        var vH2 = deltaHab2 / Square(kh * sh);

        return Math.Sqrt(vL2 + vC2 + vH2) * 0.01; // See above.
    }

    /// <summary>
    ///     CIEDE2000 distance to other color
    /// </summary>
    public double DistanceCIEDE2000(Colorful other)
    {
        return DistanceCIEDE2000KLCH(other, 1.0, 1.0, 1.0);
    }

    /// <summary>
    ///     CIEDE2000klch distance to other color with custom weighted values
    /// </summary>
    public double DistanceCIEDE2000KLCH(Colorful other, double kl, double kc, double kh)
    {
        var (l1, a1, b1) = AsLAB();
        var (l2, a2, b2) = other.AsLAB();

        // As with CIE94, we scale up the ranges of L,a,b beforehand and scale
        // them down again afterwards.
        (l1, a1, b1) = (l1 * 100.0, a1 * 100.0, b1 * 100.0);
        (l2, a2, b2) = (l2 * 100.0, a2 * 100.0, b2 * 100.0);

        var cab1 = Math.Sqrt(Square(a1) + Square(b1));
        var cab2 = Math.Sqrt(Square(a2) + Square(b2));
        var cabmean = (cab1 + cab2) / 2.0;

        var g = 0.5 * (1.0 - Math.Sqrt(Math.Pow(cabmean, 7) / (Math.Pow(cabmean, 7) + Math.Pow(25, 7))));
        var ap1 = (1.0 + g) * a1;
        var ap2 = (1.0 + g) * a2;
        var cp1 = Math.Sqrt(Square(ap1) + Square(b1));
        var cp2 = Math.Sqrt(Square(ap2) + Square(b2));

        var hp1 = 0.0;
        if (b1.IsNot(ap1) || ap1.IsNotZero())
        {
            hp1 = Math.Atan2(b1, ap1);
            if (hp1 < 0)
            {
                hp1 += Math.PI * 2.0;
            }

            hp1 *= 180 / Math.PI;
        }

        var hp2 = 0.0;
        if (b2.IsNot(ap2) || ap2.IsNotZero())
        {
            hp2 = Math.Atan2(b2, ap2);
            if (hp2 < 0)
            {
                hp2 += Math.PI * 2.0;
            }

            hp2 *= 180 / Math.PI;
        }

        var deltaLp = l2 - l1;
        var deltaCp = cp2 - cp1;
        var dhp = 0.0;
        var cpProduct = cp1 * cp2;
        if (cpProduct != 0)
        {
            dhp = hp2 - hp1;
            if (dhp > 180)
            {
                dhp -= 360;
            }
            else if (dhp < -180)
            {
                dhp += 360;
            }
        }

        var deltaHp = 2.0 * Math.Sqrt(cpProduct) * Math.Sin(dhp / 2.0 * Math.PI / 180);

        var lpmean = (l1 + l2) / 2.0;
        var cpmean = (cp1 + cp2) / 2.0;
        var hpmean = hp1 + hp2;
        if (cpProduct != 0)
        {
            hpmean /= 2.0;
            if (Math.Abs(hp1 - hp2) > 180)
            {
                if (hp1 + hp2 < 360)
                {
                    hpmean += 180;
                }
                else
                {
                    hpmean -= 180;
                }
            }
        }

        var t = 1.0 - 0.17 * Math.Cos((hpmean - 30) * Math.PI / 180) + 0.24 * Math.Cos(2.0 * hpmean * Math.PI / 180.0) +
            0.32 * Math.Cos((3.0 * hpmean + 6.0) * Math.PI / 180) - 0.2 * Math.Cos((4.0 * hpmean - 63.0) * Math.PI / 180.0);
        var deltaTheta = 30 * Math.Exp(-Square((hpmean - 275) / 25));
        var rc = 2.0 * Math.Sqrt(Math.Pow(cpmean, 7) / (Math.Pow(cpmean, 7) + Math.Pow(25, 7)));
        var sl = 1.0 + (0.015 * Square(lpmean - 50)) / Math.Sqrt(20 + Square(lpmean - 50));
        var sc = 1.0 + 0.045 * cpmean;
        var sh = 1.0 + 0.015 * cpmean * t;
        var rt = -Math.Sin(2.0 * deltaTheta * Math.PI / 180.0) * rc;

        return Math.Sqrt(
            Square(deltaLp / (kl * sl)) + Square(deltaCp / (kc * sc)) + Square(deltaHp / (kh * sh)) +
            rt * (deltaCp / (kc * sc)) * (deltaHp / (kh * sh))) * 0.01;
    }

    /// <summary>
    ///     Blend between two colors at a particular distance [0.0-1.0] using L*A*B* color space
    /// </summary>
    public Colorful BlendLAB(Colorful c2, double t)
    {
        var (l1, a1, b1) = AsLAB();
        var (l2, a2, b2) = c2.AsLAB();
        return CreateLAB(
            l1 + t * (l2 - l1),
            a1 + t * (a2 - a1),
            b1 + t * (b2 - b1));
    }

    //--- L*U*V* ------------------------------------------------------------------------------------
    // http://en.wikipedia.org/wiki/CIELUV#XYZ_.E2.86.92_CIELUV_and_CIELUV_.E2.86.92_XYZ_conversions
    // For L*U*V*, we need to L*U*V*<->XYZ<->RGB and the first one is device dependent.

    /// <summary>
    ///     Converts from XYZ to L*U*V* color space
    /// </summary>
    public static (double l, double a, double b) ConvertXYZToLUV(double x, double y, double z)
    {
        // Use D65 white as reference point by default.
        // http://www.fredmiranda.com/forum/topic/1035332
        // http://en.wikipedia.org/wiki/Standard_illuminant
        return ConvertXYZToLUVWhiteRef(x, y, z, D65);
    }

    /// <summary>
    ///     Converts from XYZ to L*U*V* color space using white reference
    /// </summary>
    public static (double l, double u, double v) ConvertXYZToLUVWhiteRef(double x, double y, double z, double[] whiteRef)
    {
        double l;
        if (y / whiteRef[1] <= 6.0 / 29.0 * 6.0 / 29.0 * 6.0 / 29.0)
        {
            l = y / whiteRef[1] * (29.0 / 3.0 * 29.0 / 3.0 * 29.0 / 3.0) / 100.0;
        }
        else
        {
            l = 1.16f * Math.Cbrt(y / whiteRef[1]) - 0.16;
        }

        var (ubis, vbis) = XYZ_to_UV(x, y, z);
        var (un, vn) = XYZ_to_UV(whiteRef[0], whiteRef[1], whiteRef[2]);
        var u = 13.0 * l * (ubis - un);
        var v = 13.0 * l * (vbis - vn);
        return (l, u, v);
    }

    internal static (double u, double v) XYZ_to_UV(double x, double y, double z)
    {
        var denom = x + 15.0 * y + 3.0 * z;
        if (denom.IsZero())
            return (0, 0);

        return (4.0 * x / denom, 9.0 * y / denom);
    }

    /// <summary>
    ///     Converts from L*U*V* to XYZ color space
    /// </summary>
    public static (double x, double y, double z) ConvertLUVToXYZ(double l, double u, double v)
    {
        // D65 white (see above).
        return ConvertLUVToXYZWhiteRef(l, u, v, D65);
    }

    /// <summary>
    ///     Converts L*U*V* to XYZ color space using white reference
    /// </summary>
    public static (double x, double y, double z) ConvertLUVToXYZWhiteRef(double l, double u, double v, double[] whiteRef)
    {
        double x, y, z = 0;
        if (l <= 0.08)
        {
            y = whiteRef[1] * l * 100.0 * 3.0 / 29.0 * 3.0 / 29.0 * 3.0 / 29.0;
        }
        else
        {
            y = whiteRef[1] * Cube((l + 0.16) / 1.16);
        }

        var (un, vn) = XYZ_to_UV(whiteRef[0], whiteRef[1], whiteRef[2]);
        if (l != 0.0)
        {
            var ubis = u / (13.0 * l) + un;
            var vbis = v / (13.0 * l) + vn;
            x = y * 9.0 * ubis / (4.0 * vbis);
            z = y * (12.0 - 3.0 * ubis - 20.0 * vbis) / (4.0 * vbis);
        }
        else
        {
            (x, y) = (0.0, 0.0);
        }

        return (x, y, z);
    }

    /// <summary>
    ///     Return color as LUV color space
    /// </summary>
    public (double l, double u, double v) AsLUV()
    {
        var (x, y, z) = AsXYZ();
        return ConvertXYZToLUV(x, y, z);
    }

    /// <summary>
    ///     Return color as LUV color space using white reference
    /// </summary>
    public (double l, double u, double v) AsLUVWhiteRef(double[] whiteRef)
    {
        var (x, y, z) = AsXYZ();
        return ConvertXYZToLUVWhiteRef(x, y, z, whiteRef);
    }

    /// <summary>
    ///     Creates color using LUV color space
    /// </summary>
    /// <remarks>Some LUV values do not have corresponding RGB color</remarks>
    public static Colorful CreateLUV(double l, double u, double v)
    {
        var (x, y, z) = ConvertLUVToXYZ(l, u, v);
        return CreateXYZ(x, y, z);
    }

    /// <summary>
    ///     Creates LUV color space using white reference
    /// </summary>
    /// <remarks>Some LUV values do not have corresponding RGB color</remarks>
    public static Colorful CreateLUVWhiteRef(double l, double u, double v, double[] whiteRef)
    {
        var (x, y, z) = ConvertLUVToXYZWhiteRef(l, u, v, whiteRef);
        return CreateXYZ(x, y, z);
    }

    /// <summary>
    ///     Distance to other color using LUV color space
    /// </summary>
    public double DistanceLUV(Colorful other)
    {
        var (l1, u1, v1) = AsLUV();
        var (l2, u2, v2) = other.AsLUV();

        return Math.Sqrt(Square(l1 - l2) + Square(u1 - u2) + Square(v1 - v2));
    }

    /// <summary>
    ///     Blend between two colors at a particular distance [0.0-1.0] using LUV color space
    /// </summary>
    public Colorful BlendLUV(Colorful c2, double t)
    {
        var (l1, u1, v1) = AsLUV();
        var (l2, u2, v2) = c2.AsLUV();
        return CreateLUV(
            l1 + t * (l2 - l1),
            u1 + t * (u2 - u1),
            v1 + t * (v2 - v1));
    }

    //--- HCL -------------------------------------------------------------------------------
    // HCL is nothing else than L*a*b* in cylindrical coordinates!
    // (this was wrong on English wikipedia, I fixed it, let's hope the fix stays.)
    // But it is widely popular since it is a "correct HSV"
    // http://www.hunterlab.com/appnotes/an09_96a.pdf

    /// <summary>
    ///     Create color from HCL values
    /// </summary>
    public (double h, double c, double l) AsHCL() { return AsHCLWhiteRef(D65); }

    /// <summary>
    ///     Converts LAB to HCL color space
    /// </summary>
    public static (double h, double c, double l) ConvertLABToHCL(double l, double a, double b)
    {
        double h;
        // Oops, doubleing point workaround necessary if a ~= b and both are very small (i.e. almost zero).
        if (Math.Abs(b - a) > 1e-4 && Math.Abs(a) > 1e-4)
        {
            h = (57.29577951308232087721 * Math.Atan2(b, a) + 360.0 % 360.0); // Rad2Deg
        }
        else
        {
            h = 0.0;
        }

        var c = Math.Sqrt(Square(a) + Square(b));
        return (h, c, l);
    }

    /// <summary>
    ///     HCL color space from reference White
    /// </summary>
    public (double h, double c, double l) AsHCLWhiteRef(double[] whiteRef)
    {
        var (l, a, b) = AsLABWhiteRef(whiteRef);
        return ConvertLABToHCL(l, a, b);
    }

    /// <summary>
    ///     Create color using HCL color space
    /// </summary>
    public static Colorful CreateHCL(double h, double c, double l)
    {
        return CreateHCLWhiteRef(h, c, l, D65);
    }

    /// <summary>
    ///     Convert HCL to LAB color space
    /// </summary>
    public static (double L, double a, double b) ConvertHCLToLAB(double h, double c, double l)
    {
        var hRad = 0.01745329251994329576 * h; // Deg2Rad
        var a = c * Math.Cos(hRad);
        var b = c * Math.Sin(hRad);
        return (l, a, b);
    }

    /// <summary>
    ///     Create color from HCL color space and white reference
    /// </summary>
    public static Colorful CreateHCLWhiteRef(double h, double c, double l, double[] whiteRef)
    {
        var (lightness, a, b) = ConvertHCLToLAB(h, c, l);
        return CreateLABWhiteRef(lightness, a, b, whiteRef);
    }

    /// <summary>
    ///     Blend between two colors at a particular distance [0.0-1.0] using HCL color space
    /// </summary>
    public Colorful BlendHCL(Colorful other, double t)
    {
        var (h1, c1, l1) = AsHCL();
        var (h2, c2, l2) = other.AsHCL();

        // We know that h are both in [0..360]
        return CreateHCL(InterpAngle(h1, h2, t), c1 + t * (c2 - c1), l1 + t * (l2 - l1)).Clamped();
    }

    //--- LuvLch -------------------------------------------------------------------------------------

    /// <summary>
    ///     Return color as LUVLCH color space
    /// </summary>
    public (double l, double c, double h) AsLUVLCH() { return AsLUVLCHWhiteRef(D65); }

    public static (double l, double c, double h) ConvertLUVToLUVLCH(double l, double u, double v)
    {
        double h;
        // Oops, doubleing point workaround necessary if u ~= v and both are very small (i.e. almost zero).
        if (Math.Abs(v - u) > 1e-4 && Math.Abs(u) > 1e-4)
        {
            h = (57.29577951308232087721 * Math.Atan2(v, u) + 360.0 % 360.0); // Rad2Deg
        }
        else
        {
            h = 0.0;
        }

        var c = Math.Sqrt(Square(u) + Square(v));
        return (l, c, h);
    }

    /// <summary>
    ///     Return color as LUVLCH color space using white reference
    /// </summary>
    public (double l, double c, double h) AsLUVLCHWhiteRef(double[] whiteRef)
    {
        var (l, u, v) = AsLUVWhiteRef(whiteRef);
        return ConvertLUVToLUVLCH(l, u, v);
    }

    /// <summary>
    ///     Create color using LUVLCH color space
    /// </summary>
    public static Colorful CreateLUVLCH(double l, double c, double h)
    {
        return CreateLUVLCHWhiteRef(l, c, h, D65);
    }

    public static (double L, double u, double v) ConvertLUVLCHToLUV(double l, double c, double h)
    {
        var hRad = 0.01745329251994329576 * h; // Deg2Rad
        var u = c * Math.Cos(hRad);
        var v = c * Math.Sin(hRad);

        return (l, u, v);
    }

    /// <summary>
    ///     Create color using LUVLCH color space with white reference
    /// </summary>
    public static Colorful CreateLUVLCHWhiteRef(double l, double c, double h, double[] whiteRef)
    {
        var (lightness, u, v) = ConvertLUVLCHToLUV(l, c, h);
        return CreateLUVWhiteRef(lightness, u, v, whiteRef);
    }

    /// <summary>
    ///     Blend between two colors at a particular distance [0.0-1.0] using cylindrical CIELUV color space
    /// </summary>
    public Colorful BlendLUVLCH(Colorful col2, double t)
    {
        var (l1, c1, h1) = AsLUVLCH();
        var (l2, c2, h2) = col2.AsLUVLCH();

        // We know that h are both in [0..360]
        return CreateLUVLCH(l1 + t * (l2 - l1), c1 + t * (c2 - c1), InterpAngle(h1, h2, t));
    }

    public bool Equals(Colorful other)
    {
        return CloseEnough(R, other.R) &&
               CloseEnough(G, other.G) &&
               CloseEnough(B, other.B);

        bool CloseEnough(double a, double b) => Math.Abs(a - b) < Delta;
    }

    public override bool Equals(object? obj) => obj is Colorful other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(R, G, B);
    public static bool operator ==(Colorful left, Colorful right) { return left.Equals(right); }
    public static bool operator !=(Colorful left, Colorful right) { return !left.Equals(right); }

    public override string ToString() => $"({R},{G},{B}:{AsHex()})";
}
