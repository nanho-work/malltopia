using UnityEngine;

namespace Malltopia
{
    [CreateAssetMenu(menuName = "Malltopia/Products/Product", fileName = "Product")]
    public class ProductData : ScriptableObject
    {
        public string productId;
        public string displayName;
        public string themeId;
        public string stageId;
        public int standOrder = 1;
        public bool isStartingStand;
        public double unlockCost = 5d;
        public double basePrice = 2d;
        public int baseProductionAmount = 1;
        public float baseProductionTimeSec = 5f;
        public int maxLevel = 25;
        public int maxStarCount = 2;
        public int baseProductionSlotCount = 1;
        public int maxProductionSlotCount = 1;
        public double upgradeBaseCost = 3d;
        public string levelCurveId;
        public float priceGrowth = 1f;
        public float costGrowth = 1f;
    }
}
