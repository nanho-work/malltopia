using UnityEngine;

namespace Malltopia
{
    [CreateAssetMenu(menuName = "Malltopia/Economy/Number Format Config", fileName = "NumberFormatConfig")]
    public class NumberFormatConfigData : ScriptableObject
    {
        public string[] suffixes = { "k", "m", "b", "t", "aa", "ab", "ac" };
        public int decimals = 1;
    }
}
