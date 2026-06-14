using UnityEngine;

namespace Malltopia
{
    [CreateAssetMenu(menuName = "Malltopia/Equipment/Option Pool", fileName = "EquipmentOptionPool")]
    public class EquipmentOptionPoolData : ScriptableObject
    {
        public string poolId;
        public string grade;
        public string slot;
        public string[] optionIds;
    }
}
