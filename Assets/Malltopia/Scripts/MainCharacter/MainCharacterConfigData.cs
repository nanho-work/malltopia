using UnityEngine;

namespace Malltopia
{
    [CreateAssetMenu(menuName = "Malltopia/Main Character/Config", fileName = "MainCharacterConfig")]
    public class MainCharacterConfigData : ScriptableObject
    {
        public string mainCharacterId;
        public float moveTimePerTileSec = 1f;
        public int carryCapacity = 1;
        public int maxCarryCapacity = 2;
        public float orderTakeTimeSec = 0.4f;
        public float workSpeedMultiplier = 1f;
        public float deliveryHandoffTimeSec = 0.5f;
        public float baseIncomeBonusPct;
        public float sGradeChancePct;
    }
}
