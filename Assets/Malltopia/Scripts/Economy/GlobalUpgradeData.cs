using UnityEngine;

namespace Malltopia
{
    [CreateAssetMenu(menuName = "Malltopia/Economy/Global Upgrade", fileName = "GlobalUpgrade")]
    public class GlobalUpgradeData : ScriptableObject
    {
        public string upgradeId;
        public string displayName;
        public string effectType;
        public double effectValue;
        public string costType;
        public double costAmount;
    }
}
