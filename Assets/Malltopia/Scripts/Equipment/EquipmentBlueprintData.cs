using UnityEngine;

namespace Malltopia
{
    [CreateAssetMenu(menuName = "Malltopia/Equipment/Blueprint", fileName = "EquipmentBlueprint")]
    public class EquipmentBlueprintData : ScriptableObject
    {
        public string blueprintId;
        public string targetItemId;
        public string grade;
        public string slot;
    }
}
