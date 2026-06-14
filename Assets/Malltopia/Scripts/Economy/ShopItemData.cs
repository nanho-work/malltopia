using UnityEngine;

namespace Malltopia
{
    [CreateAssetMenu(menuName = "Malltopia/Economy/Shop Item", fileName = "ShopItem")]
    public class ShopItemData : ScriptableObject
    {
        public string shopItemId;
        public string displayName;
        public string shopItemType;
        public string linkedContentId;
        public string costType;
        public double costAmount;
        public int sortOrder;
    }
}
