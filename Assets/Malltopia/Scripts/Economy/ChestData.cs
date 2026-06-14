using UnityEngine;

namespace Malltopia
{
    [CreateAssetMenu(menuName = "Malltopia/Economy/Chest", fileName = "Chest")]
    public class ChestData : ScriptableObject
    {
        public string chestId;
        public string displayName;
        public int diamondCost;
        public int rewardCount = 6;
    }
}
