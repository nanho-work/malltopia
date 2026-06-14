using UnityEngine;

namespace Malltopia
{
    [CreateAssetMenu(menuName = "Malltopia/Orders/Customer Order Config", fileName = "CustomerOrderConfig")]
    public class CustomerOrderConfigData : ScriptableObject
    {
        public string configId = "mvp_default_order";
        public FirstOrderMode firstOrderMode = FirstOrderMode.StartingStandOnly;
        public EligibleProductRule eligibleProductRule = EligibleProductRule.UnlockedAndProductionSlotActivated;
        public ProductSelectionRule productSelectionRule = ProductSelectionRule.UniformEligibleProducts;
        public DistinctProductCountRule distinctProductCountRule = DistinctProductCountRule.UniformRange;
        public int maxDistinctProductTypes = 2;
        public QuantityRule quantityRule = QuantityRule.UniformIntRange;
        public int minQuantityPerProduct = 1;
        public int maxQuantityPerProduct = 3;
        public WorkAssignmentRule workAssignmentRule = WorkAssignmentRule.OrderTakerThenProductUnitTasks;
        public GoldRewardTiming goldRewardTiming = GoldRewardTiming.PerProductDelivered;
    }
}
