using UnityEngine;

namespace Malltopia
{
    [CreateAssetMenu(menuName = "Malltopia/Stages/Stage Upgrade", fileName = "StageUpgrade")]
    public class StageUpgradeData : ScriptableObject
    {
        public string upgradeId;
        public string themeId;
        public string stageId;
        public string displayName;
        public StageUpgradeTargetType targetType;
        public string targetId;
        public StageUpgradeEffectType effectType;
        public double effectValue;
        public double cost;
        public int maxPurchaseCount = 1;
        public int sortOrder = 1;
    }
}
