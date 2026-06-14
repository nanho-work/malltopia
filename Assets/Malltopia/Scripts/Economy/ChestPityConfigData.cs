using UnityEngine;

namespace Malltopia
{
    [CreateAssetMenu(menuName = "Malltopia/Economy/Chest Pity Config", fileName = "ChestPityConfig")]
    public class ChestPityConfigData : ScriptableObject
    {
        public string pityConfigId;
        public string chestId;
        public int requiredOpenCount = 50;
        public string guaranteedRewardType = "LegendaryBlueprint";
        public bool resetOnTargetReward = true;
    }
}
