using UnityEngine;

namespace Ludo.Core.Analytics
{
    /// <summary>
    /// Factory class for creating analytics service and provider instances.
    /// </summary>
    public static class AnalyticsFactory
    {
        /// <summary>
        /// Creates a new instance of the analytics service.
        /// </summary>
        /// <returns>A new IAnalyticsService instance</returns>
        public static IAnalyticsService CreateAnalyticsService()
        {
            return new AnalyticsService();
        }

        /// <summary>
        /// Creates a debug analytics provider with the given configuration.
        /// </summary>
        /// <param name="config">The debug provider configuration</param>
        /// <returns>A new debug analytics provider</returns>
        public static IAnalyticsProvider CreateDebugProvider(DebugProviderConfig config = null)
        {
            return new DebugAnalyticsProvider();
        }

        /// <summary>
        /// Creates a complete analytics service with a debug provider.
        /// </summary>
        /// <param name="debugConfig">The debug provider configuration</param>
        /// <returns>An initialized analytics service with a debug provider</returns>
        public static IAnalyticsService CreateServiceWithDebugProvider(DebugProviderConfig debugConfig = null)
        {
            // Create default config if none provided
            if (debugConfig == null)
            {
                debugConfig = ScriptableObject.CreateInstance<DebugProviderConfig>();
                debugConfig.VerboseLogging = true;
                debugConfig.IsEnabledOnStart = true;
            }

            var service = CreateAnalyticsService();
            var debugProvider = CreateDebugProvider();
            
            service.RegisterProvider(debugProvider, debugConfig);
            service.Initialize();
            
            return service;
        }
    }
}
