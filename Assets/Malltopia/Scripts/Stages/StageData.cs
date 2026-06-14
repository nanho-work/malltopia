using UnityEngine;

namespace Malltopia
{
    [CreateAssetMenu(menuName = "Malltopia/Stages/Stage", fileName = "Stage")]
    public class StageData : ScriptableObject
    {
        public string stageId;
        public string themeId;
        public string displayName;
        public int stageNumber;
        public int gridWidth = 11;
        public int gridHeight = 20;
        public StageScrollMode scrollMode = StageScrollMode.FixedNoScroll;
        public string[] productIds;
        public double startingGold = 5d;
        public int startingCustomerCount = 1;
        public int startingStaffCount;
        public NextStageCostRule nextStageCostRule = NextStageCostRule.MaxFinalUpgradeCostMultiplier;
        public float nextStageCostMultiplier = 1.5f;
        public double nextStageCostOverride;
        public StageClearConditionType clearConditionType = StageClearConditionType.AllProductsMaxLevel;
        public string[] requiredMaxLevelProductIds;
        public int maxProductStandCount;
        public string clearRewardGroupId;
    }
}
