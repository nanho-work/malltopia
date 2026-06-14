using System;

namespace Malltopia
{
    public class VersionGateService
    {
        public VersionGateResult Check(string currentVersion, string minimumSupportedVersion, bool maintenanceMode)
        {
            if (maintenanceMode)
            {
                return VersionGateResult.Maintenance;
            }

            if (CompareVersion(currentVersion, minimumSupportedVersion) < 0)
            {
                return VersionGateResult.UpdateRequired;
            }

            return VersionGateResult.Allowed;
        }

        private static int CompareVersion(string left, string right)
        {
            var leftParts = SplitVersion(left);
            var rightParts = SplitVersion(right);
            var max = Math.Max(leftParts.Length, rightParts.Length);

            for (var i = 0; i < max; i++)
            {
                var leftValue = i < leftParts.Length ? leftParts[i] : 0;
                var rightValue = i < rightParts.Length ? rightParts[i] : 0;

                if (leftValue < rightValue)
                {
                    return -1;
                }

                if (leftValue > rightValue)
                {
                    return 1;
                }
            }

            return 0;
        }

        private static int[] SplitVersion(string version)
        {
            if (string.IsNullOrEmpty(version))
            {
                return new[] { 0 };
            }

            var rawParts = version.Split('.');
            var parts = new int[rawParts.Length];

            for (var i = 0; i < rawParts.Length; i++)
            {
                int value;
                parts[i] = int.TryParse(rawParts[i], out value) ? value : 0;
            }

            return parts;
        }
    }
}
