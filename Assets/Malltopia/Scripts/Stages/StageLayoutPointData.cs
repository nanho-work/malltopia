using UnityEngine;

namespace Malltopia
{
    [CreateAssetMenu(menuName = "Malltopia/Stages/Layout Point", fileName = "StageLayoutPoint")]
    public class StageLayoutPointData : ScriptableObject
    {
        public string pointId;
        public string stageId;
        public StageLayoutPointType pointType;
        public string productId;
        public int gridX;
        public int gridY;
        public int priority = 1;
        public int capacity = 1;

        public GridPosition Position
        {
            get { return new GridPosition(gridX, gridY); }
        }
    }
}
