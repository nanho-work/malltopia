using UnityEngine;

namespace Malltopia
{
    [CreateAssetMenu(menuName = "Malltopia/Customers/Customer Type", fileName = "CustomerType")]
    public class CustomerTypeData : ScriptableObject
    {
        public string customerTypeId;
        public string displayName;
        public float orderQuantityMultiplier = 1f;
        public float expensiveProductWeight = 1f;
        public float moveSpeed = 1.5f;
        public int spawnWeight = 100;
        public string prefabKey;
    }
}
