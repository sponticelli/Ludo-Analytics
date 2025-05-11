using UnityEngine;

namespace Ludo.Core.Analytics
{
    /// <summary>
    /// Configuration for the Debug Analytics Provider.
    /// </summary>
    [CreateAssetMenu(fileName = "DebugProviderConfig", menuName = "Ludo/Analytics/Debug Provider Config")]
    public class DebugProviderConfig : ProviderConfig
    {
        [Tooltip("The identifier for the Debug provider")]
        [SerializeField] private string _providerIdentifier = "Debug";
        
        [Tooltip("Whether to log verbose details including all event parameters")]
        public bool VerboseLogging = true;
        
        [Tooltip("Format string for event logs. Available placeholders: {0} = event name, {1} = timestamp")]
        public string EventLogFormat = "[DEBUG ANALYTICS] Event: {0} at {1}";
        
        [Tooltip("Format string for user property logs. Available placeholders: {0} = property name, {1} = property value")]
        public string PropertyLogFormat = "[DEBUG ANALYTICS] User Property: {0} = {1}";

        /// <summary>
        /// The identifier for this provider.
        /// </summary>
        public override string ProviderIdentifier => _providerIdentifier;
    }
}
