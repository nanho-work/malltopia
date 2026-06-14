using System;

namespace Malltopia
{
    [Serializable]
    public class GameSaveData
    {
        public int saveVersion = 1;
        public string currentThemeId;
        public string currentStageId;
        public double gold;
        public int diamond;
        public int starDust;
        public int currentCustomerCount = 1;
        public int currentStaffCount;
        public bool nextStageReady;
        public long lastSavedUnixTime;
        public MainCharacterSaveData mainCharacter = new MainCharacterSaveData();
        public ProductSaveData[] products = new ProductSaveData[0];
        public StaffSaveData[] staff = new StaffSaveData[0];
        public StageUpgradeSaveData[] stageUpgrades = new StageUpgradeSaveData[0];
        public SeenNoticeSaveData[] seenNotices = new SeenNoticeSaveData[0];
        public ActiveTimedBoostSaveData[] activeTimedBoosts = new ActiveTimedBoostSaveData[0];
        public ChestPitySaveData[] chestPityCounters = new ChestPitySaveData[0];
        public CharacterCollectionSaveData characterCollection = new CharacterCollectionSaveData();
    }
}
