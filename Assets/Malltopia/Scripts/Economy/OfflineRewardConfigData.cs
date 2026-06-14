using UnityEngine;

namespace Malltopia
{
    [CreateAssetMenu(menuName = "Malltopia/Economy/Offline Reward Config", fileName = "OfflineRewardConfig")]
    public class OfflineRewardConfigData : ScriptableObject
    {
        public int maxOfflineSeconds = 10800;
        public float offlineEfficiency = 0.25f;
    }
}
