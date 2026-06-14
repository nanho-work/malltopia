using System;

namespace Malltopia
{
    public class StaffProgressionService
    {
        public float GetEffectiveProductionTime(
            StaffData staff,
            ProductData product,
            int staffLevel,
            float productionSpeedBonusPct)
        {
            if (staff == null || product == null)
            {
                return 0f;
            }

            staffLevel = Math.Max(1, staffLevel);
            var levelMultiplier = Math.Pow(staff.workTimeMultiplierPerLevel, staffLevel - 1);
            var productionSpeedMultiplier = 1d + Math.Max(0f, productionSpeedBonusPct) / 100d;
            var time = product.baseProductionTimeSec * levelMultiplier / productionSpeedMultiplier;
            return (float)Math.Max(staff.minProductionTimeSec, time);
        }

        public double GetUpgradeCost(StaffData staff, int currentLevel)
        {
            if (staff == null || currentLevel >= staff.maxLevel)
            {
                return 0d;
            }

            currentLevel = Math.Max(1, currentLevel);
            return CurrencyMath.Sanitize(staff.upgradeBaseCost * Math.Pow(staff.costGrowth, currentLevel - 1));
        }
    }
}
