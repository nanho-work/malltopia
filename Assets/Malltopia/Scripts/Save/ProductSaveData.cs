using System;

namespace Malltopia
{
    [Serializable]
    public class ProductSaveData
    {
        public string productId;
        public bool unlocked;
        public int level = 1;
        public string[] activatedProductionSlotIds = new string[0];
    }
}
