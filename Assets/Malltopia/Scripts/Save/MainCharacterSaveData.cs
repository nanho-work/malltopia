using System;

namespace Malltopia
{
    [Serializable]
    public class MainCharacterSaveData
    {
        public string selectedCharacterId = "default_main_character";
        public string[] equippedItemIds = new string[0];
    }
}
