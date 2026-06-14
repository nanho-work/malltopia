namespace Malltopia
{
    public class RemoteConfigService
    {
        private readonly FirebaseRemoteConfigDefaultsData defaultsData;

        public RemoteConfigService(FirebaseRemoteConfigDefaultsData defaultsData)
        {
            this.defaultsData = defaultsData;
        }

        public string LatestVersion
        {
            get { return defaultsData != null ? defaultsData.latestVersion : "0.1.0"; }
        }

        public string MinimumSupportedVersion
        {
            get { return defaultsData != null ? defaultsData.minimumSupportedVersion : "0.1.0"; }
        }

        public bool MaintenanceMode
        {
            get { return defaultsData != null && defaultsData.maintenanceMode; }
        }

        public string MaintenanceMessage
        {
            get { return defaultsData != null ? defaultsData.maintenanceMessage : string.Empty; }
        }
    }
}
