namespace Malltopia
{
    public class ActionAvailabilityService
    {
        public bool CanUnlockProduct(double currentGold, ProductData product, bool alreadyUnlocked)
        {
            return product != null
                   && !alreadyUnlocked
                   && CurrencyMath.Sanitize(currentGold) >= CurrencyMath.Sanitize(product.unlockCost);
        }

        public bool CanUpgradeProduct(double currentGold, ProductData product, double upgradeCost, int currentLevel)
        {
            return product != null
                   && currentLevel < product.maxLevel
                   && CurrencyMath.Sanitize(currentGold) >= CurrencyMath.Sanitize(upgradeCost);
        }

        public bool CanActivateProductionSlot(double currentGold, ProductProductionSlotData slot, int starCount, bool alreadyActivated)
        {
            return slot != null
                   && !alreadyActivated
                   && starCount >= slot.requiredStarCount
                   && CurrencyMath.Sanitize(currentGold) >= CurrencyMath.Sanitize(slot.activationCost);
        }

        public bool CanBuyStageUpgrade(double currentGold, StageUpgradeData upgrade, int purchasedCount)
        {
            return upgrade != null
                   && purchasedCount < upgrade.maxPurchaseCount
                   && CurrencyMath.Sanitize(currentGold) >= CurrencyMath.Sanitize(upgrade.cost);
        }
    }
}
