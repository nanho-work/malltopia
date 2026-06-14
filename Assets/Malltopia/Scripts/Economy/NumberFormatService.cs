using System;
using System.Globalization;

namespace Malltopia
{
    public class NumberFormatService
    {
        private readonly string[] suffixes;
        private readonly int decimals;

        public NumberFormatService(NumberFormatConfigData config)
        {
            suffixes = config != null && config.suffixes != null && config.suffixes.Length > 0
                ? config.suffixes
                : new[] { "k", "m", "b", "t", "aa", "ab", "ac" };
            decimals = config != null ? Math.Max(0, config.decimals) : 1;
        }

        public string FormatGold(double value)
        {
            value = CurrencyMath.Sanitize(value);

            if (value < 1000d)
            {
                return Math.Round(value).ToString("0", CultureInfo.InvariantCulture);
            }

            var unitIndex = -1;
            var scaled = value;

            while (scaled >= 1000d && unitIndex + 1 < suffixes.Length)
            {
                scaled /= 1000d;
                unitIndex++;
            }

            var format = decimals <= 0 ? "0" : "0." + new string('#', decimals);
            return scaled.ToString(format, CultureInfo.InvariantCulture) + suffixes[unitIndex];
        }
    }
}
