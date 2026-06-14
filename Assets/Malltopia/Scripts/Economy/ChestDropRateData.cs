using UnityEngine;

namespace Malltopia
{
    [CreateAssetMenu(menuName = "Malltopia/Economy/Chest Drop Rate", fileName = "ChestDropRate")]
    public class ChestDropRateData : ScriptableObject
    {
        public string dropRateId;
        public string chestId;
        public string rewardGrade;
        public float ratePct;
    }
}
