using System;

namespace Malltopia
{
    public static class CurrencyMath
    {
        public static double Sanitize(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value) || value < 0d)
            {
                return 0d;
            }

            return value;
        }

        public static double Round(double value, int roundingUnit)
        {
            value = Sanitize(value);

            if (roundingUnit <= 1)
            {
                return Math.Round(value);
            }

            return Math.Round(value / roundingUnit) * roundingUnit;
        }
    }
}
