using System;

namespace Malltopia
{
    [Serializable]
    public class CharacterCollectionSaveData
    {
        public string selectedCharacterId = "default_main_character";
        public string[] ownedCharacterIds = new string[0];
    }
}
