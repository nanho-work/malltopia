using UnityEngine;

namespace Malltopia
{
    [CreateAssetMenu(menuName = "Malltopia/Equipment/Dismantle Config", fileName = "EquipmentDismantleConfig")]
    public class EquipmentDismantleConfigData : ScriptableObject
    {
        public string configId;
        public string grade;
        public int starDustAmount;
    }
}
