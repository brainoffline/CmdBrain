using No8.CmdBrain;
using System.Text;

await new Prog(
    new CellBufferModel(), 
    StartupOptions.withAltScreen | StartupOptions.withMouseCellMotion)
    .Run();


public class frameMsg : Msg { }
public class CellBufferModel : Model
{
    const int fps = 60;
    const float frequency = 7.5f;
    const float damping = 0.15f;

    CellBuffer cells = new();
    Spring spring = new(fps, frequency, damping);
    float targetX, targetY;
    float x, y;
    float xVelocity, yVelocity;

    public Cmd? Init()
    {
        targetX = (Console.WindowWidth / 2f);
        targetY = (Console.WindowHeight / 2f);
        cells.Init(Console.WindowWidth, Console.WindowHeight);
        return animate();
    }

    public (Model, Cmd?) Update(Msg msg)
    {
        switch (msg)
        {
            case KeyMsg _:
                return (this, () => Task.FromResult<Msg>(new QuitMsg()));

            case WindowSizeMsg wsMsg:
                if (!cells.ready())
                {
                    targetX = (wsMsg.Width / 2f);
                    targetY = (wsMsg.Height / 2f);
                }
                cells.Init(wsMsg.Width, wsMsg.Height);
                return (this, null);

            //case MouseMsg mMsg:
            //    if (!cells.ready())
            //    {
            //        return (this, null);
            //    }
            //    (targetX, targetY) = (mMsg.X, mMsg.Y);

            //    return (this, null);

            case frameMsg _:
                if (!cells.ready())
                {
                    return (this, null);

                }

                cells.wipe();

                (x, xVelocity) = spring.Update(x, xVelocity, targetX);
                (y, yVelocity) = spring.Update(y, yVelocity, targetY);

                cells.DrawEllipse(x, y, 16, 8);

                return (this, animate());
        }
        return (this, null);
    }

    public string View()
    {
        if (cells.ready())
        {
            return cells.ToString();
        }
        return Figlet.Render("Allo, World!");
    }

    public Cmd animate()
    {
        return async () =>
        {
            await Task.Delay(1000 / fps);
            return new frameMsg();
        };
    }
}

public class CellBuffer
{
    private string[]? cells;
    private int stride;

    public void DrawEllipse(
        float xc,
        float yc,
        float rx,
        float ry)
    {
        const string c = "*";

        float dx, dy, d1, d2;
        float x = 0;
        float y = ry;

        d1 = ry * ry - rx * rx * ry + 0.25f * rx * rx;
        dx = 2 * ry * ry * x;
        dy = 2 * rx * rx * y;

        while (dx < dy)
        {
            set(c, (int)(x + xc), (int)(y + yc));
            set(c, (int)(-x + xc), (int)(y + yc));
            set(c, (int)(x + xc), (int)(-y + yc));
            set(c, (int)(-x + xc), (int)(-y + yc));

            if (d1 < 0)
            {
                x++;

                dx = dx + (2 * ry * ry);
                d1 = d1 + dx + (ry * ry);
            }
            else
            {
                x++;
                y--;

                dx = dx + (2 * ry * ry);
                dy = dy - (2 * rx * rx);
                d1 = d1 + dx - dy + (ry * ry);
            }
        }

        d2 = ((ry * ry) * ((x + 0.5f) * (x + 0.5f))) + ((rx * rx) * ((y - 1) * (y - 1))) - (rx * rx * ry * ry);

        while (y >= 0)
        {
            set(c, (int)(x + xc), (int)(y + yc));
            set(c, (int)(-x + xc), (int)(y + yc));
            set(c, (int)(x + xc), (int)(-y + yc));
            set(c, (int)(-x + xc), (int)(-y + yc));

            if (d2 > 0)
            {
                y--;
                dy = dy - (2 * rx * rx);
                d2 = d2 + (rx * rx) - dy;
            }
            else
            {
                y--;
                x++;
                dx = dx + (2 * ry * ry);
                dy = dy - (2 * rx * rx);
                d2 = d2 + dx - dy + (rx * rx);
            }
        }
    }

    public void Init(int w, int h)
    {
        if (w == 0)
        {
            return;
        }
        stride = w;

        cells = new string[w * h];
        wipe();
    }

    public void set(string v, int x, int y)
    {
        var i = y * stride + x;

        if (i > cells.Length - 1 || x < 0 || y < 0 || x >= width() || y >= height())
        {
            return;

        }
        cells[i] = v;
    }

    public void clear(int x, int y)
    {
        set(" ", x, y);
    }

    public void wipe()
    {
        for (int i = 0; i < cells.Length; i++)
            cells[i] = " ";
    }

    public int width()
    {
        return stride;
    }

    public int height()
    {
        var h = cells.Length / stride;
        if (cells.Length % stride != 0)
        {
            h++;
        }
        return h;
    }

    public bool ready()
    {
        return cells?.Length > 0;
    }

    public override string ToString()
    {
        var b = new StringBuilder();

        for (int i = 0; i < cells?.Length; i++)
        {
            if (i > 0 && i % stride == 0 && i < cells.Length - 1)
            {
                b.Append('\n');
            }
            b.Append(cells[i]);

        }
        return b.ToString();
    }
}

public class Spring
{
    private float posPosCoef, posVelCoef;
    private float velPosCoef, velVelCoef;

    public Spring(int fps, float angularFrequency, float dampingRatio)
    {
        var deltaTime = 1000f / fps;

        // Keep values in a legal range.
        angularFrequency = MathF.Max(0.0f, angularFrequency);
        dampingRatio = MathF.Max(0.0f, dampingRatio);

        // If there is no angular frequency, the spring will not move and we can
        // return identity.
        if (angularFrequency < float.Epsilon)
        {
            posPosCoef = 1.0f;
            posVelCoef = 0.0f;
            velPosCoef = 0.0f;
            velVelCoef = 1.0f;
            return;
        }

        if (dampingRatio > 1.0f + float.Epsilon)
        {
            // Over-damped.
            var za = -angularFrequency * dampingRatio;
            var zb = angularFrequency * MathF.Sqrt(dampingRatio * dampingRatio - 1.0f);
            var z1 = za - zb;
            var z2 = za + zb;
            var e1 = MathF.Exp(z1 * deltaTime);
            var e2 = MathF.Exp(z2 * deltaTime);
            var invTwoZb = 1.0f / (2.0f * zb); // = 1 / (z2 - z1)
            var e1_Over_TwoZb = e1 * invTwoZb;
            var e2_Over_TwoZb = e2 * invTwoZb;
            var z1e1_Over_TwoZb = z1 * e1_Over_TwoZb;
            var z2e2_Over_TwoZb = z2 * e2_Over_TwoZb;

            posPosCoef = e1_Over_TwoZb * z2 - z2e2_Over_TwoZb + e2;
            posVelCoef = -e1_Over_TwoZb + e2_Over_TwoZb;
            velPosCoef = (z1e1_Over_TwoZb - z2e2_Over_TwoZb + e2) * z2;
            velVelCoef = -z1e1_Over_TwoZb + z2e2_Over_TwoZb;
        }
        else if (dampingRatio < 1.0f - float.Epsilon)
        {
            // Under-damped.
            var omegaZeta = angularFrequency * dampingRatio;
            var alpha = angularFrequency * MathF.Sqrt(1.0f - dampingRatio * dampingRatio);
            var expTerm = MathF.Exp(-omegaZeta * deltaTime);
            var cosTerm = MathF.Cos(alpha * deltaTime);
            var sinTerm = MathF.Sin(alpha * deltaTime);
            var invAlpha = 1.0f / alpha;
            var expSin = expTerm * sinTerm;
            var expCos = expTerm * cosTerm;
            var expOmegaZetaSin_Over_Alpha = expTerm * omegaZeta * sinTerm * invAlpha;


            posPosCoef = expCos + expOmegaZetaSin_Over_Alpha;
            posVelCoef = expSin * invAlpha;
            velPosCoef = -expSin * alpha - omegaZeta * expOmegaZetaSin_Over_Alpha;
            velVelCoef = expCos - expOmegaZetaSin_Over_Alpha;
        }
        else
        {
            // Critically damped.
            var expTerm = MathF.Exp(-angularFrequency * deltaTime);
            var timeExp = deltaTime * expTerm;


            var timeExpFreq = timeExp * angularFrequency;
            posPosCoef = timeExpFreq + expTerm;
            posVelCoef = timeExp;
            velPosCoef = -angularFrequency * timeExpFreq;
            velVelCoef = -timeExpFreq + expTerm;
        }
    }

    public (float newPos, float newVel) Update(float pos, float vel, float equilibriumPos)
    {
        var oldPos = pos - equilibriumPos; // update in equilibrium relative space
        var oldVel = vel;
        var newPos = oldPos * posPosCoef + oldVel * posVelCoef + equilibriumPos;
        var newVel = oldPos * velPosCoef + oldVel * velVelCoef;

        return (newPos, newVel);
    }
}