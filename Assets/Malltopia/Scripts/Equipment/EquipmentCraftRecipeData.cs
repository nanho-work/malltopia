using UnityEngine;

namespace Malltopia
{
    [CreateAssetMenu(menuName = "Malltopia/Equipment/Craft Recipe", fileName = "EquipmentCraftRecipe")]
    public class EquipmentCraftRecipeData : ScriptableObject
    {
        public string recipeId;
        public string targetItemId;
        public string requiredBlueprintId;
        public string requiredMaterialGrade;
        public int requiredMaterialCount = 5;
    }
}
