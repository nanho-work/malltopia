using UnityEngine;

namespace Malltopia
{
    [CreateAssetMenu(menuName = "Malltopia/Stages/Theme", fileName = "Theme")]
    public class ThemeData : ScriptableObject
    {
        public string themeId;
        public string displayName;
        public int order;
        public string[] stageIds;
        public int targetStageCount = 8;
    }
}
