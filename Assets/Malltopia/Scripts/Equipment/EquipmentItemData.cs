using UnityEngine;

namespace Malltopia
{
    [CreateAssetMenu(menuName = "Malltopia/Equipment/Item", fileName = "EquipmentItem")]
    public class EquipmentItemData : ScriptableObject
    {
        public string itemId;
        public string displayName;
        public string grade;
        public string slot;
        public double baseIncomeBonusPct;
        public string iconKey;
        public string[] optionIds;
    }
}
