using UnityEngine;

namespace Malltopia
{
    [CreateAssetMenu(menuName = "Malltopia/Characters/Passive Option", fileName = "CharacterPassiveOption")]
    public class CharacterPassiveOptionData : ScriptableObject
    {
        public string optionId;
        public string displayName;
        public string effectType;
        public double effectValue;
        public string valueType;
        public string scope;
    }
}
