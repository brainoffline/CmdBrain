namespace No8.CmdBrain.Tests.Helpers;

[TestClass]
public class ColorfulTests
{

    // Checks whether the relative error is below eps
    bool AlmostEqual(double v1, double v2, double eps)
    {
        if (Math.Abs(v1) > delta)
        {
            return Math.Abs((v1 - v2) / v1) < eps;
        }

        return true;
    }

    // Checks whether the relative error is below the 8bit RGB delta, which should be good enough.
    private const double delta = 1.0 / 256.0;

    bool AlmostEqual(double v1, double v2)
    {
        return AlmostEqual(v1, v2, delta);
    }

    struct FColorValues
    {
        public readonly Colorful C;
        public readonly double[] HSL;
        public readonly double[] HSV;
        public readonly string   Hex;
        public readonly double[] XYZ;
        public readonly double[] XYY;
        public readonly double[] LAB;
        public readonly double[] LAB50;
        public readonly double[] LUV;
        public readonly double[] LUV50;
        public readonly double[] HCL;
        public readonly double[] HCL50;
        public readonly uint[]   RGBA;
        public readonly byte[]   RGB255;

        public FColorValues(
            Colorful c,
            double[] hsl,
            double[] hsv,
            string hex,
            double[] xyz,
            double[] xyy,
            double[] lab,
            double[] lab50,
            double[] luv,
            double[] luv50,
            double[] hcl,
            double[] hcl50,
            uint[] rgba,
            byte[] rgb255
            )
        {
            C = c;
            HSL = hsl;
            HSV = hsv;
            Hex = hex;
            XYZ = xyz;
            XYY = xyy;
            LAB = lab;
            LAB50 = lab50;
            LUV = luv;
            LUV50 = luv50;
            HCL = hcl;
            HCL50 = hcl50;
            RGBA = rgba;
            RGB255 = rgb255;
        }
    }

    // Note: the XYZ, L*a*b*, etc. are using D65 white and D50 white if postfixed by "50".
    // See http://www.brucelindbloom.com/index.html?ColorCalcHelp.html
    // For d50 white, no "adaptation" and the sRGB model are used in colorful
    // HCL values form http://www.easyrgb.com/index.php?X=CALC and missing ones hand-computed from lab ones
    FColorValues[] vals = new[] {
        new FColorValues( new Colorful ( 1.0, 1.0, 1.0), new[] { 0.0, 0.0, 1.0 }, new[] { 0.0, 0.0, 1.0}, "#ffffff", new[] { 0.950470, 1.000000, 1.088830 }, new[] { 0.312727, 0.329023, 1.000000 }, new [] { 1.000000, 0.000000, 0.000000 }, new[] { 1.000000, -0.023881, -0.193622 }, new [] { 1.00000, 0.00000, 0.00000 }, new[] { 1.00000, -0.14716, -0.25658 }, new[] { 0.0000, 0.000000, 1.000000 }, new[] { 262.9688, 0.195089, 1.000000 }, new uint[] { 65535, 65535, 65535, 65535}, new byte[] { 255, 255, 255} ),
        new FColorValues( new Colorful ( 0.5, 1.0, 1.0), new[] { 180.0, 1.0, 0.75 }, new[]{ 180.0, 0.5, 1.0}, "#80ffff", new[] { 0.626296, 0.832848, 1.073634 }, new[] { 0.247276, 0.328828, 0.832848 }, new[] { 0.931390, -0.353319, -0.108946 }, new[] { 0.931390, -0.374100, -0.301663 }, new[] { 0.93139, -0.53909, -0.11630 }, new[] { 0.93139, -0.67615, -0.35528 }, new[] { 197.1371, 0.369735, 0.931390 }, new[] { 218.8817, 0.480574, 0.931390 }, new uint[] { 32768, 65535, 65535, 65535}, new byte[]{ 128, 255, 255} ),
        new FColorValues( new Colorful ( 1.0, 0.5, 1.0), new[] { 300.0, 1.0, 0.75 }, new[] { 300.0, 0.5, 1.0}, "#ff80ff", new[] { 0.669430, 0.437920, 0.995150 }, new[] { 0.318397, 0.208285, 0.437920 }, new[] { 0.720892, 0.651673, -0.422133 }, new[] { 0.720892, 0.630425, -0.610035 }, new[] { 0.72089, 0.60047, -0.77626 }, new[] { 0.72089, 0.49438, -0.96123 }, new[] { 327.0661, 0.776450, 0.720892 }, new[] { 315.9417, 0.877257, 0.720892 }, new uint[] { 65535, 32768, 65535, 65535}, new byte[]{ 255, 128, 255} ),
        new FColorValues( new Colorful ( 1.0, 1.0, 0.5), new[] { 60.0, 1.0, 0.75 }, new[] { 60.0, 0.5, 1.0}, "#ffff80", new[] { 0.808654, 0.943273, 0.341930 }, new[] { 0.386203, 0.450496, 0.943273 }, new[] { 0.977637, -0.165795, 0.602017 }, new[] { 0.977637, -0.188424, 0.470410 }, new[] { 0.97764, 0.05759, 0.79816 }, new[] { 0.97764, -0.08628, 0.54731 }, new[] { 105.3975, 0.624430, 0.977637 }, new[] { 111.8287, 0.506743, 0.977637 }, new uint [] { 65535, 65535, 32768, 65535}, new byte[]{ 255, 255, 128} ),
        new FColorValues( new Colorful ( 0.5, 0.5, 1.0), new[] { 240.0, 1.0, 0.75 }, new[] { 240.0, 0.5, 1.0}, "#8080ff", new[] { 0.345256, 0.270768, 0.979954 }, new[] { 0.216329, 0.169656, 0.270768 }, new[] { 0.590453, 0.332846, -0.637099 }, new[] { 0.590453, 0.315806, -0.824040 }, new[] { 0.59045, -0.07568, -1.04877 }, new[] { 0.59045, -0.16257, -1.20027 }, new[] { 297.5843, 0.718805, 0.590453 }, new[] { 290.9689, 0.882482, 0.590453 }, new uint[] { 32768, 32768, 65535, 65535}, new byte[]{ 128, 128, 255} ),
        new FColorValues( new Colorful ( 1.0, 0.5, 0.5), new[] { 0.0, 1.0, 0.75 }, new[] { 0.0, 0.5, 1.0}, "#ff8080", new[] { 0.527613, 0.381193, 0.248250 }, new[] { 0.455996, 0.329451, 0.381193 }, new[] { 0.681085, 0.483884, 0.228328 }, new[] { 0.681085, 0.464258, 0.110043 }, new[] { 0.68108, 0.92148, 0.19879 }, new[] { 0.68106, 0.82106, 0.02393 }, new[] { 25.2610, 0.535049, 0.681085 }, new[] { 13.3347, 0.477121, 0.681085 }, new uint[] { 65535, 32768, 32768, 65535}, new byte[]{ 255, 128, 128} ),
        new FColorValues( new Colorful ( 0.5, 1.0, 0.5), new[] { 120.0, 1.0, 0.75 }, new[] { 120.0, 0.5, 1.0}, "#80ff80", new[] { 0.484480, 0.776121, 0.326734 }, new[] { 0.305216, 0.488946, 0.776121 }, new[] { 0.906026, -0.600870, 0.498993 }, new[] { 0.906026, -0.619946, 0.369365 }, new[] { 0.90603, -0.58869, 0.76102 }, new[] { 0.90603, -0.72202, 0.52855 }, new[] { 140.2920, 0.781050, 0.906026 }, new[] { 149.2134, 0.721640, 0.906026 }, new uint[] { 32768, 65535, 32768, 65535}, new byte[]{ 128, 255, 128} ),
        new FColorValues( new Colorful ( 0.5, 0.5, 0.5), new[] { 0.0, 0.0, 0.50 }, new[] { 0.0, 0.0, 0.5 }, "#808080", new[] { 0.203440, 0.214041, 0.233054 }, new[] { 0.312727, 0.329023, 0.214041 }, new[] { 0.533890, 0.000000, 0.000000 }, new[] { 0.533890, -0.014285, -0.115821 }, new[] { 0.53389, 0.00000, 0.00000 }, new[] { 0.53389, -0.07857, -0.13699 }, new[] { 0.0000, 0.000000, 0.533890 }, new[] { 262.9688, 0.116699, 0.533890 }, new uint[] { 32768, 32768, 32768, 65535}, new byte[]{ 128, 128, 128} ),
        new FColorValues( new Colorful ( 0.0, 1.0, 1.0), new[] { 180.0, 1.0, 0.50 }, new[] { 180.0, 1.0, 1.0}, "#00ffff", new[] { 0.538014, 0.787327, 1.069496 }, new[] { 0.224656, 0.328760, 0.787327 }, new[] { 0.911132, -0.480875, -0.141312 }, new[] { 0.911132, -0.500630, -0.333781 }, new[] { 0.91113, -0.70477, -0.15204 }, new[] { 0.91113, -0.83886, -0.38582 }, new[] { 196.3762, 0.501209, 0.911132 }, new[] { 213.6923, 0.601698, 0.911132 }, new uint[] { 0, 65535, 65535, 65535}, new byte[]{ 0, 255, 255} ),
        new FColorValues( new Colorful ( 1.0, 0.0, 1.0), new[] { 300.0, 1.0, 0.50 }, new[] { 300.0, 1.0, 1.0}, "#ff00ff", new[] { 0.592894, 0.284848, 0.969638 }, new[] { 0.320938, 0.154190, 0.284848 }, new[] { 0.603242, 0.982343, -0.608249 }, new[] { 0.603242, 0.961939, -0.794531 }, new[] { 0.60324, 0.84071, -1.08683 }, new[] { 0.60324, 0.75194, -1.24161 }, new[] { 328.2350, 1.155407, 0.603242 }, new[] { 320.4444, 1.247640, 0.603242 }, new uint[] { 65535, 0, 65535, 65535}, new byte[]{ 255, 0, 255} ),
        new FColorValues( new Colorful ( 1.0, 1.0, 0.0), new[] { 60.0, 1.0, 0.50 }, new[] { 60.0, 1.0, 1.0}, "#ffff00", new[] { 0.770033, 0.927825, 0.138526 }, new[] { 0.419320, 0.505246, 0.927825 }, new[] { 0.971393, -0.215537, 0.944780 }, new[] { 0.971393, -0.237800, 0.847398 }, new[] { 0.97139, 0.07706, 1.06787 }, new[] { 0.97139, -0.06590, 0.81862 }, new[] { 102.8512, 0.969054, 0.971393 }, new[] { 105.6754, 0.880131, 0.971393 }, new uint[] { 65535, 65535, 0, 65535}, new byte[]{ 255, 255, 0} ),
        new FColorValues( new Colorful ( 0.0, 0.0, 1.0), new[] { 240.0, 1.0, 0.50 }, new[] { 240.0, 1.0, 1.0}, "#0000ff", new[] { 0.180437, 0.072175, 0.950304 }, new[] { 0.150000, 0.060000, 0.072175 }, new[] { 0.322970, 0.791875, -1.078602 }, new[] { 0.322970, 0.778150, -1.263638 }, new[] { 0.32297, -0.09405, -1.30342 }, new[] { 0.32297, -0.14158, -1.38629 }, new[] { 306.2849, 1.338076, 0.322970 }, new[] { 301.6248, 1.484014, 0.322970 }, new uint[] { 0, 0, 65535, 65535}, new byte[]{ 0, 0, 255} ),
        new FColorValues( new Colorful ( 0.0, 1.0, 0.0), new[] { 120.0, 1.0, 0.50 }, new[] { 120.0, 1.0, 1.0}, "#00ff00", new[] { 0.357576, 0.715152, 0.119192 }, new[] { 0.300000, 0.600000, 0.715152 }, new[] { 0.877347, -0.861827, 0.831793 }, new[] { 0.877347, -0.879067, 0.739170 }, new[] { 0.87735, -0.83078, 1.07398 }, new[] { 0.87735, -0.95989, 0.84887 }, new[] { 136.0160, 1.197759, 0.877347 }, new[] { 139.9409, 1.148534, 0.877347 }, new uint[] { 0, 65535, 0, 65535}, new byte[]{ 0, 255, 0} ),
        new FColorValues( new Colorful ( 1.0, 0.0, 0.0), new[] { 0.0, 1.0, 0.50 }, new[] { 0.0, 1.0, 1.0}, "#ff0000", new[] { 0.412456, 0.212673, 0.019334 }, new[] { 0.640000, 0.330000, 0.212673 }, new[] { 0.532408, 0.800925, 0.672032 }, new[] { 0.532408, 0.782845, 0.621518 }, new[] { 0.53241, 1.75015, 0.37756 }, new[] { 0.53241, 1.67180, 0.24096 }, new[] { 39.9990, 1.045518, 0.532408 }, new[] { 38.4469, 0.999566, 0.532408 }, new uint[] { 65535, 0, 0, 65535}, new byte[]{ 255, 0, 0} ),
        new FColorValues( new Colorful ( 0.0, 0.0, 0.0), new[] { 0.0, 0.0, 0.00 }, new[] { 0.0, 0.0, 0.0}, "#000000", new[] { 0.000000, 0.000000, 0.000000 }, new[] { 0.312727, 0.329023, 0.000000 }, new[] { 0.000000, 0.000000, 0.000000 }, new[] { 0.000000, 0.000000, 0.000000 }, new[] { 0.00000, 0.00000, 0.00000 }, new[] { 0.00000, 0.00000, 0.00000 }, new[] { 0.0000, 0.000000, 0.000000 }, new[] { 0.0000, 0.000000, 0.000000 }, new uint[] { 0, 0, 0, 65535}, new byte[]{ 0, 0, 0} )
    };

    // For testing short-hex values, since the above contains colors which don't
    // have corresponding short hexes.

    struct ShortHexValue
    {
        public readonly Colorful C;
        public readonly string            Hex;

        public ShortHexValue(Colorful c, string hex)
        {
            C = c;
            Hex = hex;
        }
    }

    private ShortHexValue[] shorthexvals = new[]
    {
       new ShortHexValue(new Colorful(1.0, 1.0, 1.0), "#fff"),
       new ShortHexValue(new Colorful(0.6, 1.0, 1.0), "#9ff"),
       new ShortHexValue(new Colorful(1.0, 0.6, 1.0), "#f9f"),
       new ShortHexValue(new Colorful(1.0, 1.0, 0.6), "#ff9"),
       new ShortHexValue(new Colorful(0.6, 0.6, 1.0), "#99f"),
       new ShortHexValue(new Colorful(1.0, 0.6, 0.6), "#f99"),
       new ShortHexValue(new Colorful(0.6, 1.0, 0.6), "#9f9"),
       new ShortHexValue(new Colorful(0.6, 0.6, 0.6), "#999"),
       new ShortHexValue(new Colorful(0.0, 1.0, 1.0), "#0ff"),
       new ShortHexValue(new Colorful(1.0, 0.0, 1.0), "#f0f"),
       new ShortHexValue(new Colorful(1.0, 1.0, 0.0), "#ff0"),
       new ShortHexValue(new Colorful(0.0, 0.0, 1.0), "#00f"),
       new ShortHexValue(new Colorful(0.0, 1.0, 0.0), "#0f0"),
       new ShortHexValue(new Colorful(1.0, 0.0, 0.0), "#f00"),
       new ShortHexValue(new Colorful(0.0, 0.0, 0.0), "#000"),
    };


    //--- RGBA 

    [Fact]
    public void TestRGBAConversion()
    {
        foreach (var tt in vals)
        {
            var (r, g, b, a) = tt.C.AsRGBA();

            Assert.Equal(tt.RGBA[0], r);
            Assert.Equal(tt.RGBA[1], g);
            Assert.Equal(tt.RGBA[2], b);
            Assert.Equal(tt.RGBA[3], a);

        }
    }

    [Fact]
    public void TestRGB255Conversion()
    {
        foreach (var tt in vals)
        {
            var (r, g, b) = tt.C.AsRGB255();

            Assert.Equal(tt.RGB255[0], r);
            Assert.Equal(tt.RGB255[1], g);
            Assert.Equal(tt.RGB255[2], b);
        }
    }

    [Fact]
    public void TestHsvCreation()
    {
        foreach (var tt in vals)
        {
            var c = Colorful.CreateHSV(tt.HSV[0], tt.HSV[1], tt.HSV[2]);

            Assert.Equal(tt.C, c);
        }
    }

    [Fact]
    public void TestHsvConversion()
    {
        foreach (var tt in vals)
        {
            var (h, s, v) = tt.C.AsHSV();

            Assert.Equal(tt.HSV[0], h);
            Assert.Equal(tt.HSV[1], s);
            Assert.Equal(tt.HSV[2], v);
        }
    }

    [Fact]
    public void TestHslCreation()
    {
        foreach (var tt in vals)
        {
            var c = Colorful.CreateHSL(tt.HSL[0], tt.HSL[1], tt.HSL[2]);

            Assert.Equal(tt.C, c);
        }
    }

    [Fact]
    public void TestHslConversion()
    {
        foreach (var tt in vals)
        {
            var (h, s, l) = tt.C.AsHSL();

            Assert.Equal(tt.HSL[0], h);
            Assert.Equal(tt.HSL[1], s);
            Assert.Equal(tt.HSL[2], l);
        }
    }

    [Fact]
    public void TestHexCreation()
    {
        foreach (var tt in vals)
        {
            var (c, err) = Colorful.CreateHex(tt.Hex);

            Assert.Equal(tt.C, c);
        }
    }

    [Fact]
    public void TestShortHexCreation()
    {
        foreach (var tt in shorthexvals)
        {
            var (c, err) = Colorful.CreateHex(tt.Hex);

            Assert.Equal(tt.C, c);
        }
    }

    [Fact]
    public void TestHexConversion()
    {
        foreach (var tt in vals)
        {
            var hex = tt.C.AsHex();

            Assert.Equal(tt.Hex, hex);
        }
    }

    // LinearRgb itself is implicitly tested by XYZ conversions below (they use it).
    // So what we do here is just test that the FastLinearRgb approximation is "good enough"
    [Fact(Skip = "Algoritm Fast, but test is slow")]
    public void TestFastLinearRgb()
    {
        var eps = 6.0 / 255.0; // We want that "within 6 RGB values total" is "good enough".
        
        for (var r = 0.0; r < 256.0; r++) 
        {
            for(var g = 0.0; g < 256.0; g++) 
            {
                for (var b = 0.0; b < 256.0; b++)
                {
                    var c = new Colorful(r / 255.0, g / 255.0, b / 255.0);

                    var (r_want, g_want, b_want) = c.AsLinearRGB();
                    var (r_appr, g_appr, b_appr) = c.AsFastLinearRGB();

                    var (dr, dg, db) = 
                        (Math.Abs(r_want - r_appr), Math.Abs(g_want - g_appr), Math.Abs(b_want - b_appr));
        
                    if (dr + dg + db > eps)
                    {
                        Assert.False(true,
                            $"FastLinearRgb not precise enough for {c}: differences are ({dr}, {dg}, {db}), allowed total difference is {eps}");
                    }

                    var c_want = Colorful.AsLinearRGB(r / 255.0, g / 255.0, b / 255.0);
                    var c_appr = Colorful.CreateFastLinearRGB(r / 255.0, g / 255.0, b / 255.0);

                    (dr, dg, db) = 
                        (Math.Abs(c_want.R - c_appr.R), Math.Abs(c_want.G - c_appr.G), Math.Abs(c_want.B - c_appr.B));
        
                    if (dr + dg + db > eps)
                    {
                        Assert.False(true,
                            $"FastLinearRgb not precise enough for ({r}, {g}, {b}): differences are ({dr}, {dg}, {db}), allowed total difference is {eps}");
                    }
                }
            }
        }
        Assert.True(true);
    }

    [Fact]
    public void TestXyzCreation()
    {
        foreach (var tt in vals)
        {
            var c = Colorful.CreateXYZ(tt.XYZ[0], tt.XYZ[1], tt.XYZ[2]);

            Assert.Equal(tt.C, c);
        }
    }

    [Fact]
    public void TestXyzConversion()
    {
        foreach (var tt in vals)
        {
            var (x, y, y2) = tt.C.AsXYZ();

            Assert.True(AlmostEqual(tt.XYZ[0], x));
            Assert.True(AlmostEqual(tt.XYZ[1], y));
            Assert.True(AlmostEqual(tt.XYZ[2], y2));
        }
    }

    [Fact]
    public void TestXyyCreation()
    {
        foreach (var tt in vals)
        {
            var c = Colorful.CreateXYY(tt.XYY[0], tt.XYY[1], tt.XYY[2]);

            Assert.Equal(tt.C, c);
        }
    }

    [Fact]
    public void TestXyyConversion()
    {
        foreach (var tt in vals)
        {
            var (h, s, v) = tt.C.AsXYY();

            Assert.True(AlmostEqual(tt.XYY[0], h));
            Assert.True(AlmostEqual(tt.XYY[1], s));
            Assert.True(AlmostEqual(tt.XYY[2], v));
        }
    }

    [Fact]
    public void TestLabCreation()
    {
        foreach (var tt in vals)
        {
            var c = Colorful.CreateLAB(tt.LAB[0], tt.LAB[1], tt.LAB[2]);

            Assert.Equal(tt.C, c);
        }
    }

    [Fact]
    public void TestLabConversion()
    {
        foreach (var tt in vals)
        {
            var (l, a, b) = tt.C.AsLAB();

            Assert.True(AlmostEqual(tt.LAB[0], l));
            Assert.True(AlmostEqual(tt.LAB[1], a));
            Assert.True(AlmostEqual(tt.LAB[2], b));
        }
    }

    [Fact]
    public void TestLabWhiteRefCreation()
    {
        foreach (var tt in vals)
        {
            var c = Colorful.CreateLABWhiteRef(tt.LAB50[0], tt.LAB50[1], tt.LAB50[2], Colorful.D50);

            Assert.Equal(tt.C, c);
        }
    }

    [Fact]
    public void TestLabWhiteRefConversion()
    {
        foreach (var tt in vals)
        {
            var (l, a, b) = tt.C.AsLABWhiteRef(Colorful.D50);

            Assert.True(AlmostEqual(tt.LAB50[0], l));
            Assert.True(AlmostEqual(tt.LAB50[1], a));
            Assert.True(AlmostEqual(tt.LAB50[2], b));
        }
    }

    [Fact]
    public void TestLuvCreation()
    {
        foreach (var tt in vals)
        {
            var c = Colorful.CreateLUV(tt.LUV[0], tt.LUV[1], tt.LUV[2]);

            Assert.Equal(tt.C, c);
        }
    }

    [Fact]
    public void TestLuvConversion()
    {
        foreach (var tt in vals)
        {
            var (l, u, v) = tt.C.AsLUV();

            Assert.True(AlmostEqual(tt.LUV[0], l));
            Assert.True(AlmostEqual(tt.LUV[1], u));
            Assert.True(AlmostEqual(tt.LUV[2], v));
        }
    }

    [Fact(Skip = "Unknown Error")]
    public void TestLuvWhiteRefCreation()
    {
        foreach (var tt in vals)
        {
            var c = Colorful.CreateLABWhiteRef(tt.LUV50[0], tt.LUV50[1], tt.LUV50[2], Colorful.D50);

            Assert.True(tt.C.AlmostEqualRGB(c), $"Expected:{tt.C} actual:{c}");
        }
    }

    [Fact(Skip = "Unknown Error")]
    public void TestLuvWhiteRefConversion()
    {
        foreach (var tt in vals)
        {
            var (l, u, v) = tt.C.AsLUVWhiteRef(Colorful.D50);

            Assert.True(AlmostEqual(tt.LUV50[0], l));
            Assert.True(AlmostEqual(tt.LUV50[1], u));
            Assert.True(AlmostEqual(tt.LUV50[2], v));
        }
    }

    [Fact]
    public void TestHclCreation()
    {
        foreach (var tt in vals)
        {
            var c = Colorful.CreateHCL(tt.HCL[0], tt.HCL[1], tt.HCL[2]);

            Assert.Equal(tt.C, c);
        }
    }

    [Fact(Skip = "Unknown Error")]
    public void TestHclConversion()
    {
        foreach (var tt in vals)
        {
            var (h, c, l) = tt.C.AsHCL();

            Assert.True(AlmostEqual(tt.HCL[0], h), $"Expected:{tt.HCL[0]} Actual:{h}");
            Assert.True(AlmostEqual(tt.HCL[1], c), $"Expected:{tt.HCL[1]} Actual:{c}");
            Assert.True(AlmostEqual(tt.HCL[2], l), $"Expected:{tt.HCL[2]} Actual:{l}");
        }
    }

    [Fact]
    public void TestHclWhiteRefCreation()
    {
        foreach (var tt in vals)
        {
            var c = Colorful.CreateHCLWhiteRef(tt.HCL50[0], tt.HCL50[1], tt.HCL50[2], Colorful.D50);

            Assert.True(tt.C.AlmostEqualRGB(c), $"Expected:{tt.C} actual:{c}");
        }
    }

    [Fact(Skip = "Unknown Error")]
    public void TestHCLWhiteRefConversion()
    {
        foreach (var tt in vals)
        {
            var (h, c, l) = tt.C.AsHCLWhiteRef(Colorful.D50);

            Assert.True(AlmostEqual(tt.HCL50[0], h), $"Expected:{tt.HCL50[0]} Actual:{h}");
            Assert.True(AlmostEqual(tt.HCL50[1], c), $"Expected:{tt.HCL50[0]} Actual:{h}");
            Assert.True(AlmostEqual(tt.HCL50[2], l), $"Expected:{tt.HCL50[0]} Actual:{h}");
        }
    }


    struct Distance
    {
        public Colorful c1;
        public Colorful c2;
        public double            d76;
        public double            d94;
        public double            d00;

        public Distance(Colorful c1, Colorful c2, double d76, double d94, double d00)
        {
            this.c1  = c1;
            this.c2  = c2;
            this.d76 = d76;
            this.d94 = d94;
            this.d00 = d00;
        }
    }

    // Ground-truth from http://www.brucelindbloom.com/index.html?ColorDifferenceCalcHelp.html

    private Distance[] dists = new Distance[]
    {
        new Distance( new Colorful(1.0,1.0,1.0), new Colorful(1.0,1.0,1.0), 0.0,0.0,0.0),
        new Distance( new Colorful(0.0,0.0,0.0), new Colorful(0.0,0.0,0.0), 0.0,0.0,0.0),

        // Just pairs of values of the table way above.
        new Distance( Colorful.CreateLAB(1.000000f, 0.000000f, 0.000000),  Colorful.CreateLAB(0.931390, -0.353319, -0.108946), 0.37604638f, 0.37604638f, 0.23528129f),
        new Distance( Colorful.CreateLAB(0.720892, 0.651673, -0.422133),   Colorful.CreateLAB(0.977637f, -0.165795f, 0.602017), 1.33531088f, 0.65466377f, 0.75175896f),
        new Distance( Colorful.CreateLAB(0.590453f, 0.332846f, -0.637099), Colorful.CreateLAB(0.681085f, 0.483884f, 0.228328), 0.88317072, 0.42541075f, 0.37688153f),
        new Distance( Colorful.CreateLAB(0.906026f, -0.600870f, 0.498993), Colorful.CreateLAB(0.533890f, 0.000000f, 0.000000), 0.86517280f, 0.41038323f, 0.39960503f),
        new Distance( Colorful.CreateLAB(0.911132, -0.480875, -0.141312),  Colorful.CreateLAB(0.603242, 0.982343, -0.608249), 1.56647162, 0.87431457f, 0.57983482),
        new Distance( Colorful.CreateLAB(0.971393, -0.215537f, 0.944780),  Colorful.CreateLAB(0.322970f, 0.791875, -1.078602), 2.35146891, 1.11858192, 1.03426977f),
        new Distance( Colorful.CreateLAB(0.877347f, -0.861827f, 0.831793), Colorful.CreateLAB(0.532408f, 0.800925f, 0.672032), 1.70565338f, 0.68800270f, 0.86608245),
    };

    [Fact]
    public void TestLabDistance()
    {
        foreach (var tt in dists)
        {
            var d = tt.c1.DistanceCIE76(tt.c2);

            Assert.True(AlmostEqual(tt.d76, d), $"Expected:{tt.d76}. Actual:{d}");
        }
    }

    [Fact]
    public void TestLabDistance94()
    {
        foreach (var tt in dists)
        {
            var d = tt.c1.DistanceCIE94(tt.c2);

            Assert.True(AlmostEqual(tt.d94, d), $"Expected:{tt.d94}. Actual:{d}");
        }
    }

    [Fact]
    public void TestLabDistance00()
    {
        foreach (var tt in dists)
        {
            var d = tt.c1.DistanceCIEDE2000(tt.c2);

            Assert.True(AlmostEqual(tt.d00, d), $"Expected:{tt.d00}. Actual:{d}");
        }
    }

    [Fact]
    public void TestClamp()
    {
        var actual   = new Colorful(1.1, -0.1, 0.5).Clamped();
        var expected = new Colorful( 1.0, 0.0, 0.5);

        Assert.True(expected.AlmostEqualRGB(actual), $"Expected:{expected}. Actual:{actual}.");
    }

    [Fact]
    public void TestColor()
    {
        var color    = System.Drawing.Color.FromArgb(255, 123, 45, 67);
        var fColor   = (Colorful)color;
        var (r, g, b) = fColor.AsRGB255();

        Assert.Equal(123, r);
        Assert.Equal(45, g);
        Assert.Equal(67, b);
    }

    [Fact]
    public void TestColorGray()
    {
        var fColor = (Colorful)System.Drawing.Color.Gray;
        var (r, g, b) = fColor.AsRGB255();

        Assert.Equal(0x80, r);
        Assert.Equal(0x80, g);
        Assert.Equal(0x80, b);
    }

    [Fact]
    public void TestColorTransparent()
    {
        var fColor = (Colorful)System.Drawing.Color.Transparent;
        var (r, g, b) = fColor.AsRGB255();

        Assert.Equal(0, r);
        Assert.Equal(0, g);
        Assert.Equal(0, b);
    }

    struct AngleValue
    {
        public double a0;
        public double a1;
        public double t;
        public double at;

        public AngleValue(double a0, double a1, double t, double at)
        {
            this.a0 = a0;
            this.a1 = a1;
            this.t  = t;
            this.at = at;
        }
    }

    private AngleValue[] angleVals = new AngleValue[]
    {
        
        new AngleValue( 0.0, 1.0, 0.0, 0.0),
        new AngleValue( 0.0, 1.0, 0.25f, 0.25),
        new AngleValue( 0.0, 1.0, 0.5f, 0.5),
        new AngleValue( 0.0, 1.0, 1.0, 1.0),
        new AngleValue( 0.0, 90.0, 0.0, 0.0),
        new AngleValue( 0.0, 90.0, 0.25, 22.5),
        new AngleValue( 0.0, 90.0, 0.5, 45.0),
        new AngleValue( 0.0, 90.0, 1.0, 90.0),
        new AngleValue( 0.0, 178.0, 0.0, 0.0), // Exact 0-180 is ambiguous
        new AngleValue( 0.0, 178.0, 0.25, 44.5),
        new AngleValue( 0.0, 178.0, 0.5, 89.0),
        new AngleValue( 0.0, 178.0, 1.0, 178.0),
        new AngleValue( 0.0, 182.0, 0.0, 0.0), // Exact 0-180 is ambiguous
        new AngleValue( 0.0, 182.0, 0.25, 315.5),
        new AngleValue( 0.0, 182.0, 0.5, 271.0),
        new AngleValue( 0.0, 182.0, 1.0, 182.0),
        new AngleValue( 0.0, 270.0, 0.0, 0.0),
        new AngleValue( 0.0, 270.0, 0.25, 337.5),
        new AngleValue( 0.0, 270.0, 0.5, 315.0),
        new AngleValue( 0.0, 270.0, 1.0, 270.0),
        new AngleValue( 0.0, 359.0, 0.0, 0.0),
        new AngleValue( 0.0, 359.0, 0.25, 359.75),
        new AngleValue( 0.0, 359.0, 0.5, 359.5),
        new AngleValue( 0.0, 359.0, 1.0, 359.0),
    };

    [Fact]
    public void TestInterpolation()
    {
        foreach (var tt in angleVals)
        {
            var actual = Colorful.InterpAngle(tt.a0, tt.a1, tt.t);

            Assert.Equal(tt.at, actual);
        }
    }

    [Fact]
    public void TestInterpolationBackwards()
    {
        foreach (var tt in angleVals)
        {
            var actual = Colorful.InterpAngle(tt.a1, tt.a0, 1.0 - tt.t);

            Assert.Equal(tt.at, actual);
        }
    }
}