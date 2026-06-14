using UnityEngine;

namespace Malltopia
{
    [CreateAssetMenu(menuName = "Malltopia/Equipment/Option", fileName = "EquipmentOption")]
    public class EquipmentOptionData : ScriptableObject
    {
        public string optionId;
        public string displayName;
        public string effectType;
        public double effectValue;
        public string target;
    }
}
