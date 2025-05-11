using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Ludo.Core.Analytics
{
    /// <summary>
    /// A debug implementation of IAnalyticsProvider that logs events to the Unity console.
    /// Useful for development and testing.
    /// </summary>
    public class DebugAnalyticsProvider : IAnalyticsProvider
    {
        private DebugProviderConfig _config;
        private bool _isEnabled;
        private bool _isInitialized;

        /// <summary>
        /// The name of this provider.
        /// </summary>
        public string ProviderName => "Debug Analytics Provider";

        /// <summary>
        /// Whether this provider has been initialized.
        /// </summary>
        public bool IsInitialized => _isInitialized;

        /// <summary>
        /// Whether this provider is currently enabled.
        /// </summary>
        public bool IsEnabled => _isEnabled && _isInitialized;

        /// <summary>
        /// Initializes the debug provider with the given configuration.
        /// </summary>
        /// <param name="config">The provider configuration</param>
        public void Initialize(ProviderConfig config)
        {
            if (config is not DebugProviderConfig debugConfig)
            {
                Debug.LogError($"{ProviderName} requires a DebugProviderConfig, but received {config.GetType().Name}");
                return;
            }

            _config = debugConfig;
            _isInitialized = true;
            _isEnabled = _config.IsEnabledOnStart;
            
            Debug.Log($"{ProviderName} initialized. Verbose logging: {_config.VerboseLogging}");
        }

        /// <summary>
        /// Tracks an analytics event by logging it to the console.
        /// </summary>
        /// <param name="analyticsEvent">The event to track</param>
        public void TrackEvent(AnalyticsEvent analyticsEvent)
        {
            if (!IsEnabled || analyticsEvent == null)
            {
                return;
            }

            var timestamp = analyticsEvent.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var logMessage = string.Format(_config.EventLogFormat, analyticsEvent.EventName, timestamp);
            
            if (_config.VerboseLogging && analyticsEvent.Parameters.Count > 0)
            {
                var sb = new StringBuilder(logMessage);
                sb.AppendLine();
                sb.AppendLine("Parameters:");
                
                foreach (var param in analyticsEvent.Parameters)
                {
                    sb.AppendLine($"  {param.Key} = {FormatParameterValue(param.Value)}");
                }
                
                logMessage = sb.ToString();
            }
            
            Debug.Log(logMessage);
        }

        /// <summary>
        /// Sets a user property by logging it to the console.
        /// </summary>
        /// <param name="propertyName">The name of the property</param>
        /// <param name="propertyValue">The value of the property</param>
        public void SetUserProperty(string propertyName, object propertyValue)
        {
            if (!IsEnabled)
            {
                return;
            }

            var logMessage = string.Format(_config.PropertyLogFormat, propertyName, FormatParameterValue(propertyValue));
            Debug.Log(logMessage);
        }

        /// <summary>
        /// Simulates flushing events to a server.
        /// </summary>
        public void FlushEvents()
        {
            if (!IsEnabled)
            {
                return;
            }

            Debug.Log($"[{ProviderName}] Flushing events (simulation)");
        }

        /// <summary>
        /// Enables or disables this provider.
        /// </summary>
        /// <param name="enable">Whether to enable the provider</param>
        public void Enable(bool enable)
        {
            _isEnabled = enable;
            Debug.Log($"[{ProviderName}] {(enable ? "Enabled" : "Disabled")}");
        }

        /// <summary>
        /// Sets consent status for this provider.
        /// </summary>
        /// <param name="hasConsentedToProvider">Whether the user has consented to this provider</param>
        /// <param name="consentDetails">Additional consent details</param>
        public void SetConsent(bool hasConsentedToProvider, Dictionary<string, object> consentDetails)
        {
            Debug.Log($"[{ProviderName}] Consent set to: {hasConsentedToProvider}");
            
            if (_config.VerboseLogging && consentDetails != null && consentDetails.Count > 0)
            {
                var sb = new StringBuilder();
                sb.AppendLine("Consent details:");
                
                foreach (var detail in consentDetails)
                {
                    sb.AppendLine($"  {detail.Key} = {FormatParameterValue(detail.Value)}");
                }
                
                Debug.Log(sb.ToString());
            }
            
            // Update enabled state based on consent
            Enable(hasConsentedToProvider);
        }

        /// <summary>
        /// Formats a parameter value for logging.
        /// </summary>
        /// <param name="value">The value to format</param>
        /// <returns>A string representation of the value</returns>
        private string FormatParameterValue(object value)
        {
            if (value == null)
            {
                return "null";
            }

            // Handle dictionaries
            if (value is IDictionary<string, object> dict)
            {
                var sb = new StringBuilder("{");
                foreach (var kvp in dict)
                {
                    sb.Append($"{kvp.Key}:{FormatParameterValue(kvp.Value)}, ");
                }
                if (dict.Count > 0)
                {
                    sb.Length -= 2; // Remove trailing comma and space
                }
                sb.Append("}");
                return sb.ToString();
            }

            // Handle lists
            if (value is IList<object> list)
            {
                var sb = new StringBuilder("[");
                foreach (var item in list)
                {
                    sb.Append($"{FormatParameterValue(item)}, ");
                }
                if (list.Count > 0)
                {
                    sb.Length -= 2; // Remove trailing comma and space
                }
                sb.Append("]");
                return sb.ToString();
            }

            // Handle arrays
            if (value is Array array)
            {
                var sb = new StringBuilder("[");
                foreach (var item in array)
                {
                    sb.Append($"{FormatParameterValue(item)}, ");
                }
                if (array.Length > 0)
                {
                    sb.Length -= 2; // Remove trailing comma and space
                }
                sb.Append("]");
                return sb.ToString();
            }

            // Default to ToString()
            return value.ToString();
        }
    }
}
