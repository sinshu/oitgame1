using System;

namespace OitGame1
{
    public static class Utility
    {
        public static double ClampAbs(double value, double maxValue)
        {
            var sign = Math.Sign(value);
            var abs = Math.Abs(value);
            if (abs > maxValue)
            {
                return sign * maxValue;
            }
            else
            {
                return sign * abs;
            }
        }

        public static double AddClampMin(double value, double delta, double minValue)
        {
            var result = value + delta;
            if (result < minValue)
            {
                result = minValue;
            }
            return result;
        }

        public static double AddClampMax(double value, double delta, double maxValue)
        {
            var result = value + delta;
            if (result > maxValue)
            {
                result = maxValue;
            }
            return result;
        }

        public static double DecreaseAbs(double value, double delta)
        {
            var sign = Math.Sign(value);
            var abs = Math.Abs(value);
            var result = abs - delta;
            if (result < 0)
            {
                result = 0;
            }
            return sign * result;
        }
    }
}
