using UnityEngine;

namespace Malltopia
{
    [CreateAssetMenu(menuName = "Malltopia/Stages/Clear Reward", fileName = "StageClearReward")]
    public class StageClearRewardData : ScriptableObject
    {
        public string rewardId;
        public string rewardGroupId;
        public string stageId;
        public RewardType rewardType;
        public string rewardTargetId;
        public int amount = 1;
        public bool enabledInMvp;
    }
}
