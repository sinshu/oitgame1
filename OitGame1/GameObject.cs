using System;
using System.Collections.Generic;

public abstract class GameObject
{
    private double x;
    private double y;

    public abstract double Width { get; }
    public abstract double Height { get; }

    public bool IsOverlappedWith(GameObject other)
    {
        if (other.Right > Left && other.Left < Right && other.Bottom > Top && other.Top < Bottom)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public double X
    {
        get
        {
            return x;
        }

        set
        {
            x = value;
        }
    }

    public double Y
    {
        get
        {
            return y;
        }

        set
        {
            y = value;
        }
    }

    public double Left
    {
        get
        {
            return x;
        }

        set
        {
            x = value;
        }
    }

    public double Right
    {
        get
        {
            return x + Width;
        }

        set
        {
            x = value - Width;
        }
    }

    public double Top
    {
        get
        {
            return y;
        }

        set
        {
            y = value;
        }
    }

    public double Bottom
    {
        get
        {
            return y + Height;
        }

        set
        {
            y = value - Height;
        }
    }

    public double CenterX
    {
        get
        {
            return x + Width / 2;
        }

        set
        {
            x = value - Width / 2;
        }
    }

    public double CenterY
    {
        get
        {
            return y + Height / 2;
        }

        set
        {
            y = value - Height / 2;
        }
    }
}
