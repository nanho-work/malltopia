using System;

namespace Malltopia
{
    [Serializable]
    public class ProductOrderCandidate
    {
        public string productId;
        public bool isStartingStand;
        public bool isUnlocked;
        public bool hasActiveProductionSlot;
    }

    [Serializable]
    public class CustomerOrderLine
    {
        public string productId;
        public int quantity;
    }

    [Serializable]
    public class CustomerOrder
    {
        public string customerTypeId;
        public CustomerOrderLine[] lines;
    }

    [Serializable]
    public class ProductUnitTask
    {
        public string productId;
        public int lineIndex;
        public int unitIndex;
    }
}
