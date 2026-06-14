using UnityEngine;

namespace Malltopia
{
    [CreateAssetMenu(menuName = "Malltopia/Economy/Timed Boost", fileName = "TimedBoost")]
    public class TimedBoostConfigData : ScriptableObject
    {
        public string boostId;
        public string displayName;
        public string boostType;
        public float multiplier = 2f;
        public int durationSeconds = 900;
    }
}
