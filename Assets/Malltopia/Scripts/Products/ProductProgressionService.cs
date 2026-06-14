using System;

namespace Malltopia
{
    public class ProductProgressionService
    {
        public ProductProgressionResult Evaluate(
            ProductData product,
            ProductLevelCurveData curve,
            ProductStarMilestoneData[] milestones,
            int level,
            double productRewardUpgradeMultiplier = 1d,
            double equipmentIncomeMultiplier = 1d,
            double characterPassiveIncomeMultiplier = 1d,
            double timedBoostMultiplier = 1d,
            double doubleProductMultiplier = 1d,
            double sGradeProductMultiplier = 1d)
        {
            level = ClampLevel(product, level);

            var starCount = GetStarCount(product, milestones, level);
            var baseSale = EvaluateSaleBase(curve, level);
            var starMultiplier = GetStarMultiplier(milestones, level);
            var sale = baseSale
                * starMultiplier
                * productRewardUpgradeMultiplier
                * equipmentIncomeMultiplier
                * characterPassiveIncomeMultiplier
                * timedBoostMultiplier
                * doubleProductMultiplier
                * sGradeProductMultiplier;

            return new ProductProgressionResult
            {
                level = level,
                starCount = starCount,
                levelBaseSaleGold = CurrencyMath.Sanitize(baseSale),
                saleGold = CurrencyMath.Sanitize(sale),
                upgradeCostGold = GetUpgradeCost(product, curve, level),
                totalDiamondReward = GetTotalDiamondReward(milestones, level)
            };
        }

        public int GetStarCount(ProductData product, ProductStarMilestoneData[] milestones, int level)
        {
            if (milestones == null)
            {
                return 0;
            }

            var count = 0;

            for (var i = 0; i < milestones.Length; i++)
            {
                var milestone = milestones[i];

                if (milestone != null && level >= milestone.requiredLevel)
                {
                    count++;
                }
            }

            if (product != null && product.maxStarCount > 0)
            {
                return Math.Min(count, product.maxStarCount);
            }

            return count;
        }

        public int GetNewDiamondReward(ProductStarMilestoneData[] milestones, int previousLevel, int newLevel)
        {
            if (milestones == null || newLevel <= previousLevel)
            {
                return 0;
            }

            var reward = 0;

            for (var i = 0; i < milestones.Length; i++)
            {
                var milestone = milestones[i];

                if (milestone != null && previousLevel < milestone.requiredLevel && newLevel >= milestone.requiredLevel)
                {
                    reward += Math.Max(0, milestone.diamondReward);
                }
            }

            return reward;
        }

        public double GetUpgradeCost(ProductData product, ProductLevelCurveData curve, int currentLevel)
        {
            if (product != null && currentLevel >= product.maxLevel)
            {
                return 0d;
            }

            return CurrencyMath.Sanitize(EvaluateUpgradeCost(curve, currentLevel));
        }

        public bool CanShowProductionSlot(ProductProductionSlotData slot, int starCount, bool alreadyActivated)
        {
            return slot != null
                   && !alreadyActivated
                   && starCount >= slot.requiredStarCount
                   && slot.activationMode == ProductionSlotActivationMode.FreeTap;
        }

        public double CalculateNextStageCost(
            StageData stage,
            ProductData[] products,
            ProductLevelCurveData[] curves)
        {
            if (stage == null)
            {
                return 0d;
            }

            if (stage.nextStageCostOverride > 0d)
            {
                return CurrencyMath.Sanitize(stage.nextStageCostOverride);
            }

            if (stage.nextStageCostRule != NextStageCostRule.MaxFinalUpgradeCostMultiplier)
            {
                return 0d;
            }

            var maxFinalUpgradeCost = 0d;

            if (products != null)
            {
                for (var i = 0; i < products.Length; i++)
                {
                    var product = products[i];

                    if (product == null)
                    {
                        continue;
                    }

                    var curve = FindCurve(curves, product.levelCurveId);
                    var finalUpgradeLevel = Math.Max(1, product.maxLevel - 1);
                    var finalUpgradeCost = GetUpgradeCost(product, curve, finalUpgradeLevel);
                    maxFinalUpgradeCost = Math.Max(maxFinalUpgradeCost, finalUpgradeCost);
                }
            }

            return CurrencyMath.Sanitize(maxFinalUpgradeCost * stage.nextStageCostMultiplier);
        }

        private static int ClampLevel(ProductData product, int level)
        {
            level = Math.Max(1, level);

            if (product != null && product.maxLevel > 0)
            {
                return Math.Min(level, product.maxLevel);
            }

            return level;
        }

        private static double GetStarMultiplier(ProductStarMilestoneData[] milestones, int level)
        {
            if (milestones == null)
            {
                return 1d;
            }

            var multiplier = 1d;

            for (var i = 0; i < milestones.Length; i++)
            {
                var milestone = milestones[i];

                if (milestone != null && level >= milestone.requiredLevel)
                {
                    multiplier *= Math.Max(1d, milestone.rewardMultiplier);
                }
            }

            return multiplier;
        }

        private static int GetTotalDiamondReward(ProductStarMilestoneData[] milestones, int level)
        {
            if (milestones == null)
            {
                return 0;
            }

            var reward = 0;

            for (var i = 0; i < milestones.Length; i++)
            {
                var milestone = milestones[i];

                if (milestone != null && level >= milestone.requiredLevel)
                {
                    reward += Math.Max(0, milestone.diamondReward);
                }
            }

            return reward;
        }

        private static double EvaluateSaleBase(ProductLevelCurveData curve, int level)
        {
            var segment = FindSegment(curve, level);
            return segment == null ? 0d : EvaluateGrowth(segment.levelBaseSaleGoldAtStart, segment.saleGrowthType, segment.saleGrowthValue, level - segment.levelStart);
        }

        private static double EvaluateUpgradeCost(ProductLevelCurveData curve, int level)
        {
            var segment = FindSegment(curve, level);
            return segment == null ? 0d : EvaluateGrowth(segment.upgradeCostGoldAtStart, segment.upgradeCostGrowthType, segment.upgradeCostGrowthValue, level - segment.levelStart);
        }

        private static ProductLevelCurveSegment FindSegment(ProductLevelCurveData curve, int level)
        {
            if (curve == null || curve.segments == null)
            {
                return null;
            }

            ProductLevelCurveSegment fallback = null;

            for (var i = 0; i < curve.segments.Length; i++)
            {
                var segment = curve.segments[i];

                if (segment == null)
                {
                    continue;
                }

                if (fallback == null || segment.levelStart < fallback.levelStart)
                {
                    fallback = segment;
                }

                if (level >= segment.levelStart && level <= segment.levelEnd)
                {
                    return segment;
                }
            }

            return fallback;
        }

        private static ProductLevelCurveData FindCurve(ProductLevelCurveData[] curves, string curveId)
        {
            if (curves == null)
            {
                return null;
            }

            for (var i = 0; i < curves.Length; i++)
            {
                var curve = curves[i];

                if (curve != null && curve.curveId == curveId)
                {
                    return curve;
                }
            }

            return null;
        }

        private static double EvaluateGrowth(double startValue, LevelGrowthType growthType, double growthValue, int offset)
        {
            offset = Math.Max(0, offset);

            switch (growthType)
            {
                case LevelGrowthType.Add:
                    return startValue + (growthValue * offset);
                case LevelGrowthType.Multiply:
                    return startValue * Math.Pow(growthValue, offset);
                case LevelGrowthType.Fixed:
                    return startValue;
                default:
                    return startValue;
            }
        }
    }
}
