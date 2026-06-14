using UnityEngine;

namespace Malltopia
{
    [CreateAssetMenu(menuName = "Malltopia/Staff/Staff", fileName = "Staff")]
    public class StaffData : ScriptableObject
    {
        public string staffId;
        public string displayName;
        public StaffRole role = StaffRole.Generalist;
        public AddSource addSource = AddSource.StageUpgrade;
        public float moveTimePerTileSec = 1f;
        public int carryCapacity = 1;
        public int maxCarryCapacity = 2;
        public float workSpeedMultiplier = 1f;
        public float minProductionTimeSec = 0.6f;
        public int maxLevel = 50;
        public double upgradeBaseCost = 100d;
        public float workTimeMultiplierPerLevel = 0.97f;
        public float costGrowth = 1.15f;
        public string prefabKey;
        public string iconKey;
    }
}
