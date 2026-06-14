using UnityEngine;

namespace Malltopia
{
    [CreateAssetMenu(menuName = "Malltopia/Products/Production Slot", fileName = "ProductProductionSlot")]
    public class ProductProductionSlotData : ScriptableObject
    {
        public string slotId;
        public string productId;
        public int slotIndex = 1;
        public int requiredStarCount;
        public ProductionSlotActivationMode activationMode = ProductionSlotActivationMode.FreeTap;
        public double activationCost;
        public string layoutPointId;
    }
}
