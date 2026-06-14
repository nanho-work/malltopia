using UnityEngine;

namespace Malltopia
{
    [CreateAssetMenu(menuName = "Malltopia/Products/Star Milestone", fileName = "ProductStarMilestone")]
    public class ProductStarMilestoneData : ScriptableObject
    {
        public string milestoneId;
        public string productId;
        public int starIndex = 1;
        public int requiredLevel = 10;
        public float rewardMultiplier = 2f;
        public int diamondReward = 1;
        public string[] showProductionSlotIds;
    }
}
