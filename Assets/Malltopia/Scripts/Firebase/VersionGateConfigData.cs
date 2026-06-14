using UnityEngine;

namespace Malltopia
{
    [CreateAssetMenu(menuName = "Malltopia/Firebase/Version Gate Config", fileName = "VersionGateConfig")]
    public class VersionGateConfigData : ScriptableObject
    {
        public string latestVersion = "0.1.0";
        public string minimumSupportedVersion = "0.1.0";
        public bool maintenanceMode;
    }
}
