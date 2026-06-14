using UnityEngine;

namespace Malltopia
{
    [CreateAssetMenu(menuName = "Malltopia/Characters/Character", fileName = "Character")]
    public class CharacterData : ScriptableObject
    {
        public string characterId;
        public string displayName;
        public string grade;
        public int diamondCost;
        public string prefabKey;
        public string iconKey;
        public string[] passiveOptionIds;
    }
}
