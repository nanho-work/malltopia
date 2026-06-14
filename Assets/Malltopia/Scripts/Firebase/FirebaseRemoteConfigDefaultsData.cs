using UnityEngine;

namespace Malltopia
{
    [CreateAssetMenu(menuName = "Malltopia/Firebase/Remote Config Defaults", fileName = "FirebaseRemoteConfigDefaults")]
    public class FirebaseRemoteConfigDefaultsData : ScriptableObject
    {
        public string latestVersion = "0.1.0";
        public string minimumSupportedVersion = "0.1.0";
        public bool maintenanceMode;
        public string maintenanceMessage;
    }
}
