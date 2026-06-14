using UnityEngine;

namespace Malltopia
{
    [CreateAssetMenu(menuName = "Malltopia/Economy/Economy Config", fileName = "EconomyConfig")]
    public class EconomyConfigData : ScriptableObject
    {
        public double startingGold;
        public int startingDiamond;
        public int startingStarDust;
        public float nextCustomerSpawnDelaySec = 0.2f;
        public float customerPickProductDurationSec = 0.6f;
        public float leaveDelaySec = 0.2f;
        public int goldRoundingUnit = 1;
        public float autosaveIntervalSec = 10f;
    }
}
