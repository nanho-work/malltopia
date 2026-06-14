using System;
using UnityEngine;

namespace Malltopia
{
    [Serializable]
    public class ProductLevelCurveSegment
    {
        public int levelStart = 1;
        public int levelEnd = 1;
        public double levelBaseSaleGoldAtStart;
        public LevelGrowthType saleGrowthType = LevelGrowthType.Add;
        public double saleGrowthValue;
        public double upgradeCostGoldAtStart;
        public LevelGrowthType upgradeCostGrowthType = LevelGrowthType.Add;
        public double upgradeCostGrowthValue;
        public string note;
    }

    [CreateAssetMenu(menuName = "Malltopia/Products/Level Curve", fileName = "ProductLevelCurve")]
    public class ProductLevelCurveData : ScriptableObject
    {
        public string curveId;
        public ProductLevelCurveSegment[] segments;
    }
}
